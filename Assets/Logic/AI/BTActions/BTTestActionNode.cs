using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Action")]
public class BTTestActionNode : BTHyppoliteActionNodeBase
{
    public bool Success = false;

	protected override void OnEnter(object options = null)
	{
		base.OnEnter(options);

	}

	protected override Status OnTick(BTNode from, object options = null)
	{
		float distance = Vector3.Distance(GameCharacter.transform.position, TargetGameCharacter.transform.position);

		Ultra.Utilities.Instance.DebugLogOnScreen("Distance: " + distance, 0f, distance <= 10f ? StringColor.White : StringColor.Red);

		return Status.Running;
	}
}
