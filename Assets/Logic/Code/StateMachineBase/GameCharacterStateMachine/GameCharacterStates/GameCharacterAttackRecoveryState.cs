using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterAttackRecoveryState : AGameCharacterState
{
	public GameCharacterAttackRecoveryState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
	
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.AttackRecovery;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
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

	public override void EndState(EGameCharacterState newState)
	{
	
	}
}