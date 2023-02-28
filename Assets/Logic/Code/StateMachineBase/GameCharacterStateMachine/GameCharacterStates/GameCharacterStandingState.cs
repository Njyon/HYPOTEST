using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterStandingState : AGameCharacterState
{
	public GameCharacterStandingState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(GameCharacterState oldState)
	{

	}

	public override GameCharacterState GetStateType()
	{
		return GameCharacterState.Standing;
	}

	public override GameCharacterState UpdateState(float deltaTime, GameCharacterState newStateRequest)
	{
		if (GameCharacter.Veloctiy.magnitude > 0 || GameCharacter.GetMovementInputDir().magnitude > 0)
			return GameCharacterState.Moving;

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

	public override void EndState(GameCharacterState newState)
	{

	}
}
