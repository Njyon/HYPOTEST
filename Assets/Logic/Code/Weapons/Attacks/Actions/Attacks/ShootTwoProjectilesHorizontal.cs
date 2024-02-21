using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShootTwoProjectilesHorizontalData : AttackData
{
	public AnimationClip shootAnimation;
	public WeaponProjectile projectile;
	public float projectileSpeed = 5f;
	public float projtilceLifeTime = 1f;
}

public class ShootTwoProjectilesHorizontal : AttackBase
{
	public ShootTwoProjectilesHorizontalData attackData;
	ProjectilePool projectilePool;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, () => {
			projectilePool = new ProjectilePool(attackData.projectile, gameCharacter.CreateHolderChild("DoubleProjectileHolder"), 5);
		});
	}

	public override void StartAction()
	{
		StartAttack(attackData.shootAnimation);
	}

	public override void TriggerAnimationEvent()
	{
		var forwardProjectile = projectilePool.GetValue();
		var backwardProjectile = projectilePool.GetValue();
		forwardProjectile.transform.position = GameCharacter.MovementComponent.CharacterCenter;
		backwardProjectile.transform.position = GameCharacter.MovementComponent.CharacterCenter;
		Vector3 projectileDirForward = GameCharacter.transform.forward;
		Vector3 projectileDirBackward = -GameCharacter.transform.forward;
		forwardProjectile.Init(GameCharacter, projectileDirForward, attackData.projectileSpeed, attackData.Damage, null, OnProjectileLifeTimeEnd, attackData.projtilceLifeTime);
		backwardProjectile.Init(GameCharacter, projectileDirBackward, attackData.projectileSpeed, attackData.Damage, null, OnProjectileLifeTimeEnd, attackData.projtilceLifeTime);
	}

	public override void ActionInterupted()
	{

	}

	void OnProjectileLifeTimeEnd(WeaponProjectile projectile)
	{
		projectilePool.ReturnValue(projectile);
	}

	public override ActionBase CreateCopy()
	{
		ShootTwoProjectilesHorizontal copy = new ShootTwoProjectilesHorizontal();
		copy.attackData = attackData;
		return copy;
	}
}
