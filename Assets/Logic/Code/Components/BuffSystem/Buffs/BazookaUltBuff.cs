using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaBuffData
{
	public ProjectilePool projectilePool;
	public ParticleSystemPool explosionPartilcePool;
	public WeaponObjData bazookaData;
	public AnimationClip addativeShootAnimation;
	public WeaponBase weapon;
	public int missileAmountToShoot;
	public float projectileSpeed;
	public float projectileDamage;

	public BazookaBuffData(ProjectilePool projectilePool, ParticleSystemPool explosionPartilcePool, WeaponObjData bazookaData, AnimationClip addativeShootAnimation, WeaponBase weapon, int missileAmountToShoot, float projectileSpeed, float projectileDamage)
	{
		this.projectilePool = projectilePool;
		this.explosionPartilcePool = explosionPartilcePool;
		this.bazookaData = bazookaData;
		this.addativeShootAnimation = addativeShootAnimation;
		this.weapon = weapon;
		this.missileAmountToShoot = missileAmountToShoot;
		this.projectileSpeed = projectileSpeed;
		this.projectileDamage = projectileDamage;
	}
}

public class BazookaUltBuff : ABuff
{
	BazookaBuffData bazookaData;
	Ultra.Timer timer;
	float timeBetweenShots;

	public BazookaUltBuff(GameCharacter gameCharacter, float duration, BazookaBuffData data) : base(gameCharacter, duration)
	{
		bazookaData = data;

		timeBetweenShots = duration / bazookaData.missileAmountToShoot;
		
		timer = new Ultra.Timer(timeBetweenShots, false);
		timer.onTimerFinished += OnTimerFinished;
		ShootMissil();
	}

	void ShootMissil()
	{
		GameCharacter.AnimController.ApplyUpperBodyAddativeAnimationToState(bazookaData.addativeShootAnimation);
		GameCharacter.AnimController.InUpperBodyAddativeState = true;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += OnTimerFinished;
		GameCharacter.CombatComponent.AttackTimer.Start(bazookaData.addativeShootAnimation.length);
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Shoot);
		GameCharacter.AnimController.ApplyBlendTree(GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].AimAnimations);
		bazookaData.weapon.PlayAttackSound(0);
		bazookaData.weapon.SpawnWeaponFlash(bazookaData.bazookaData);

		WeaponProjectile projectile = bazookaData.projectilePool.GetValue();
		projectile.transform.position = bazookaData.bazookaData.transform.position;
		projectile.Init(GameCharacter, bazookaData.bazookaData.transform.forward, bazookaData.projectileSpeed, bazookaData.projectileDamage, null, null);
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		timer.Update(deltaTime);
	}

	public override void BuffEnds()
	{
		timer.onTimerFinished -= OnTimerFinished;
		timer.Stop();
	}

	public override EBuff GetBuffType()
	{
		return EBuff.BazookaUlt;
	}

	void OnTimerFinished()
	{
		ShootMissil();
		timer.Start();
	}
}
