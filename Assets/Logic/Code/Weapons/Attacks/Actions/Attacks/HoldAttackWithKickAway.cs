using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HoldAttackWithKickAwayData : AttackData
{
	public AnimationClip attackAnimation;
	public AnimationClip holdAttackAnimation;
}

public class HoldAttackWithKickAway : AttackBase
{
	public HoldAttackWithKickAwayData attackData;

	public override void StartAction()
	{

	}

	public override ActionBase CreateCopy()
	{
		HoldAttackWithKickAway copy = new HoldAttackWithKickAway();
		copy.attackData = attackData;
		return copy;
	}
}
