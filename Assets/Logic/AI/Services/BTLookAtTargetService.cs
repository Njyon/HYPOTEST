using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Service")]
public class BTLookAtTargetService : BTServiceNodeBase
{
	protected override void OnEnter(object options = null)
	{
		base.OnEnter(options);

		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.LookInVelocityDirection);
	}

	protected override Status OnTick(BTNode from, object options = null)
	{
		if (TargetGameCharacter != null)
		{
			switch (GameCharacter.StateMachine.GetCurrentStateType())
			{
				case EGameCharacterState.Attack:
				case EGameCharacterState.AttackRecovery:
				case EGameCharacterState.DefensiveAction:
				case EGameCharacterState.FlyAway:
				case EGameCharacterState.MoveToPosition:
					break;
				default:
					Vector3 targetDir = (Ultra.Utilities.IgnoreAxis(TargetGameCharacter.MovementComponent.CharacterCenter, EAxis.YZ) - Ultra.Utilities.IgnoreAxis(GameCharacter.MovementComponent.CharacterCenter, EAxis.YZ)).normalized;

					Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, targetDir, 1f, Color.blue, 2f, 200, DebugAreas.AI);
					GameCharacter.RotationTarget = Quaternion.LookRotation(targetDir, Vector3.up);
					Quaternion targetRot = Quaternion.Slerp(GameCharacter.transform.rotation, GameCharacter.RotationTarget, Time.deltaTime * GameCharacter.GameCharacterData.RoationSpeed);
					Vector3 dir = GameCharacter.transform.rotation * Vector3.forward;
					Vector3 cross = Vector3.Cross(GameCharacter.LastDir.normalized, dir);
					float sign = Mathf.Sign(cross.y);
					if (Ultra.Utilities.IsNearlyEqual(cross, Vector3.zero, 0.2f))
					{
						sign = 0f;
					}
					// invert sign because of lerping rotation
					GameCharacter.AnimController.RotationTrarget = sign * -1;
					GameCharacter.transform.rotation = targetRot;
					break;
			}
		}

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

		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.LookInVelocityDirection);
	}
}
