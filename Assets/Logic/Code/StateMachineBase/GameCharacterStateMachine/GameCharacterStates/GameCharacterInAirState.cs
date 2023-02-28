using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterInAirState : AGameCharacterState
{
	public GameCharacterInAirState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(GameCharacterState oldState)
	{

	}

	public override GameCharacterState GetStateType()
	{
		return GameCharacterState.InAir;
	}

	public override GameCharacterState UpdateState(float deltaTime, GameCharacterState newStateRequest)
	{
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
