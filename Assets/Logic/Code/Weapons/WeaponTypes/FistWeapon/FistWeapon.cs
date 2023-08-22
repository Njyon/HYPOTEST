using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameCharacterMovementComponent;

public class FistWeapon : WeaponBase
{
	bool landed = false;
	bool startFalling = false;
	Vector3 targetDir = Vector3.zero;
	float speed = 40f;
	float interpSpeed = 10f;
	float targetAngel;
	Ultra.Timer backupFallTimer;
	Vector3 defensiveMoveVector;
	bool defensiveShouldMove = false;

	public FistWeapon() { }
	public FistWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{
		backupFallTimer = new Ultra.Timer(0.4f, true);
		backupFallTimer.onTimerFinished += OnBackupTimerFinished;
	}

    public override void EquipWeapon()
    {
        base.EquipWeapon();
    }

    public override void UnEquipWeapon()
    {
        base.UnEquipWeapon();
	}

    public override void UpdateWeapon(float deltaTime)
    {
        base.UpdateWeapon(deltaTime);
    }

    public override void GroundAttack()   
    {
        base.GroundAttack();
    }
    public override void GroundUpAttack()    
    {
        base.GroundUpAttack();
    }
    public override void GroundDownAttack()  
    {
        base.GroundDownAttack();
    }
    public override void GroundDirectionAttack()   
    {
        base.GroundDirectionAttack();
    }

    public override void AirAttack()  
    {
        base.AirAttack();
    }
    public override void AirUpAttack()
    {
        base.AirUpAttack();
    }
    public override void AirDownAttack()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) Attack3BlendLogic(EExplicitAttackType.AirDownAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks, EAnimationType.Default);

		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += AttackTimerFinished;
		GameCharacter.MovementComponent.onCharacterGroundedChanged += OnCharacterGroundedChanged;

		landed = false;
		startFalling = false;

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
		}
		targetAngel = Vector3.Angle(targetDir, GameCharacter.transform.forward);
	}
    public override void AirDirectionAttack() 
    {
        base.AirDirectionAttack();
    }

	public override void AttackPhaseStart()
	{
		base.AttackPhaseStart();
		startFalling = true;
	}

	public override void GroundAttackHit(GameObject hitObj)
	{
        IDamage damageInterface = GetDamageInterface(hitObj);
        if (damageInterface == null) return;
        damageInterface.DoDamage(GameCharacter, 10);
	}

	public override void GroundUpAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);
		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null) return;
		if (enemyCharacter.CombatComponent != null) enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
		HookCharacterToCharacter(enemyCharacter);
		if (enemyCharacter.StateMachine != null) enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.HookedToCharacter);
	}

	

	public override void GroundDownAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);

	}

	public override void GroundDirectionAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);

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
			RequestFreez(enemyCharacter);
		}
	}

	public override void AirAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);

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
		damageInterface.DoDamage(GameCharacter, 10);

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
		if (enemyCharacter.StateMachine.CanSwitchToStateOrIsState(EGameCharacterState.HookedToCharacter))
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
		damageInterface.DoDamage(GameCharacter, 10);

		HeavyAttackLogic(hitObj);
	}

	public override void EndAttackStateLogic()
	{
		foreach (GameObject go in hitObjects)
		{
			if (go == null) continue;
			GameCharacter character = go.GetComponent<GameCharacter>();
			if (character == null) continue;
			character.CombatComponent.HookedToCharacter = null;
		}

		base.EndAttackStateLogic();
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

		CameraController.Instance.ShakeCamerea(2);

		UnHookAllHookedCharacerts();

		foreach (GameObject obj in hitObjects)
		{
			OnGroundAttackHit(obj);
		}
	}

	void OnGroundAttackHit(GameObject hitObject)
	{
		IDamage damageInterface = GetDamageInterface(hitObject);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);

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
				float deltaTimeScale = 1f / Time.deltaTime;
				defensiveMoveVector *= deltaTimeScale;
				GameCharacter.MovementComponent.MovementVelocity = defensiveMoveVector;
				GameCharacter.MovementComponent.DeactiveStepup();
				Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, defensiveMoveVector.normalized, defensiveMoveVector.magnitude, Color.white, 10f, 100, DebugAreas.Combat);
				defensiveShouldMove = false;
				DidMove();
			}
		}
	}

	void UpdateAirDownAttack(float deltaTime)
	{
		if (!landed && startFalling)
		{
			Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
			velocity = targetDir * speed;
			GameCharacter.MovementComponent.MovementVelocity = velocity;
		}
	}

	void OnBackupTimerFinished()
	{
		startFalling = true;
	}

	public override void DefensiveActionStart()
	{
		GameCharacter targetCharacter = Ultra.HypoUttilies.FindCharactereNearestToDirectionWithRange(GameCharacter.MovementComponent.CharacterCenter, GameCharacter.MovementInput.normalized.magnitude > 0 ? GameCharacter.MovementInput.normalized : GameCharacter.transform.forward, CurrentDefensiveAction.extraData.rangeValue, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
		Vector3 targetPosition = Vector3.zero;
		if (targetCharacter != null)
		{
			Vector3 dir =  GameCharacter.MovementComponent.CharacterCenter - targetCharacter.MovementComponent.CharacterCenter;
			dir = new Vector3(dir.x, 0f, 0f).normalized;
			float lenght = targetCharacter.MovementComponent.CapsuleCollider.radius + GameCharacter.MovementComponent.CapsuleCollider.radius;
			targetPosition = targetCharacter.MovementComponent.CharacterCenter + dir * lenght;
			Ultra.Utilities.DrawWireSphere(targetPosition, .5f, Color.red, 10f, 100, DebugAreas.Combat);
		}
		else
		{
			targetPosition = GameCharacter.MovementComponent.CharacterCenter + (GameCharacter.MovementInput.normalized.magnitude > 0 ? new Vector3(GameCharacter.MovementInput.x, 0f, 0f) : GameCharacter.transform.forward)* (CurrentDefensiveAction.extraData.rangeValue / 2);
		}
		defensiveMoveVector = targetPosition - GameCharacter.MovementComponent.CharacterCenter;
		defensiveShouldMove = true;

		SpawnedWeapon?.SetActive(false);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = false;
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
	}
}