using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterLookAtAimTargetDirectionPluginState : AGameCharacterPluginState
{
	public GameCharacterLookAtAimTargetDirectionPluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base (gameCharacter, pluginStateMachine)
	{ }
	
	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.LookAtAimTargetDirection;
	}

    public override void Active()
	{
		base.Active();
	}

	public override void Deactive()
	{
		base.Deactive();

		GameCharacter.AnimController.MoveBackwards = false;
	}

	public override void AddState()
	{
	
	}
	
	public override void RemoveState()
	{
	
	}

	public override bool WantsToBeActive()
	{
		return GameCharacter.CombatComponent.AimCharacter != null || GameCharacter.CombatComponent.AimPositionCheck != null;
	}

	public override void ExecuteState(float deltaTime)
	{
		if (GameCharacter.CombatComponent.AimCharacter != null || GameCharacter.CombatComponent.AimPositionCheck != null)
		{
			switch (GameCharacter.StateMachine.GetCurrentStateType())
			{
				case EGameCharacterState.Attack:
				case EGameCharacterState.AttackRecovery:
				case EGameCharacterState.DefensiveAction:
				case EGameCharacterState.FlyAway:
				case EGameCharacterState.MoveToPosition:
				case EGameCharacterState.Freez:
					break;
				default:
					Vector3 targetPos = GameCharacter.CombatComponent.AimCharacter != null ? (Ultra.Utilities.IgnoreAxis(GameCharacter.CombatComponent.AimCharacter.MovementComponent.CharacterCenter, EAxis.YZ)) : (Ultra.Utilities.IgnoreAxis(GameCharacter.CombatComponent.AimPositionCheck.Position, EAxis.YZ));
					Vector3 targetDir = (targetPos - Ultra.Utilities.IgnoreAxis(GameCharacter.MovementComponent.CharacterCenter, EAxis.YZ)).normalized;

					if (GameCharacter.MovementInput.magnitude > 0)
						GameCharacter.AnimController.MoveBackwards = targetDir.normalized.ToVector2() != GameCharacter.MovementInput.normalized;
					else
						GameCharacter.AnimController.MoveBackwards = false;

					Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, targetDir, 1f, Color.blue, 2f, 200, DebugAreas.Combat);
					GameCharacter.RotationTarget = Quaternion.LookRotation(targetDir, Vector3.up);
					Quaternion targetRot = Quaternion.Lerp(GameCharacter.transform.rotation, GameCharacter.RotationTarget, Time.deltaTime * GameCharacter.GameCharacterData.RoationSpeed);
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
					GameCharacter.LastDir = targetDir;
					break;
			}
		}
	}
}