using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameCharacterState 
{ 
    Unknown = 0,
    Standing = 1,
    Moving = 2,
    InAir = 4,
}

public class GameCharacterStateMachine : AStateMachineBase<GameCharacterState>
{
	GameCharacter gameCharacter;

	protected override void Start()
	{
		gameCharacter = gameObject.GetComponent<GameCharacter>();
		base.Start();
	}

	public override bool CompareStateTypes(GameCharacterState A, GameCharacterState B)
	{
		return A == B;
	}

	public override IState<GameCharacterState> CreateUnknownStartState()
	{
		IState<GameCharacterState> unknownState = null;
		CreateState(GameCharacterState.Unknown, out unknownState);
		return unknownState;
	}

	public override GameCharacterState GetUnknownT()
	{
		return GameCharacterState.Unknown;
	}

	public override void InitPreviousStatesWithUnknown()
	{
		for (int i = 0; i < PreviousStateListLenght; i++)
		{
			PreviousStates[i] = CreateUnknownStartState();
		}
	}

	public override bool IsStateTypeUnknown(GameCharacterState stateType)
	{
		return stateType == GameCharacterState.Unknown;
	}

	protected override IState<GameCharacterState> CreateDefaultState()
	{
		IState<GameCharacterState> newState = null;
		CreateState(GameCharacterState.Standing, out newState);
		return newState;
	}

	protected override bool CreateState(GameCharacterState stateType, out IState<GameCharacterState> newState)
	{
		switch (stateType)
		{
			case GameCharacterState.Standing: newState = new GameCharacterStandingState(this, gameCharacter); break;
			case GameCharacterState.Moving: newState = new GameCharacterMovingState(this, gameCharacter); break;
			case GameCharacterState.InAir: newState = new GameCharacterInAirState(this, gameCharacter); break;
			default:
				newState = null;
				return false;
		}
		return true;
	}
}
