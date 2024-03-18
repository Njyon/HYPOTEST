using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BazookaUltimateData : AttackData
{
	public AnimationClip addativeShootAnimation;
	public int minimumMissileAmount = 5;
	public float ultiDuration = 2f;
	public float forceAimDuration = 2f;
	public float projectileSpeed = 5;
	public float projectileLifeTime = 10f;
	public WeaponProjectile projectileBase;
	public int cameraShakeIndex = 1;

}

public class BazookaUltimate : AttackBase
{
	public BazookaUltimateData attackData;
	WeaponObjData weaponObjData;
	ProjectilePool projectilePool;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, () => {
			projectilePool = new ProjectilePool(attackData.projectileBase, GameCharacter.CreateHolderChild("BazookaProjectileHolder"));
		});

		weaponObjData = GameCharacter.CombatComponent.CurrentWeapon.SpawnedWeapon.GetComponent<WeaponObjData>();
	}

	public override void StartAction()
	{
		GameCharacter target = Ultra.HypoUttilies.FindCharactereNearestToDirection(GameCharacter.MovementComponent.CharacterCenter, GameCharacter.MovementInput.magnitude > 0 ? GameCharacter.MovementInput : GameCharacter.transform.forward, ref GameCharacter.CharacterDetection.TargetGameCharacters);
		
		if (target != null) 
			GameCharacter.CombatComponent.AimCharacter = target;

		Weapon.AddForceAimBuff(attackData.forceAimDuration);

		int missileAmount = Mathf.Max(GameCharacter.CharacterDetection.TargetGameCharacters.Count, attackData.minimumMissileAmount);
		BazookaBuffData bazookaBuffData = new BazookaBuffData(projectilePool, weaponObjData, attackData.addativeShootAnimation, Weapon, target, missileAmount, attackData.projectileSpeed, attackData.Damage, attackData.projectileLifeTime, attackData.cameraShakeIndex);
		GameCharacter.BuffComponent.AddBuff(new BazookaUltBuff(GameCharacter, attackData.ultiDuration, bazookaBuffData));
	}

	public override void ActionInterupted()
	{
		base.ActionInterupted();

	}

	public override ActionBase CreateCopy()
	{
		BazookaUltimate copy = new BazookaUltimate();
		copy.attackData = attackData;
		return copy;
	}
}
