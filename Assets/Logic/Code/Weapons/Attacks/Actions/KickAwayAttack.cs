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
			Weapon.KickAway(enemyCharacter, attackData.stunTime, attackData.kickDirection, attackData.kickStrengh);
		} else if (attackData.freezBetweenAttacks)
		{
			enemyCharacter.CombatComponent.RequestFreez();
		}
	}

	public override float GetActionRanting()
	{
		return attackData.Rating;
	}

	public override float GetActionDischarge()
	{
		return attackData.Discharge;
	}
}
