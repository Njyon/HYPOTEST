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

}

public class BazookaUltimate : AttackBase
{
	public BazookaUltimateData attackData;
	WeaponObjData weaponObjData;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, action);

		weaponObjData = GameCharacter.CombatComponent.CurrentWeapon.SpawnedWeapon.GetComponent<WeaponObjData>();
	}

	public override void StartAction()
	{
		GameCharacter target = Ultra.HypoUttilies.FindCharactereNearestToDirection(GameCharacter.MovementComponent.CharacterCenter, GameCharacter.MovementInput.magnitude > 0 ? GameCharacter.MovementInput : GameCharacter.transform.forward, ref GameCharacter.CharacterDetection.DetectedGameCharacters);
		GameCharacter.CombatComponent.AimCharacter = target;

		Weapon.AddForceAimBuff(attackData.forceAimDuration);

		int missileAmount = Mathf.Max(GameCharacter.CharacterDetection.DetectedGameCharacters.Count, attackData.minimumMissileAmount);
		BazookaBuffData bazookaBuffData = new BazookaBuffData(null, null, weaponObjData, attackData.addativeShootAnimation, Weapon, missileAmount, attackData.projectileSpeed, attackData.Damage);
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
