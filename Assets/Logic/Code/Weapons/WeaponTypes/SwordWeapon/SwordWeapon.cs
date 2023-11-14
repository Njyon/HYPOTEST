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

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new SwordWeapon(gameCharacter, weapon);
	}
}