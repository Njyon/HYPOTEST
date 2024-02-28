using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KickAwayAttackData : DefaultAttackData
{
	public int attackTriggerAmount = 0;
	public bool freezBetweenAttacks = false;
	public Vector3 kickDirection;
	public float stunTime = 1f;
	public float kickStrengh = 1f;
}

public class KickAwayAttack : AttackBase
{
	public KickAwayAttackData attackData;
	// Needed for shotgun heavy Attack
	WeaponObjData weaponObjData;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, action);

		switch (Weapon.WeaponData.WeaponType)
		{
			case EWeaponType.Ranged:
				weaponObjData = GameCharacter.CombatComponent.CurrentWeapon.SpawnedWeapon.GetComponent<WeaponObjData>();
				break;
			default: break;
		}
	}

	public override void StartAction()
	{
		StartAttack(attackData.attackAnimation);
	}
	public override void OnHit(GameObject hitObj)
	{
		base.OnHit(hitObj);

		DoDamage(hitObj, attackData.Damage);

		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null) return;
		if (Weapon.ComboIndexInSameAttack == attackData.attackTriggerAmount)
		{
			GameCharacter.CombatComponent.KickAway(enemyCharacter, attackData.stunTime, attackData.kickDirection, attackData.kickStrengh);
		} else if (attackData.freezBetweenAttacks)
		{
			enemyCharacter.CombatComponent.RequestFreez();
		}
		switch(Weapon.WeaponData.WeaponType)
		{
			case EWeaponType.Ranged:
				Weapon.SpawnDamageHitEffect(enemyCharacter);
				break;
			default: break;
		}
	}

	public override void TriggerAnimationEvent()
	{
		Weapon.SpawnWeaponFlash(weaponObjData, Weapon.SpawnRangedAttackShootPartilceAttached());
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
		KickAwayAttack copy = new KickAwayAttack();
		copy.attackData = attackData;
		return copy;
	}
}
