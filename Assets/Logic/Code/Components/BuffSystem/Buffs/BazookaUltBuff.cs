using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BazookaBuffData
{
	public ProjectilePool projectilePool;
	public WeaponObjData bazookaData;
	public AnimationClip addativeShootAnimation;
	public GameCharacter mainTarget;
	public WeaponBase weapon;
	public int missileAmountToShoot;
	public float projectileSpeed;
	public float projectileDamage;
	public float projectileLifeTime;
	public int cameraShakeIndex;

	public BazookaBuffData(ProjectilePool projectilePool, WeaponObjData bazookaData, AnimationClip addativeShootAnimation, WeaponBase weapon, GameCharacter mainTarget, int missileAmountToShoot, float projectileSpeed, float projectileDamage, float projectileLifeTime, int cameraShakeIndex)
	{
		this.projectilePool = projectilePool;
		this.bazookaData = bazookaData;
		this.addativeShootAnimation = addativeShootAnimation;
		this.weapon = weapon;
		this.missileAmountToShoot = missileAmountToShoot;
		this.projectileSpeed = projectileSpeed;
		this.projectileDamage = projectileDamage;
		this.mainTarget = mainTarget;
		this.projectileLifeTime = projectileLifeTime;
		this.cameraShakeIndex = cameraShakeIndex;
}
}

public class BazookaUltBuff : ABuff
{
	BazookaBuffData bazookaData;
	Ultra.Timer timer;
	float timeBetweenShots;
	int missileIndex;

	public BazookaUltBuff(GameCharacter gameCharacter, float duration, BazookaBuffData data) : base(gameCharacter, duration)
	{
		bazookaData = data;

		timeBetweenShots = duration / bazookaData.missileAmountToShoot;

		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += OnAttackTimerFinished;
		timer = new Ultra.Timer(timeBetweenShots, false);
		timer.onTimerFinished += OnTimerFinished;
		missileIndex = 0;
		ShootMissil();
	}

	void ShootMissil()
	{
		GameCharacter.AnimController.ApplyUpperBodyAddativeAnimationToState(bazookaData.addativeShootAnimation);
		GameCharacter.AnimController.InUpperBodyAddativeState = true;
		GameCharacter.CombatComponent.AttackTimer.Start(bazookaData.addativeShootAnimation.length);
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Shoot);
		GameCharacter.AnimController.ApplyBlendTree(GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].AimAnimations);
		bazookaData.weapon.PlayDefaultAttackSound(0);
		bazookaData.weapon.SpawnWeaponFlash(bazookaData.bazookaData);

		CameraController.Instance.ShakeCamerea(bazookaData.cameraShakeIndex);

		if (GameCharacter.CharacterDetection.TargetGameCharacters.Count > 0)
		{
			WeaponProjectile projectile = bazookaData.projectilePool.GetValue();
			if (projectile == null) projectile = bazookaData.projectilePool.GetValue();
			projectile.transform.position = bazookaData.bazookaData.weaponTip.transform.position;
			projectile.transform.rotation = Quaternion.LookRotation(-(bazookaData.mainTarget.MovementComponent.CharacterCenter - projectile.transform.position).normalized);
			projectile.Init(GameCharacter, bazookaData.bazookaData.transform.forward, bazookaData.projectileSpeed, bazookaData.projectileDamage, null, OnProjectileLifeTimeEnd, bazookaData.projectileLifeTime);
			BazookaMissileProjectile bazookaMissle = (BazookaMissileProjectile)projectile;
			GameCharacter target = GameCharacter.CharacterDetection.TargetGameCharacters[missileIndex % GameCharacter.CharacterDetection.TargetGameCharacters.Count];
			bazookaMissle.SetTarget(target);
		}
	
		missileIndex++;
	}

	void OnProjectileLifeTimeEnd(WeaponProjectile projectile)
	{
		bazookaData.projectilePool.ReturnValue(projectile);
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		timer.Update(deltaTime);
	}

	public override void BuffEnds()
	{
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= OnAttackTimerFinished;
		timer.onTimerFinished -= OnTimerFinished;
		timer.Stop();
	}

	public override EBuff GetBuffType()
	{
		return EBuff.BazookaUlt;
	}

	void OnAttackTimerFinished()
	{
		GameCharacter.AnimController.InUpperBodyAddativeState = false;
	}

	void OnTimerFinished()
	{
		ShootMissil();
		timer.Start();
	}
}
