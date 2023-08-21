using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCharacterMovementComponent;

public class FistWeapon : WeaponBase
{
	bool landed = false;
	bool startFalling = false;
	Vector3 targetDir = Vector3.zero;
	float speed = 40f;
	public FistWeapon() { }
	public FistWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
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
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) Attack3BlendLogic(EExplicitAttackType.AirDownAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks, EAnimationType.Default);

		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += AttackTimerFinished;
		GameCharacter.MovementComponent.onCharacterGroundedChanged += OnCharacterGroundedChanged;

		landed = false;
		startFalling = false;

		Vector3 maxDir = (GameCharacter.transform.forward + Vector3.down).normalized;
		GameCharacter target = Ultra.HypoUttilies.FindCHaracterNearestToDirectionWithMinAngel(GameCharacter.MovementComponent.CharacterCenter, Vector3.down, 45f, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
		targetDir = maxDir;
		//if (target == null)
		{
			GameCharacter.AnimController.Combat3BlendDir = Vector3.Dot(maxDir, GameCharacter.transform.forward);
		}
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
		if (ComboIndexInSameAttack == 2)
		{
			GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
			if (enemyCharacter == null) return;
			RequestFlyAway(enemyCharacter);
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

	private void RequestFlyAway(GameCharacter enemyCharacter)
	{
		if (enemyCharacter.CombatComponent.CanRequestFlyAway())
		{
			enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.FlyAway);
			if (Mathf.Sign(GameCharacter.transform.forward.x) < 0)
			{
				Vector3 direction = Quaternion.Euler(CurrentAttack.extraData.flyAwayDirection) * GameCharacter.transform.forward;
				direction.y = direction.y * -1;
				enemyCharacter.MovementComponent.MovementVelocity = direction * CurrentAttack.extraData.flyAwayStrengh;
			}
			else
			{
				Vector3 direction = Quaternion.Euler(CurrentAttack.extraData.flyAwayDirection) * GameCharacter.transform.forward;
				enemyCharacter.MovementComponent.MovementVelocity = direction * CurrentAttack.extraData.flyAwayStrengh;
			}
			
			Ultra.Utilities.DrawArrow(enemyCharacter.MovementComponent.CharacterCenter, enemyCharacter.MovementComponent.MovementVelocity, 5f, Color.magenta, 10f, 100, DebugAreas.Combat);
		}
	}

	public override void AirUpAttackHit(GameObject hitObj)
	{
		IDamage damageInterface = GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, 10);
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
			GameCharacter.CombatComponent.HookedCharacter = enemyCharacter;
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

	void UpdateAirDownAttack(float deltaTime)
	{
		if (!landed && startFalling)
		{
			Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
			velocity = targetDir * speed;
			GameCharacter.MovementComponent.MovementVelocity = velocity;
		}
	}
}