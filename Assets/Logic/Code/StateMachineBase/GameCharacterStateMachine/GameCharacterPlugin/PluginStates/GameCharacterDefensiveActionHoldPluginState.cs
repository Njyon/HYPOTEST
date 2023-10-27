using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterDefensiveActionHoldPluginState : AGameCharacterPluginState
{
	public GameCharacterDefensiveActionHoldPluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base (gameCharacter, pluginStateMachine)
	{ }
	
	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.DefensiveActionHold;
	}

    public override void Active()
	{
		base.Active();
		if (GameCharacter != null && GameCharacter.StateMachine != null)
			GameCharacter.StateMachine.onStateChanged += OnStateChanged;
	}

	public override void Deactive()
	{
		if (GameCharacter != null && GameCharacter.StateMachine != null)
			GameCharacter.StateMachine.onStateChanged -= OnStateChanged;
		base.Deactive();
	}

	public override void AddState()
	{
	
	}
	
	public override void RemoveState()
	{
	
	}

	public override bool WantsToBeActive()
	{
		return true;
	}

	public override void ExecuteState(float deltaTime)
	{
	
	}

	void OnStateChanged(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		if (newState != null && newState.GetStateType() != EGameCharacterState.DefensiveAction)
		{
			GameCharacter.EventComponent.AddEvent(new DefensiveEvent(GameCharacter));
		}
	}
}