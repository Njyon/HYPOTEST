using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterMoveToPositionState : AGameCharacterState
{
	float interpolationSpeed = 20f;
	float distenceMultiplier = 100f;
	public GameCharacterMoveToPositionState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{

	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.MoveToPosition;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (newStateRequest != EGameCharacterState.Unknown)
			return newStateRequest;
		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		GameCharacter.MovementComponent.MovementVelocity = (GameCharacter.CombatComponent.MoveToPosition - GameCharacter.transform.position).normalized * (interpolationSpeed * (Vector3.Distance(GameCharacter.transform.position, GameCharacter.CombatComponent.MoveToPosition) * distenceMultiplier)) * deltaTime;
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