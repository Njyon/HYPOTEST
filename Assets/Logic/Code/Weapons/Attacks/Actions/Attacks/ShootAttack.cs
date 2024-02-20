using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShootAttackData : AttackData
{
	public AnimationClip shootAddativeAnimation;
	public WeaponProjectile projectile;
	public float projectileSpeed;
}

public class ShootAttack : AttackBase
{
	public ShootAttackData attackData;
	ProjectilePool projectilePool;
	WeaponBase lastWeapon;
	WeaponObjData weaponObjData;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, () => {
			projectilePool = new ProjectilePool(attackData.projectile, gameCharacter.CreateHolderChild("BulletProjectileHolder"), 5);
		});

		if (lastWeapon != GameCharacter.CombatComponent.CurrentWeapon)
		{
			lastWeapon = GameCharacter.CombatComponent.CurrentWeapon;
			weaponObjData = GameCharacter.CombatComponent.CurrentWeapon.SpawnedWeapon.GetComponent<WeaponObjData>();
		}
	}

	public override void StartAction()
	{
		GameCharacter.AnimController.ApplyAddativeAnimationToAddativeState(attackData.shootAddativeAnimation);
		GameCharacter.AnimController.InAddativeState = true;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += OnTimerFinished;
		GameCharacter.CombatComponent.AttackTimer.Start(attackData.shootAddativeAnimation.length);
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Shoot);
		Weapon.PlayAttackSound(0);

		if (weaponObjData != null)
		{
			var projectile = projectilePool.GetValue();
			projectile.transform.position = weaponObjData.weaponTip.transform.position;
			Vector3 projectileDir = GameCharacter.CombatComponent.AimPositionCheck.Value ? GameCharacter.CombatComponent.AimPositionCheck.Position - weaponObjData.weaponTip.transform.position : GameCharacter.CombatComponent.AimCharacter != null ? GameCharacter.CombatComponent.AimCharacter.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter : GameCharacter.transform.forward;
			projectile.Init(GameCharacter, projectileDir, attackData.projectileSpeed, attackData.Damage, OnProjectileHit, OnProjectileLifeTimeEnd);
		}
	}

	void OnProjectileHit(WeaponProjectile projectile, Collider other)
	{
		IDamage iDamage = other.GetComponent<IDamage>();
		if (iDamage != null)
		{
			DoDamage(projectile, iDamage);
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
				DoDamage(projectile, iDamage);
			}
		}
	}

	void DoDamage(WeaponProjectile projectile, IDamage iDamage)
	{
		if (iDamage == null) return;
		if (iDamage.GetTeam() == GameCharacter.GetTeam()) return;
		iDamage.DoDamage(GameCharacter, attackData.Damage, false, true, false);
		projectilePool.ReturnValue(projectile);
	}

	void OnProjectileLifeTimeEnd(WeaponProjectile projectile)
	{
		projectilePool.ReturnValue(projectile);
	}

	public override void ActionInterupted()
	{
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= OnTimerFinished;
		GameCharacter.CombatComponent.AttackTimer.Stop();
		GameCharacter.AnimController.InAddativeState = false;
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
	}

	void OnTimerFinished()
	{
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= OnTimerFinished;
		GameCharacter.AnimController.InAddativeState = false;
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
	}

	public override float GetActionRanting()
	{
		return attackData.Rating;
	}

	public override float GetActionDischarge()
	{
		return attackData.Discharge;
	}

	public override ActionBase CreateCopy()
	{
		ShootAttack copy = new ShootAttack();
		copy.attackData = attackData;
		return copy;
	}
}
