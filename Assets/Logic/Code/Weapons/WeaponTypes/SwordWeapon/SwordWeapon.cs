using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : WeaponBase
{
    public SwordWeapon() { }
	public SwordWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
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
		return base.AirDownAttack();
	}
	public override AttackAnimationData AirDirectionAttack()
	{
		return base.AirDirectionAttack();
	}
}