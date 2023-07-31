using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearWeapon : WeaponBase
{
	float yVel = 50f;
	bool landed = false;
	bool startFalling = false;
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
        base.AirDownAttack();
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += AttackTimerFinished;
		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;
		landed = false;
		startFalling = false;

	}
    public override void AirDirectionAttack() 
    {
        base.AirDirectionAttack();
		GameCharacter.MovementComponent.MoveThroughCharacterLayer();
		GameCharacter.CharacterHeightTarget = GameCharacter.CharacterHeightTarget / 2;
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
	}

	public override void GroundDownAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);
		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null || enemyCharacter.StateMachine == null || enemyCharacter.CombatComponent == null) return;
		if (enemyCharacter.StateMachine.CanSwitchToStateOrIsState(EGameCharacterState.Freez))
		{
			enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			if (enemyCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Freez)
				enemyCharacter.AddFreezTime();
			else
				enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
			float downforce = -Mathf.Sqrt(2 * enemyCharacter.GameCharacterData.MovmentGravity * 4f);
			if (enemyCharacter.MovementComponent.MovementVelocity.y < 0) 
				downforce -= Mathf.Abs(enemyCharacter.MovementComponent.MovementVelocity.y); // Abs because its easier than Minus - Minus math :D
			enemyCharacter.MovementComponent.MovementVelocity = new Vector3(enemyCharacter.MovementComponent.MovementVelocity.x, downforce, enemyCharacter.MovementComponent.MovementVelocity.z);
		}
	}

	public override void GroundDirectionAttackHit(GameObject hitObj)
	{
		IDamage damageIterface = hitObj.GetComponent<IDamage>();
		if (damageIterface == null) return;
		damageIterface.DoDamage(GameCharacter, 10);
	}

	public override void AirAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);
	}

	public override void AirUpAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);
		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null || enemyCharacter.StateMachine == null || enemyCharacter.CombatComponent == null) return;
		if (enemyCharacter.StateMachine.CanSwitchToStateOrIsState(EGameCharacterState.PullCharacterOnHorizontalLevel))
		{
			enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.PullCharacterOnHorizontalLevel);
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
			enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.HookedToCharacter);
		}
	}

	public override void AirDirectionAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);
	}

	void AttackTimerFinished()
	{
		if (CurrentAttackType == EExplicitAttackType.AirDownAttack)
		{
			GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
			if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
			if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) HoldAttackAfterAttack(CurrentAttackType, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks);
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
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) TriggerAttack(CurrentAttackType, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks);
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
		damageInterface.DoDamage(GameCharacter, 10);
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		base.PostAttackStateLogic(deltaTime);
		if (CurrentAttackType == EExplicitAttackType.AirDownAttack)
		{
			UpdateAirDownAttack(deltaTime);
		}
	}

	public override void EndAttackStateLogic()
	{
		GameCharacter.MovementComponent.SetLayerToDefault();
		GameCharacter.MovementComponent.ResetCharacterCapsulToDefault();
		SpawnedWeapon.SetActive(true);

		foreach (GameObject go in hitObjects)
		{
			if (go == null) continue;
			GameCharacter character = go.GetComponent<GameCharacter>();
			if (character == null) continue;
			character.CombatComponent.HookedToCharacter = null;
		}

		base.EndAttackStateLogic();
	}

	void UpdateAirDownAttack(float deltaTime)
	{
		if (!landed && startFalling)
		{
			Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
			velocity = new Vector3(velocity.x, velocity.y - yVel, velocity.z);
			GameCharacter.MovementComponent.MovementVelocity = velocity;
		}
	}

	public override void AttackPhaseStart()
	{
		base.AttackPhaseEnd();
		if (CurrentAttackType == EExplicitAttackType.GroundedDirectionalAttack)
			ThrowSpear();
		startFalling = true;
	}

	void ThrowSpear()
	{
		SpawnedWeapon.SetActive(false);
		GameObject throwSpear = GameObject.Instantiate(GameAssets.Instance.ThrowSpear);
		throwSpear.transform.position =  new Vector3(SpawnedWeaponBones.transform.position.x, SpawnedWeaponBones.transform.position.y, 0);
		throwSpear.transform.rotation = Quaternion.LookRotation(GameCharacter.transform.forward.normalized, Vector3.up);
		throwSpear.transform.eulerAngles = new Vector3(throwSpear.transform.eulerAngles.x, throwSpear.transform.eulerAngles.y, 90f);
		WeaponProjectile weaponProjectile = throwSpear.GetComponent<WeaponProjectile>();
		weaponProjectile.onProjectileHit += GroundDirectionAttackHit;
		weaponProjectile.Initialize(GameCharacter, GameCharacter.transform.forward);

		thrownSpears.Add(throwSpear);
	}

	public override void DefensiveAction()
	{
		base.DefensiveAction();

		SpawnedWeapon.SetActive(false);
		GameObject throwSpear = GameObject.Instantiate(GameAssets.Instance.ThrowSpear);
		throwSpear.transform.position = new Vector3(SpawnedWeaponBones.transform.position.x, SpawnedWeaponBones.transform.position.y, 0);
		throwSpear.transform.rotation = Quaternion.LookRotation(GameCharacter.transform.forward.normalized, Vector3.up);
		throwSpear.transform.eulerAngles = new Vector3(throwSpear.transform.eulerAngles.x, throwSpear.transform.eulerAngles.y, 90f);
		WeaponProjectile weaponProjectile = throwSpear.GetComponent<WeaponProjectile>();
		weaponProjectile.onProjectileHit += GroundDirectionAttackHit;
		weaponProjectile.Initialize(GameCharacter, GameCharacter.transform.forward);
	}
}