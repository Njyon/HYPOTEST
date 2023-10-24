using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParryDefensiveActionData : AttackData
{
	public AnimationClip defensiveAction;
	public AnimationClip defensiveActionHold;

}

public class ParryDefensiveAction : ActionBase
{
	public ParryDefensiveActionData attackData;

	public override void StartAction()
	{
		Debug.Log("StartAction");
	}

	public override void StartActionInHold()
	{
		Debug.Log("StartActionInHold");
	}

	public override void ImplementUI()
	{
		Debug.Log("ImplementUI");
	}

	public override bool HasAttackInputInHold()
	{
		return true;
	}

	public override bool HasUIImplementation()
	{
		return true;
	}

	public override ActionBase CreateCopy()
	{
		ParryDefensiveAction copy = new ParryDefensiveAction();
		copy.attackData = attackData;
		return copy;
	}
}
