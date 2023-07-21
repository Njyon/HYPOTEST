using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearWeapon : WeaponBase
{
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
		if (enemyCharacter.StateMachine != null && (enemyCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.InAir || enemyCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Freez)) {
			enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
			float downforce = -Mathf.Sqrt(2 * enemyCharacter.GameCharacterData.MovmentGravity * 4f);
			enemyCharacter.MovementComponent.MovementVelocity = new Vector3(enemyCharacter.MovementComponent.MovementVelocity.x, downforce, enemyCharacter.MovementComponent.MovementVelocity.z);
		}
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
		if (enemyCharacter == null) return;
		if (enemyCharacter.CombatComponent != null) enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
		if (enemyCharacter.StateMachine != null) enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.PullCharacterOnHorizontalLevel);
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
	}

	public override void AttackEnd()
	{
		foreach (GameObject go in hitObjects)
		{
			if (go == null) continue;
			GameCharacter character = go.GetComponent<GameCharacter>();
			if (character == null) continue;
			character.CombatComponent.HookedToCharacter = null;
		}
	}
}