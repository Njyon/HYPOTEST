using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterFreezState : AGameCharacterState
{
	public GameCharacterFreezState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		GameCharacter.FreezTimer.Start(GameCharacter.FreezTime);

		GameCharacter.FreezTimer.onTimerPaused += TimerEnded;
		GameCharacter.FreezTimer.onTimerFinished += TimerEnded;
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Freez;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (newStateRequest != EGameCharacterState.Unknown) return newStateRequest;
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
		if (GameCharacter != null && GameCharacter.FreezTimer != null) GameCharacter.FreezTimer.onTimerPaused -= TimerEnded;
		if (GameCharacter != null && GameCharacter.FreezTimer != null) GameCharacter.FreezTimer.onTimerFinished -= TimerEnded;
	}

	public void TimerEnded()
	{
		GameCharacter.RequestBestCharacterState();
	}
}