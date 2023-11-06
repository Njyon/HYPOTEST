using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Service")]
public class BTBlockAimService : BTServiceNodeBase
{
	protected override void OnEnter(object options = null)
	{
		if (GameCharacter != null)
		{
			GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.BlockAim);
		}
	}

	protected override Status OnTick(BTNode from, object options = null)
	{
		var result = Child0.Tick(this, options);

		if (result == Status.Running)
			return Status.Running;

		if (result == Status.Succeeded)
			return Status.Succeeded;

		return Status.Failed;
	}

	protected override void OnExit(Status result, object options = null)
	{
		base.OnExit(result, options);

		if (GameCharacter != null)
		{
			GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.BlockAim);
		}
	}
}
