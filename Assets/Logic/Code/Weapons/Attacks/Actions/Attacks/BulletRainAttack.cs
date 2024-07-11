using Megumin.GameFramework.AI.BehaviorTree.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BulletRainAttackData : AttackData
{
	public AnimationClip attackAnim;
	public float bulletRainDebuffDuration;
	public float bulletRainTimeBetweenWaves;
	public int waveSize;
	public float bulletRainInitailDelay;
	public float bulletToBulletDistance;
	public float hightForBullets;
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
		target.BuffComponent.AddBuff(new BulletRainDebuff(target, attackData.bulletRainDebuffDuration, projectilePool, attackData.bulletRainTimeBetweenWaves, attackData.waveSize, attackData.bulletRainInitailDelay, attackData.bulletToBulletDistance, attackData.hightForBullets));

	}

	public override ActionBase CreateCopy()
	{
		BulletRainAttack copy = new BulletRainAttack();
		copy.attackData = attackData;
		return copy;
	}
}
