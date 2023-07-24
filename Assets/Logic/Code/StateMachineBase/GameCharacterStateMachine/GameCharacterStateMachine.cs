using EasyButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public enum EGameCharacterState 
{ 
    Unknown = 0,
    Standing = 1,
    Moving = 2,
    InAir = 4,
	Sliding = 5,
	Attack = 6,
	AttackRecovery = 7,
	HookedToCharacter = 8,
	PullCharacterOnHorizontalLevel = 9,
	Freez = 10,
}

public class GameCharacterStateMachine : AStateMachineBase<EGameCharacterState>
{
	GameCharacter gameCharacter;

	protected override void Start()
	{
		gameCharacter = gameObject.GetComponent<GameCharacter>();
		base.Start();
	}

	public override bool CompareStateTypes(EGameCharacterState A, EGameCharacterState B)
	{
		return A == B;
	}

	public override IState<EGameCharacterState> CreateUnknownStartState()
	{
		IState<EGameCharacterState> unknownState = null;
		CreateState(EGameCharacterState.Unknown, out unknownState);
		return unknownState;
	}

	public override EGameCharacterState GetUnknownT()
	{
		return EGameCharacterState.Unknown;
	}

	public override void InitPreviousStatesWithUnknown()
	{
		for (int i = 0; i < PreviousStateListLenght; i++)
		{
			PreviousStates[i] = CreateUnknownStartState();
		}
	}

	public override bool IsStateTypeUnknown(EGameCharacterState stateType)
	{
		return stateType == EGameCharacterState.Unknown;
	}

	protected override IState<EGameCharacterState> CreateDefaultState()
	{
		IState<EGameCharacterState> newState = null;
		CreateState(EGameCharacterState.Standing, out newState);
		return newState;
	}

	protected override bool CreateState(EGameCharacterState stateType, out IState<EGameCharacterState> newState)
	{
		var stateTypes = Assembly.GetExecutingAssembly().GetTypes()
		.Where(t => t.BaseType == typeof(AGameCharacterState))
		.ToList();

		var stateClass = stateTypes.FirstOrDefault(t => t.Name == "GameCharacter" + stateType.ToString() + "State");

		if (stateClass == null)
		{
			newState = null;
			return false;
		}

		newState = Activator.CreateInstance(stateClass, this, gameCharacter) as AGameCharacterState;

		return newState != null;
	}

	public bool CanSwitchToStateOrIsState(EGameCharacterState state)
	{
		return (CurrentState.UpdateState(0, state) == state || GetCurrentStateType() == state);
	}

	[Button("FreezState")]
	async private void RequestFreezState()
	{
		RequestStateChange(EGameCharacterState.Freez, true);
		await new WaitForSeconds(0.5f);
		gameCharacter.FreezTimer.Start(10000f);
		gameCharacter.transform.position = new Vector3(gameCharacter.transform.position.x, gameCharacter.transform.position.y + 1, gameCharacter.transform.position.z);
	}
}
