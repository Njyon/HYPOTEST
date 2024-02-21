using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShootAttackProjectileData : AttackData
{
	public AnimationClip shootAnimation;
	public WeaponProjectile projectile;
	public float projectileSpeed;
}

public class ShootAttackProjectile : AttackBase
{
	public ShootAttackProjectileData attackData;
	ProjectilePool projectilePool;
	WeaponObjData weaponObjData;

	WeaponObjData WeaponObjData
	{
		get
		{
			if (weaponObjData == null)
			{
				weaponObjData = GameCharacter.CombatComponent.CurrentWeapon.SpawnedWeapon.GetComponent<WeaponObjData>();
			}
			return weaponObjData;
		}
	}

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, () => {
			projectilePool = new ProjectilePool(attackData.projectile, gameCharacter.CreateHolderChild("ProjectileHolder"), 5);
		});
	}

	public override void StartAction()
	{
		StartAttack(attackData.shootAnimation);
	}

	public override void TriggerAnimationEvent()
	{
		if (WeaponObjData != null)
		{
			var projectile = projectilePool.GetValue();
			projectile.transform.position = WeaponObjData.weaponTip.transform.position;
			Vector3 projectileDir = GameCharacter.CombatComponent.AimPositionCheck.Value ? GameCharacter.CombatComponent.AimPositionCheck.Position - WeaponObjData.weaponTip.transform.position : GameCharacter.transform.forward;
			projectile.Init(GameCharacter, projectileDir, attackData.projectileSpeed, attackData.Damage, null, OnProjectileLifeTimeEnd);
		}
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
		ShootAttackProjectile copy = new ShootAttackProjectile();
		copy.attackData = attackData;
		return copy;
	}
}
