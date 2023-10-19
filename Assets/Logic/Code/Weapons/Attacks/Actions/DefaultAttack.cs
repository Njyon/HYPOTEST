using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DefaultAttackData : AttackData
{
	[SerializeField]
	public AnimationClip attackAnimation;
}

[Serializable]
public class DefaultAttack : AttackBase
{
	public DefaultAttackData attackData;

	public DefaultAttack()
	{

	}

	public override void StartAction() 
	{
		StartAttack(attackData.attackAnimation);
	}

	public override void OnHit(GameObject hitObj) 
	{
		IDamage damageInterface = DoDamage(hitObj, attackData.Damage);
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