using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GameCharacterDefensiveActionState : AGameCharacterState
{
	float initYVelocity;
	float initXVelocity;
	float lerpTimeY;
	float lerpTimeX;
	float currentYPosAnimCurve;
	Quaternion newDir;
	public GameCharacterDefensiveActionState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		GameCharacter.AnimController.InDefensiveAction = true;
		GameCharacter.MovementComponent.UseGravity = false;
		GameCharacter.CombatComponent.DefensiveTimer.Start(GameCharacter.CombatComponent.CurrentWeapon.CurrentDefensiveAction.clip.length);

		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(0, 10f);
		initYVelocity = GameCharacter.MovementComponent.MovementVelocity.y;
		initXVelocity = GameCharacter.MovementComponent.MovementVelocity.x;
		lerpTimeY = 0;
		lerpTimeX = 0;

		GameCharacter.AnimController.RotationTrarget = 0f;
		if (GameCharacter.MovementInput.x != 0)
		{
			Vector3 currentDir = new Vector3(GameCharacter.MovementInput.x, 0, 0);
			newDir = Quaternion.LookRotation(currentDir.normalized, Vector3.up);
		}
		else
		{
			newDir = GameCharacter.RotationTarget;
		}
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.DefensiveAction;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (newStateRequest != EGameCharacterState.Unknown)
			return newStateRequest;

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		RotateCharacter(newDir);

		CombatMovement(deltaTime, initYVelocity, initXVelocity, ref lerpTimeY, ref lerpTimeX, ref currentYPosAnimCurve);

	}
	
	public override void FixedExecuteState(float deltaTime)
	{
	
	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(EGameCharacterState newState)
	{
		GameCharacter.MovementComponent.UseGravity = true;
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(1);
	}
}