using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FistWeapon : WeaponBase
{
	public FistWeapon() { }
	public FistWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{
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
		UnHookAllHookedCharacerts();
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

	public override AttackAnimationData DefensiveAction()
	{
		return base.DefensiveAction();
	}

	public override void AttackPhaseStart()
	{
		base.AttackPhaseStart();
	}

	public override void GroundAttackHit(GameObject hitObj)
	{
		base.GroundAttackHit(hitObj);
	}

	public override void GroundUpAttackHit(GameObject hitObj)
	{
		base.GroundUpAttackHit(hitObj);
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

	public override void EndAttackStateLogic()
	{
		UnHookAllHookedCharacerts();
		GameCharacter.MovementComponent.SetLayerToDefault();

		base.EndAttackStateLogic();
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		base.PostAttackStateLogic(deltaTime);
	}

	public override bool CanLeaveDefensiveState()
	{
		if (CurrentAction != null && CurrentAction.Action != null)
			if (!CurrentAction.Action.CanLeaveDefensiveState()) return false;
		return base.CanLeaveDefensiveState();
	}

	
}