using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterLookInVelocityDirectionPluginState : AGameCharacterPluginState
{
	public GameCharacterLookInVelocityDirectionPluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base (gameCharacter, pluginStateMachine)
	{ }
	
	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.LookInVelocityDirection;
	}

    public override void Active()
	{
		base.Active();
	}

	public override void Deactive()
	{
		base.Deactive();
	}

	public override void AddState()
	{
		// Set Default Data
		if (GameCharacter.LastDir == Vector3.zero)
			GameCharacter.LastDir = GameCharacter.transform.right;
	}
	
	public override void RemoveState()
	{
	
	}

	public override bool WantsToBeActive()
	{
		switch (GameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Attack:
			case EGameCharacterState.AttackRecovery:
			case EGameCharacterState.DefensiveAction:
			case EGameCharacterState.FlyAway:
			case EGameCharacterState.MoveToPosition:
				return false;
			default: break;
		}
		if (GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.LookAtAimTargetDirection)) return false;
		return true;
	}

	public override void ExecuteState(float deltaTime)
	{
		if (Mathf.Abs(GameCharacter.MovementComponent.MovementVelocity.x) >= 0.2f) GameCharacter.LastDir = new Vector3(GameCharacter.MovementComponent.MovementVelocity.x, 0, 0);
		if (GameCharacter.LastDir == Vector3.zero) return;
		GameCharacter.RotationTarget = Quaternion.LookRotation(GameCharacter.LastDir.normalized, Vector3.up);
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
		//Ultra.Utilities.DrawArrow(transform.position, targetRot * Vector3.forward, 10, Color.green);
	}
}