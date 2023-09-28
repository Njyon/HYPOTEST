using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Action")]
public class BTMoveToMovementTarget : BTHyppoliteActionNodeBase
{

	[Header("MoveToMovementTargetData")]
	public bool checkHight = false;
	public float minDistance = 0.05f;
	public float threshhold = 0.5f;

	Vector3 MovementTarget {
		get {
			if (Tree.Variable.TryGetParam<Vector3>("MovementTarget", out var target))
				return target.Value;
			return Vector3.zero;
		}
	}


	protected override Status OnTick(BTNode from, object options = null)
	{
		Vector3 movementTarget = MovementTarget;
		Ultra.Utilities.DrawWireSphere(movementTarget, 1, Color.blue, 0f, 100, DebugAreas.AI);
		float distance;
		if (checkHight)
			distance = Vector3.Distance(GameCharacter.MovementComponent.CharacterCenter, MovementTarget);
		else 
			distance = Vector3.Distance(Ultra.Utilities.IgnoreAxis(GameCharacter.MovementComponent.CharacterCenter, EAxis.YZ), Ultra.Utilities.IgnoreAxis(movementTarget, EAxis.YZ));

		if (Ultra.Utilities.IsNearlyEqual(distance, minDistance, threshhold))
		{
			GameCharacter.VerticalMovmentInput(0);
			GameCharacter.HorizontalMovementInput(0);
		}else
		{
			Vector3 dir = movementTarget - GameCharacter.MovementComponent.CharacterCenter;
			dir = dir.normalized;

			GameCharacter.VerticalMovmentInput(dir.y);
			GameCharacter.HorizontalMovementInput(dir.x);
		}

		return Status.Running;
	}
}
