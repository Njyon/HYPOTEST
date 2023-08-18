using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistWeapon : WeaponBase
{
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
        base.AirDownAttack();
    }
    public override void AirDirectionAttack() 
    {
        base.AirDirectionAttack();
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
		damageInterface.DoDamage(GameCharacter, 10);
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
}