using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearWeapon : WeaponBase
{
	bool landed = false;
	bool startFalling = false;
	bool groundUpAttackhit = false;
	bool groundUpAttackMove = false;
	Ultra.Timer groundUpTimer = new Ultra.Timer();
	WeaponProjectile defensiveSpear = null;
	List<GameObject> thrownSpears = new List<GameObject>();

    public SpearWeapon() { }
	public SpearWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

    public override void EquipWeapon()
    {
        base.EquipWeapon();
	}

    public override void UnEquipWeapon()
    {
        base.UnEquipWeapon();

		GameCharacter.AnimController.InAimBlendTree = false;
		UnHookAllHookedCharacerts();
		GameCharacter.RequestBestCharacterState();
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
		SpawnedWeapon?.SetActive(true);

		landed = false;
		startFalling = false;
		groundUpAttackhit = false;
		groundUpAttackMove = false;

		if (defensiveSpear != null)
			GameObject.Destroy(defensiveSpear.gameObject);
	}

    public override void UpdateWeapon(float deltaTime)
    {
        base.UpdateWeapon(deltaTime);
    }

    public override AttackAnimationData GroundAttack(float attackDeltaTime)   
    {
      	return base.GroundAttack(attackDeltaTime);
    }
    public override AttackAnimationData GroundUpAttack(float attackDeltaTime)    
    {
		return base.GroundUpAttack(attackDeltaTime);
	}
    public override AttackAnimationData GroundDownAttack(float attackDeltaTime)  
    {
		AttackAnimationData returnData = null;

		returnData = base.GroundDownAttack(attackDeltaTime);
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += AttackTimerFinished;
		groundUpAttackhit = false;
		groundUpAttackMove = false;

		return returnData;
	}
    public override AttackAnimationData GroundDirectionAttack(float attackDeltaTime)   
    {
        return base.GroundDirectionAttack(attackDeltaTime);
    }

    public override AttackAnimationData AirAttack(float attackDeltaTime)  
    {
        return base.AirAttack(attackDeltaTime);
    }
    public override AttackAnimationData AirUpAttack(float attackDeltaTime)
    {
        return base.AirUpAttack(attackDeltaTime);
    }
    public override AttackAnimationData AirDownAttack(float attackDeltaTime)  
    {
		AttackAnimationData returnData = null;

		returnData = base.AirDownAttack(attackDeltaTime);
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += AttackTimerFinished;
		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;
		landed = false;
		startFalling = false;

		return returnData;
	}
    public override AttackAnimationData AirDirectionAttack(float attackDeltaTime) 
    {
        return base.AirDirectionAttack(attackDeltaTime);
		//GameCharacter.MovementComponent.MoveThroughCharacterLayer();
		//GameCharacter.CharacterHeightTarget = GameCharacter.CharacterHeightTarget / 2;
	}

	public override void GroundAttackHit(GameObject hitObj)
	{
		base.GroundAttackHit(hitObj);
		return;

		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage(1));
	}

	public override void GroundUpAttackHit(GameObject hitObj)
	{
		base.GroundAttackHit(hitObj);
		return;

		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage(1));

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
		base.GroundDownAttackHit(hitObj);
		return;

		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage(1));

		if (!groundUpAttackhit)
		{
			GroundDownAttackEnd();
		}
	}

	public override void GroundDirectionAttackHit(GameObject hitObj)
	{
		base.GroundDirectionAttackHit(hitObj);
		return;

		IDamage damageIterface = hitObj.GetComponent<IDamage>();
		if (damageIterface == null) return;
		damageIterface.DoDamage(GameCharacter, GetDamage(1));

		if (AttackIndex == 2)
		{
			GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
			if (enemyCharacter == null) return;
			//RequestFlyAway(enemyCharacter);
		}
	}

	public override void AirAttackHit(GameObject hitObj)
	{
		base.AirAttackHit(hitObj);
		return;

		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage(1));

		if (AttackIndex == 2)
		{
			GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
			if (enemyCharacter == null) return;
			//RequestFlyAway(enemyCharacter);
		}
	}

	public override void AirUpAttackHit(GameObject hitObj)
	{
		base.AirUpAttackHit(hitObj);
		return;

		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage(1));
		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null || enemyCharacter.StateMachine == null || enemyCharacter.CombatComponent == null) return;
		if (enemyCharacter.StateMachine.CanSwitchToStateOrIsState(EGameCharacterState.PullCharacterOnHorizontalLevel))
		{
			enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			HookCharacterToCharacter(enemyCharacter);
			enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.PullCharacterOnHorizontalLevel);
		}
	}

	public override void AirDownAttackHit(GameObject hitObj)
	{
		base.AirDownAttackHit(hitObj);
		return;

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
		base.AirDirectionAttackHit(hitObj);
		return;

		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage(1));

		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null) return;
		//RequestFlyAway(enemyCharacter);
	}

	void AttackTimerFinished()
	{
		if (CurrentAttackType == EExplicitAttackType.AirDownAttack)
		{
			GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
			if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
			if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) HoldAttackAfterAttack(CurrentAttackType, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks, -1);
		}
		else if (CurrentAttackType == EExplicitAttackType.GroundedDownAttack)
		{
			GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
			
			if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
			if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) HoldAttackAfterAttack(CurrentAttackType, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDownAttacks, -1);
			GameCharacter.HitDetectionEventStart(new AnimationEvent());
			groundUpAttackMove = true;
			groundUpTimer.Start(CurrentAction.extraData.timeValue);
			groundUpTimer.onTimerFinished += OnGroundDownTimerFinished;
		}
	}

	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		
		if ((collisionFlag & CollisionFlags.Below) != 0)
		{
			if (CurrentAttackType == EExplicitAttackType.AirDownAttack)
			{
				if (!landed) OnAirDownHitLanding();
			}
		}
	}

	void OnAirDownHitLanding()
	{
		landed = true;
		GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;

		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) TriggerAttack(CurrentAttackType, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks, -1);
		GameCharacter.AnimController.HoldAttack = false;
		GameCharacter.AnimController.InAttack = false;

		foreach (GameObject obj in hitObjects)
		{
			OnGroundAttackHit(obj);
		}
	}

	void OnGroundAttackHit(GameObject hitObject)
	{
		IDamage damageInterface = GetDamageInterface(hitObject);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, GetDamage(1));
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		base.PostAttackStateLogic(deltaTime);
		if (CurrentAttackType == EExplicitAttackType.AirDownAttack)
		{
			UpdateAirDownAttack(deltaTime);
		}
		else if (CurrentAttackType == EExplicitAttackType.GroundedDownAttack)
		{
			UpdateGroundDownAttack(deltaTime);
		}
	}

	public override void EndAttackStateLogic()
	{
		GameCharacter.MovementComponent.SetLayerToDefault();
		GameCharacter.MovementComponent.ResetCharacterCapsulToDefault();
		SpawnedWeapon.SetActive(true);

		UnHookAllHookedCharacerts();

		base.EndAttackStateLogic();
	}


	void UpdateAirDownAttack(float deltaTime)
	{
		if (!landed && startFalling)
		{
			Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
			velocity = new Vector3(velocity.x, velocity.y - CurrentAction.extraData.speedValue, velocity.z);
			GameCharacter.MovementComponent.MovementVelocity = velocity;
		}
	}

	void UpdateGroundDownAttack(float deltaTime)
	{
		if (groundUpAttackMove)
		{
			groundUpTimer.Update(deltaTime);
			Vector3 velocity = GameCharacter.transform.forward.normalized * CurrentAction.extraData.speedValue;
			GameCharacter.MovementComponent.MovementVelocity = velocity;
		}
	}

	public override void AttackPhaseStart()
	{
		base.AttackPhaseEnd();

		//if (CurrentAttackType == EExplicitAttackType.GroundedDirectionalAttack)
		//	ThrowSpear();

		if (CurrentAttackType == EExplicitAttackType.AirDownAttack)
			startFalling = true;
	}

	//void ThrowSpear()
	//{
	//	SpawnedWeapon.SetActive(false);
	//	GameObject throwSpear = GameObject.Instantiate(GameAssets.Instance.ThrowSpear);
	//	throwSpear.transform.position =  new Vector3(SpawnedWeaponBones.transform.position.x, SpawnedWeaponBones.transform.position.y, 0);
	//	throwSpear.transform.rotation = Quaternion.LookRotation(GameCharacter.transform.forward.normalized, Vector3.up);
	//	throwSpear.transform.eulerAngles = new Vector3(throwSpear.transform.eulerAngles.x, throwSpear.transform.eulerAngles.y, 90f);
	//	WeaponProjectile weaponProjectile = throwSpear.GetComponent<WeaponProjectile>();
	//	weaponProjectile.onProjectileHit += GroundDirectionAttackHit;
	//	weaponProjectile.Initialize(GameCharacter, GameCharacter.transform.forward);
	//
	//	thrownSpears.Add(throwSpear);
	//}

	public override AttackAnimationData DefensiveAction()
	{
		AttackAnimationData returnData = null;
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction.Count > 0) returnData = DefensiveActionAimLogic(ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction, EAnimationType.Default);


		GameCharacter targetEnemy = Ultra.HypoUttilies.FindCharactereNearestToDirection(GameCharacter.MovementComponent.CharacterCenter, (GameCharacter.MovementInput.magnitude <= 0) ? GameCharacter.transform.forward : GameCharacter.MovementInput, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
		if (targetEnemy == null) return null;


		SpawnedWeapon.SetActive(false);
		GameObject throwSpear = GameObject.Instantiate(GameAssets.Instance.ThrowSpear);
		Vector3 spearDir = targetEnemy.MovementComponent.CharacterCenter - throwSpear.transform.position;
		throwSpear.transform.position = new Vector3(SpawnedWeaponBones.transform.position.x, SpawnedWeaponBones.transform.position.y, 0);
		throwSpear.transform.rotation = Quaternion.LookRotation(spearDir.normalized, Vector3.up);
		throwSpear.transform.eulerAngles = new Vector3(throwSpear.transform.eulerAngles.x, throwSpear.transform.eulerAngles.y, 90f);

		GameCharacter.CombatComponent.AimCharacter = targetEnemy;
		HookCharacterToCharacter(targetEnemy);
		GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;


		defensiveSpear = throwSpear.GetComponent<WeaponProjectile>();
		defensiveSpear.onProjectileHit += DefensiveActionHit;
		defensiveSpear.Initialize(GameCharacter, throwSpear.transform.position, targetEnemy);

		return returnData;
	}

	void DefensiveActionHit(GameObject hitObj)
	{
		GameCharacter hitgameCharacter = hitObj.GetComponent<GameCharacter>();
		if (hitgameCharacter == null)
		{
			IDamage damageInterface = hitObj.GetComponent<IDamage>();
			if (damageInterface != null) damageInterface.DoDamage(GameCharacter, 0);
			return;
		}

		GameCharacter.CombatComponent.AimCharacter = hitgameCharacter;
		HookCharacterToCharacter(hitgameCharacter);
		GameCharacter.CombatComponent.HookedCharacters.Add(hitgameCharacter);
		hitgameCharacter.CombatComponent.HookedToCharacter = GameCharacter;
		hitgameCharacter.CombatComponent.MoveToPosition = GameCharacter.transform.position + GameCharacter.transform.forward * 1f;

		hitgameCharacter.StateMachine.RequestStateChange(EGameCharacterState.MoveToPosition);
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction.Count > 0) DefensiveActionAimLogic(ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction, EAnimationType.Trigger, false);
		
	}

	public override void CharacterArrivedAtRequestedLocation(GameCharacter movedCharacter)
	{
		AfterDefensiveActionCleanUp();
	}

	public override void CharacterMoveToAbort(GameCharacter movedCharacter)
	{

		AfterDefensiveActionCleanUp();
	}

	public override void CharacterMoveToEnd(GameCharacter movedCharacter)
	{
		AfterDefensiveActionCleanUp();
	}

	public override void DefensiveActionEnd()
	{
		base.DefensiveActionEnd();
		AfterDefensiveActionCleanUp();
	}

	private void AfterDefensiveActionCleanUp()
	{
		GameCharacter.AnimController.InAimBlendTree = false;
		UnHookAllHookedCharacerts();
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
		GameCharacter.RequestBestCharacterState();
		SpawnedWeapon.SetActive(true);
		if (defensiveSpear != null)
			GameObject.Destroy(defensiveSpear.gameObject);
	}

	void OnGroundDownTimerFinished()
	{
		groundUpTimer.onTimerFinished -= OnGroundDownTimerFinished;
		if (!groundUpAttackhit)
		{
			GroundDownAttackEnd();
		}
	}

	void GroundDownAttackEnd()
	{
		groundUpAttackhit = true;
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) TriggerAttack(CurrentAttackType, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDownAttacks, -1);
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.AnimController.HoldAttack = false;
		groundUpAttackMove = false;
	}
}