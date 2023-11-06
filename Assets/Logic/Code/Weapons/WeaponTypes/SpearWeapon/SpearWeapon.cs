using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearWeapon : WeaponBase
{
	OldWeaponProjectile defensiveSpear = null;
	List<GameObject> thrownSpears = new List<GameObject>();

	public OldWeaponProjectile DefensiveSpear { get { return defensiveSpear; } set { defensiveSpear = value; } }


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
		return base.GroundDownAttack(attackDeltaTime);
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
		return base.AirDownAttack(attackDeltaTime);
	}
    public override AttackAnimationData AirDirectionAttack(float attackDeltaTime) 
    {
        return base.AirDirectionAttack(attackDeltaTime);
	}

	public override void GroundAttackHit(GameObject hitObj)
	{
		base.GroundAttackHit(hitObj);
	}

	public override void GroundUpAttackHit(GameObject hitObj)
	{
		base.GroundAttackHit(hitObj);
	}

	public override void GroundDownAttackHit(GameObject hitObj)
	{
		base.GroundDownAttackHit(hitObj);
	}

	public override void GroundDirectionAttackHit(GameObject hitObj)
	{
		base.GroundDirectionAttackHit(hitObj);
	}

	public override void AirAttackHit(GameObject hitObj)
	{
		base.AirAttackHit(hitObj);
	}

	public override void AirUpAttackHit(GameObject hitObj)
	{
		base.AirUpAttackHit(hitObj);
	}

	public override void AirDownAttackHit(GameObject hitObj)
	{
		base.AirDownAttackHit(hitObj);
	}

	public override void AirDirectionAttackHit(GameObject hitObj)
	{
		base.AirDirectionAttackHit(hitObj);
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		base.PostAttackStateLogic(deltaTime);
	}

	public override void EndAttackStateLogic()
	{
		// Still needed?
		GameCharacter.MovementComponent.SetLayerToDefault();
		GameCharacter.MovementComponent.ResetCharacterCapsulToDefault();
		SpawnedWeapon.SetActive(true);

		UnHookAllHookedCharacerts();

		base.EndAttackStateLogic();
	}

	public override void AttackPhaseStart()
	{
		base.AttackPhaseEnd();
	}


	public override AttackAnimationData DefensiveAction()
	{
		return base.DefensiveAction();
	}
}