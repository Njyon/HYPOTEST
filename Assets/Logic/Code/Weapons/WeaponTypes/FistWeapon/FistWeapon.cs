using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FistWeapon : WeaponBase
{
	bool landed = false;
	bool startFalling = false;
	Vector3 targetDir = Vector3.zero;
	float speed = 40f;
	float interpSpeed = 10f;
	float targetAngel;
	float teleportCharacterDistance = 0.5f;
	float smashDownDistance = 0.2f;
	Ultra.Timer backupFallTimer;
	Vector3 defensiveMoveVector;
	Vector3 defensiveMoveInput;
	bool defensiveShouldMove = false;
	bool canTeleport = true;

	bool StartFalling { 
		get { return startFalling; }
		set
		{
			startFalling = value;
			if (startFalling)
			{
				GameCharacter.MovementComponent.MoveThroughCharacterLayer();
				HitDetectionStart();
			}
		}
	}

	GameCharacter targetTeleportCharacter;

	public FistWeapon() { }
	public FistWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{
		backupFallTimer = new Ultra.Timer(0.4f, true);
		backupFallTimer.onTimerFinished += OnBackupTimerFinished;
	}

	public override void InitWeapon()
	{
		base.InitWeapon();

	}

	public override void EquipWeapon()
    {
        base.EquipWeapon();
    }

    public override void UnEquipWeapon()
    {
        base.UnEquipWeapon();
		GameCharacter.GameCharacterData.MeshRenderer.enabled = true;
		GameCharacter.MovementComponent.ActivateStepup();
		GameCharacter.MovementComponent.SetLayerToDefault();
	}

    public override void UpdateWeapon(float deltaTime)
    {
        base.UpdateWeapon(deltaTime);
    }

    public override AttackAnimationData GroundAttack()   
    {
        return base.GroundAttack();
    }
    public override AttackAnimationData GroundUpAttack()    
    {
        return base.GroundUpAttack();
    }
    public override AttackAnimationData GroundDownAttack()  
    {
        return base.GroundDownAttack();
    }
    public override AttackAnimationData GroundDirectionAttack()   
    {
        return base.GroundDirectionAttack();
    }

    public override AttackAnimationData AirAttack()  
    {
        return base.AirAttack();
    }
    public override AttackAnimationData AirUpAttack()
    {
        return base.AirUpAttack();
    }
    public override AttackAnimationData AirDownAttack()
	{
		AttackAnimationData returnData = null;
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) returnData = Attack3BlendLogic(EExplicitAttackType.AirDownAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks, EAnimationType.Default);

		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += AttackTimerFinished;
		GameCharacter.MovementComponent.onCharacterGroundedChanged += OnCharacterGroundedChanged;

		landed = false;
		StartFalling = false;

		Vector3 maxDir = (GameCharacter.transform.forward + Vector3.down).normalized;
		GameCharacter target = Ultra.HypoUttilies.FindCHaracterNearestToDirectionWithMinAngel(GameCharacter.MovementComponent.CharacterCenter, Vector3.down, GameCharacter.transform.forward, 45f, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
		GameCharacter.AnimController.Combat3BlendDir = 0f;
		backupFallTimer.Start();
		if (target == null)
		{
			targetDir = maxDir;
		}else
		{
			// Aim towards feet for better results
			targetDir = (target.transform.position - GameCharacter.MovementComponent.CharacterCenter).normalized;
			float minDistance = GameCharacter.MovementComponent.Radius + target.MovementComponent.Radius + smashDownDistance;
			Vector3 newTargetPos = target.transform.position + Ultra.Utilities.IgnoreAxis(targetDir * -1, EAxis.YZ).normalized * minDistance;
			targetDir = (newTargetPos - GameCharacter.MovementComponent.CharacterCenter).normalized;
		}
		targetAngel = Vector3.Angle(targetDir, GameCharacter.transform.forward);

		return returnData;
	}
    public override AttackAnimationData AirDirectionAttack() 
    {
        return base.AirDirectionAttack();
    }

	public override AttackAnimationData DefensiveAction()
	{
		AttackAnimationData returnData = null; 
		if (canTeleport)
		{
			returnData = base.DefensiveAction();
			defensiveMoveVector = GameCharacter.MovementInput;
			canTeleport = false;
		}
		return returnData;
	}

	public override void AttackPhaseStart()
	{
		base.AttackPhaseStart();
		StartFalling = true;
	}

	public override void GroundAttackHit(GameObject hitObj)
	{
        IDamage damageInterface = GetDamageInterface(hitObj);
        if (damageInterface == null) return;
        damageInterface.DoDamage(GameCharacter, GetDamage());

		if (ComboIndexInSameAttack == 1)
		{
			GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
			if (enemyCharacter == null) return;
			RequestFlyAway(enemyCharacter);
		}
	}

	public override void GroundUpAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage());
		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null) return;

		if (enemyCharacter.CombatComponent.CanGetHooked())
		{
			if (enemyCharacter.CombatComponent != null) enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			HookCharacterToCharacter(enemyCharacter);
			if (enemyCharacter.StateMachine != null) enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.HookedToCharacter);
		}
	}

	

	public override void GroundDownAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage());

	}

	public override void GroundDirectionAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage());

		HeavyAttackLogic(hitObj);
	}

	private void HeavyAttackLogic(GameObject hitObj)
	{
		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null) return;

		if (ComboIndexInSameAttack == 2)
		{	
			RequestFlyAway(enemyCharacter);
		}else
		{
			enemyCharacter.CombatComponent.RequestFreez();
		}
	}

	public override void AirAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage());

		if (AttackIndex == 2)
		{
			GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
			if (enemyCharacter == null) return;
			RequestFlyAway(enemyCharacter);
		}
	}


	public override void AirUpAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage());

		if (ComboIndexInSameAttack > 0)
		{
			GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
			if (enemyCharacter == null) return;
			RequestFlyAway(enemyCharacter);
		}
	}

	public override void AirDownAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null || enemyCharacter.StateMachine == null || enemyCharacter.CombatComponent == null) return;
		if (enemyCharacter.CombatComponent.CanGetHooked() && enemyCharacter.StateMachine.CanSwitchToStateOrIsState(EGameCharacterState.HookedToCharacter))
		{
			enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			HookCharacterToCharacter(enemyCharacter);
			enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.HookedToCharacter);
		}
	}

	public override void AirDirectionAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage());

		HeavyAttackLogic(hitObj);
	}

	public override void EndAttackStateLogic()
	{
		UnHookAllHookedCharacerts();
		GameCharacter.MovementComponent.SetLayerToDefault();

		base.EndAttackStateLogic();
	}

	public override void GroundReset()
	{
		base.GroundReset();
		canTeleport = true;
	}

	void AttackTimerFinished()
	{
		if (CurrentAttackType == EExplicitAttackType.AirDownAttack)
		{
			GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
			if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
			if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0)
			{
				GameCharacter.AnimController.ApplyCombat3BlendTree(WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks[AttackIndex].aimBlendTypes.blendHoldAnimations);
				GameCharacter.AnimController.InCombat3Blend = true;
			}
		}
	}
	void OnCharacterGroundedChanged(bool newState)
	{
		if (CurrentAttackType == EExplicitAttackType.AirDownAttack)
		{
			if (!landed) OnAirDownHitLanding();
		}
	}

	void OnAirDownHitLanding()
	{
		landed = true;
		GameCharacter.MovementComponent.onCharacterGroundedChanged -= OnCharacterGroundedChanged;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
		GameCharacter.AnimController.InCombat3Blend = false;

		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) TriggerAttack(CurrentAttackType, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks);
		GameCharacter.AnimController.HoldAttack = false;
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.MovementComponent.SetLayerToDefault();

		CameraController.Instance.ShakeCamerea(2);

		UnHookAllHookedCharacerts();

		foreach (GameObject obj in hitObjects)
		{
			OnGroundAttackHit(obj);
		}

		HitDetectionEnd();
	}

	void OnGroundAttackHit(GameObject hitObject)
	{
		IDamage damageInterface = GetDamageInterface(hitObject);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage());

		GameCharacter enemyCharacter = hitObject.GetComponent<GameCharacter>();
		if (enemyCharacter == null) return;
		RequestFlyAway(enemyCharacter);
		FreezAfterPush(enemyCharacter);
	}

	async void FreezAfterPush(GameCharacter character)
	{
		await new WaitForSeconds(CurrentAttack.extraData.flyAwayTime);
		character.CombatComponent.RequestFreez(CurrentAttack.extraData.freezTime);
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		base.PostAttackStateLogic(deltaTime);
		backupFallTimer.Update(deltaTime);
		if (CurrentAttackType == EExplicitAttackType.AirDownAttack)
		{
			GameCharacter.AnimController.Combat3BlendDir = Mathf.Lerp(GameCharacter.AnimController.Combat3BlendDir, Ultra.Utilities.Remap(targetAngel, 0, 180, 1, -1), deltaTime * interpSpeed);
			UpdateAirDownAttack(deltaTime);
		}
		else if (CurrentAttackType == EExplicitAttackType.DefensiveAction)
		{
			if (defensiveShouldMove)
			{
				Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, defensiveMoveVector.normalized, defensiveMoveVector.magnitude, Color.white, 10f, 100, DebugAreas.Combat);

				RotateCharacterToTeleportLocationOrEnemy();

				float deltaTimeScale = 1f / Time.deltaTime;
				defensiveMoveVector *= deltaTimeScale;
				GameCharacter.MovementComponent.MovementVelocity = defensiveMoveVector;
				GameCharacter.MovementComponent.DeactiveStepup();

				defensiveShouldMove = false;
				DidMove();
			}
		}
	}

	private void RotateCharacterToTeleportLocationOrEnemy()
	{
		// Rotate Character
		Vector3 targetLocation = GameCharacter.MovementComponent.CharacterCenter + defensiveMoveVector;
		Ultra.Utilities.DrawWireSphere(targetLocation, .1f, Color.black, 10f, 200, DebugAreas.Combat);

		Vector3 dir = Vector3.zero;
		if (targetTeleportCharacter != null)
			dir = targetTeleportCharacter.MovementComponent.CharacterCenter - targetLocation;
		else
			dir = targetLocation - GameCharacter.MovementComponent.CharacterCenter;

		Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, dir.normalized, dir.magnitude, Color.red, 10f, 200, DebugAreas.Combat);
		dir = new Vector3(dir.x, 0f, 0f);
		GameCharacter.RotateToDir(dir);
		Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, dir.normalized, dir.magnitude, Color.cyan, 10f, 200, DebugAreas.Combat);
	}

	void UpdateAirDownAttack(float deltaTime)
	{
		if (!landed && StartFalling)
		{
			Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
			velocity = targetDir * speed;
			GameCharacter.MovementComponent.MovementVelocity = velocity;
		}
	}

	void OnBackupTimerFinished()
	{
		if (!landed && !StartFalling)
			StartFalling = true;
	}

	public override void DefensiveActionStart()
	{
		float angelTreshold = 5f;
		targetTeleportCharacter = Ultra.HypoUttilies.FindCharactereNearestToDirectionTresholdWithRange(GameCharacter.MovementComponent.CharacterCenter, defensiveMoveVector.normalized.magnitude > 0 ? defensiveMoveVector.normalized : GameCharacter.transform.forward, angelTreshold, Ultra.Utilities.IgnoreAxis(defensiveMoveVector, EAxis.YZ).normalized.magnitude > 0 ? Ultra.Utilities.IgnoreAxis(defensiveMoveVector, EAxis.YZ).normalized : GameCharacter.transform.forward, CurrentDefensiveAction.extraData.rangeValue, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
		Vector3 targetPosition = Vector3.zero;
		if (targetTeleportCharacter != null)
		{
			Vector3 dir =  GameCharacter.MovementComponent.CharacterCenter - targetTeleportCharacter.MovementComponent.CharacterCenter;
			dir = new Vector3(dir.x, 0f, 0f).normalized;
			float lenght = targetTeleportCharacter.MovementComponent.CapsuleCollider.radius + GameCharacter.MovementComponent.CapsuleCollider.radius + teleportCharacterDistance;
			targetPosition = targetTeleportCharacter.MovementComponent.CharacterCenter + dir * lenght;
			Ultra.Utilities.DrawWireSphere(targetPosition, .5f, Color.red, 10f, 100, DebugAreas.Combat);
			targetTeleportCharacter.MovementComponent.RequestMovementOverride(0.5f);
		}
		else
		{
			targetPosition = GameCharacter.MovementComponent.CharacterCenter + (GameCharacter.MovementInput.normalized.magnitude > 0 ? new Vector3(GameCharacter.MovementInput.x, 0f, 0f) : GameCharacter.transform.forward)* (CurrentDefensiveAction.extraData.rangeValue / 2);
		}
		defensiveMoveVector = targetPosition - GameCharacter.MovementComponent.CharacterCenter;
		defensiveShouldMove = true;

		SpawnedWeapon?.SetActive(false);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = false;
		GameCharacter.MovementComponent.MoveThroughCharacterLayer();
	}	

	public override void DefensiveActionEnd()
	{
		SpawnedWeapon?.SetActive(true);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = true;
		GameCharacter.RequestBestCharacterState();
	}

	async void DidMove()
	{
		await new WaitForEndOfFrame();
		await new WaitForEndOfFrame();

		GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
		GameCharacter.MovementComponent.ActivateStepup();
		GameCharacter.MovementComponent.SetLayerToDefault();
	}

	public override void DefensiveActionStateEnd()
	{
		base.DefensiveActionStateEnd();
		SpawnedWeapon?.SetActive(true);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = true;
		GameCharacter.MovementComponent.SetLayerToDefault();
		defensiveMoveVector = Vector3.zero;

		if (defensiveShouldMove)
			Ultra.Utilities.Instance.DebugLogOnScreen("Character Left State without move!", 10f, StringColor.Red, 100, DebugAreas.Combat);
	}

	public override bool CanLeaveDefensiveState()
	{
		if (defensiveShouldMove) return false;
		return base.CanLeaveDefensiveState();
	}
}