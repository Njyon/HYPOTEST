using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Service")]
public class BTAimAtTargetService : BTServiceNodeBase
{
	protected override void OnEnter(object options = null)
	{
		base.OnEnter(options);

		if (TargetGameCharacter != null)
		{
			GameCharacter.CombatComponent.AimTarget = TargetGameCharacter;
			GameCharacter.AnimController.ApplyBlendTree(GameCharacter.CombatComponent.CurrentWeapon.AnimationData.AimAnimations);
			GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Aim);
			GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.LookAtAimTargetDirection);
		}else
		{
			Ultra.Utilities.Instance.DebugErrorString("BTAimAtTargetService", "OnEnter", "TargetGameCharacter was not set, Servies is on wrong Position, without check or something went wrong!");
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
			GameCharacter.CombatComponent.AimTarget = null;
			GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
			GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.LookAtAimTargetDirection);
		}
	}
}
