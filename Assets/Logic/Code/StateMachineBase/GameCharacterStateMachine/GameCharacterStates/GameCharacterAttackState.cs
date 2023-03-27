using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterAttackState : AGameCharacterState
{
	public GameCharacterAttackState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		GameCharacter.AnimController.InAttack = true;
		GameCharacter.AnimController.InterpRotationLayerWeight(0, 10f);
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(0, 10f);
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Attack;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		switch (newStateRequest)
		{
			case EGameCharacterState.AttackRecovery: return EGameCharacterState.AttackRecovery;
			default: break;
		}

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