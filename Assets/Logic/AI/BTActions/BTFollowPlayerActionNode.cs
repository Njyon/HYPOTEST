using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System;
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
		// Dont Change Input when in these states!
		switch (GameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Attack:
			case EGameCharacterState.AttackRecovery:
				Vector3 dir = (TargetGameCharacter.transform.position - GameCharacter.transform.position).normalized;
				GameCharacter.HorizontalMovementInput(dir.x);
				GameCharacter.VerticalMovmentInput(dir.y);
				return Status.Running;
			default: break;
		}

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
			Vector3 dirToTarget = checkWithHight ? (TargetGameCharacter.transform.position - GameCharacter.transform.position) : (Ultra.Utilities.IgnoreAxis(TargetGameCharacter.transform.position, EAxis.YZ) - Ultra.Utilities.IgnoreAxis(GameCharacter.transform.position, EAxis.YZ));
			Vector3 dirToDestinationPoint = ((TargetGameCharacter.transform.position + (-dirToTarget.normalized) * minDistanceToTarget) - GameCharacter.transform.position).normalized;

			//Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, dirToTarget.normalized, dirToTarget.magnitude, Color.green, 0f);
			//RaycastHit[] hits = Physics.RaycastAll(GameCharacter.MovementComponent.CharacterCenter, dirToTarget.normalized, dirToTarget.magnitude, LayerMask.NameToLayer("Character"), QueryTriggerInteraction.Collide);
			//foreach (RaycastHit hit in hits)
			//{
			//	if (hit.collider == null || hit.collider.gameObject == GameObject || hit.collider.gameObject == TargetGameCharacter) continue;
			//	Ultra.Utilities.DrawWireSphere(hit.point, 1f, Color.magenta, 10f, 200, DebugAreas.AI);
			//	dirToDestinationPoint = ((hit.point + (-dirToTarget.normalized) * minDistanceToTarget) - GameCharacter.transform.position).normalized;
			//}
			//
			//Ultra.Utilities.Instance.DebugLogOnScreen("Hits Count = " + hits.Length, 0f, StringColor.Teal, 100, DebugAreas.AI);

			GameCharacter.VerticalMovmentInput(dirToDestinationPoint.y);
			GameCharacter.HorizontalMovementInput(dirToDestinationPoint.x);
		}

		return Status.Running;
	}

	protected override void OnAbort(object options = null)
	{
		GameCharacter.VerticalMovmentInput(0);
		GameCharacter.HorizontalMovementInput(0);
	}
}
