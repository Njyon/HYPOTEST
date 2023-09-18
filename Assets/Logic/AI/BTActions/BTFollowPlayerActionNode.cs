using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Action")]
public class BTFollowPlayerActionNode : BTHyppoliteActionNodeBase
{
	[Header("FollowPlayerData")]
	public float minDistanceToTarget = 1;
	public float treshhold = 1f;
	public bool checkWithHight = false;

	protected override Status OnTick(BTNode from, object options = null)
	{
		float distance;
		if (checkWithHight)
			distance = Vector3.Distance(GameCharacter.transform.position, TargetGameCharacter.transform.position);
		else
			distance = Vector3.Distance(Ultra.Utilities.IgnoreAxis(GameCharacter.transform.position, EAxis.YZ), Ultra.Utilities.IgnoreAxis(TargetGameCharacter.transform.position, EAxis.YZ));

		if (Ultra.Utilities.IsNearlyEqual(distance, minDistanceToTarget, treshhold))
		{
			GameCharacter.VerticalMovmentInput(0);
			GameCharacter.HorizontalMovementInput(0);
		}else
		{
			Vector3 dirToTarget = (TargetGameCharacter.transform.position - GameCharacter.transform.position).normalized;
			Vector3 dirToDestinationPoint = ((TargetGameCharacter.transform.position + (-dirToTarget) * minDistanceToTarget) - GameCharacter.transform.position).normalized;
			GameCharacter.VerticalMovmentInput(dirToDestinationPoint.y);
			GameCharacter.HorizontalMovementInput(dirToDestinationPoint.x);
		}

		return Status.Running;
	}
}
