using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterStandingState : AGameCharacterState
{
	public GameCharacterStandingState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(EGameCharacterState oldState)
	{
		SetSlopStrenghToZero(oldState);
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Standing;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		switch (newStateRequest)
		{
			case EGameCharacterState.Attack: return EGameCharacterState.Attack;
			case EGameCharacterState.HookedToCharacter: return EGameCharacterState.HookedToCharacter;
			case EGameCharacterState.PullCharacterOnHorizontalLevel: return EGameCharacterState.PullCharacterOnHorizontalLevel;
			case EGameCharacterState.DefensiveAction: return EGameCharacterState.DefensiveAction;
			case EGameCharacterState.MoveToPosition: return EGameCharacterState.MoveToPosition;
			case EGameCharacterState.FlyAway: return EGameCharacterState.FlyAway;
			case EGameCharacterState.Dodge: return EGameCharacterState.Dodge;
			case EGameCharacterState.Freez: return EGameCharacterState.Freez;
			default: break;
		}

		if (GameCharacter.CheckIfCharacterIsInAir())
			return EGameCharacterState.InAir;

		if (GameCharacter.CheckIfCharacterIsOnSteepGround())
			return EGameCharacterState.InAir;

		if (GameCharacter.CheckIfCharacterIsMoving())
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
