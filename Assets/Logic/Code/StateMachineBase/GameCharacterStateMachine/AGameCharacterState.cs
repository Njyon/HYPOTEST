using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGameCharacterState : IState<GameCharacterState>
{
	GameCharacterStateMachine stateMachine;
	GameCharacter gameCharacter;
	public GameCharacterStateMachine StateMachine { get { return stateMachine; } }
	public GameCharacter GameCharacter { get { return gameCharacter; } }

	public AGameCharacterState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter)
	{
		this.stateMachine = stateMachine;
		this.gameCharacter = gameCharacter;
	}

	public abstract void EndState(GameCharacterState newState);
	public abstract void ExecuteState(float deltaTime);
	public abstract void FixedExecuteState(float deltaTime);
	public abstract GameCharacterState GetStateType();
	public abstract void LateExecuteState(float deltaTime);
	public abstract void StartState(GameCharacterState oldState);
	public abstract GameCharacterState UpdateState(float deltaTime, GameCharacterState newStateRequest);

	public void ResetJumps()
	{
		GameCharacter.CurrentJumpAmount = 0;
	}
}
