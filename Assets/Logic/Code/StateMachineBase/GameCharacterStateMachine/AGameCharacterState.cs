using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AGameCharacterState : IState<EGameCharacterState>
{
	GameCharacterStateMachine stateMachine;
	GameCharacter gameCharacter;
	public GameCharacterStateMachine StateMachine { get { return stateMachine; } }
	public GameCharacter GameCharacter { get { return gameCharacter; } }
	public const EGameCharacterState gameCharacterState = EGameCharacterState.Unknown;

	public AGameCharacterState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter)
	{
		this.stateMachine = stateMachine;
		this.gameCharacter = gameCharacter;
	}


	public abstract void EndState(EGameCharacterState newState);
	public abstract void ExecuteState(float deltaTime);
	public abstract void FixedExecuteState(float deltaTime);
	public abstract EGameCharacterState GetStateType();
	public abstract void LateExecuteState(float deltaTime);
	public abstract void StartState(EGameCharacterState oldState);
	public abstract EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest);

	public void ResetJumps()
	{
		GameCharacter.CurrentJumpAmount = 0;
	}

	public void SetSlopStrenghToZero(EGameCharacterState oldState)
	{
		switch (oldState)
		{
			default: break;
		}
	}
}
