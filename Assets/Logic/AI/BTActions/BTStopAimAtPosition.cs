using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTStopAimAtPosition : BTHyppoliteActionNodeBase
{
	protected override Status OnTick(BTNode from, object options = null)
	{
		if (GameCharacter != null)
		{
			GameCharacter.CombatComponent.AimPositionCheck.Value = false;
		}
		else
		{
			return Status.Running;
		}

		return Status.Succeeded;
	}
}
