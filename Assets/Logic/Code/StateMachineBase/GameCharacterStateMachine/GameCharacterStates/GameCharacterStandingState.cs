using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterStandingState : AGameCharacterState
{
	public GameCharacterStandingState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(EGameCharacterState oldState)
	{
		ResetJumps();
		SetSlopStrenghToZero(oldState);
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Standing;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (!GameCharacter.IsGrounded || GameCharacter.IsInJump)
			return EGameCharacterState.InAir;

		if (GameCharacter.GetPossibleGroundAngle() > GameCharacter.CharacterController.slopeLimit)
			return EGameCharacterState.Sliding;

		if (GameCharacter.Veloctiy.magnitude > 0 || GameCharacter.GetHorizontalMovementInputDir().magnitude > 0)
			return EGameCharacterState.Moving;

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{

	}

	public override void FixedExecuteState(float deltaTime)
	{

	}

	public override void LateExecuteState(float deltaTime)
	{

	}

	public override void EndState(EGameCharacterState newState)
	{

	}
}
