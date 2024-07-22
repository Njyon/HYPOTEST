using Megumin.GameFramework.AI.BehaviorTree.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BulletRainAttackData : AttackData
{
	public AnimationClip attackAnim;
	public float bulletRainDebuffDuration = 5f;
	public float bulletRainTimeBetweenWaves = 1f;
	public int waveSize = 20;
	public float bulletRainInitailDelay = 0.5f;
	public float bulletToBulletDistance = 0.5f;
	public float hightForBullets = 10;
	public float bulletSpeed = 5f;
	public WeaponProjectile weaponProjectile;
}

public class BulletRainAttack : AttackBase
{
	public BulletRainAttackData attackData;
	ProjectilePool projectilePool;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, () => {
			projectilePool = new ProjectilePool(attackData.weaponProjectile, GameCharacter.CreateHolderChild("BulletRainProjectilePool"), attackData.waveSize);
		});
	}

	public override void StartAction()
	{
		StartAttack(attackData.attackAnim);
	}

	public override void TriggerAnimationEvent()
	{
		GameCharacter target = GameCharacter.CharacterDetection.TargetGameCharacters[0];
		target.BuffComponent.AddBuff(new BulletRainDebuff(target, attackData.bulletRainDebuffDuration, projectilePool, attackData.bulletRainTimeBetweenWaves, attackData.waveSize, attackData.bulletRainInitailDelay, attackData.bulletToBulletDistance, attackData.hightForBullets, attackData.bulletSpeed, attackData.Damage, OnProjectileHit, OnProjectileLifeTimeEnd));

	}

	void OnProjectileHit(WeaponProjectile projectile, Collider other)
	{
		IDamage iDamage = other.GetComponent<IDamage>();
		if (iDamage != null)
		{
			DoDamage(other.gameObject, attackData.Damage);
		}
		else
		{
			var parent = other.transform;
			while (parent.parent != null)
			{
				parent = parent.parent;
			}
			iDamage = parent.GetComponent<IDamage>();
			if (parent != null)
			{
				DoDamage(other.gameObject, attackData.Damage);
			}
		}
		if (iDamage.CanBeDamaged())
			projectilePool.ReturnValue(projectile);
	}

	void OnProjectileLifeTimeEnd(WeaponProjectile projectile)
	{
		projectilePool.ReturnValue(projectile);
	}

	public override ActionBase CreateCopy()
	{
		BulletRainAttack copy = new BulletRainAttack();
		copy.attackData = attackData;
		return copy;
	}
}
