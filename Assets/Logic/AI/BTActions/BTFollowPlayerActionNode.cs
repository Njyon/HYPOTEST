using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Action")]
public class BTFollowPlayerActionNode : BTHyppoliteActionNodeBase
{
	public bool Success = true;

	protected override Status OnTick(BTNode from, object options = null)
	{

		return Status.Running;
	}
}
