using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameCharacterHookedToCharacterState : AGameCharacterState
{
	public GameCharacterHookedToCharacterState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		if (ShouldLeaveState())
		{
			GameCharacter.RequestBestCharacterState();
			return;
		}

		GameCharacter.MovementComponent.MovementVelocity = GameCharacter.CombatComponent.HookedToCharacter.MovementComponent.MovementVelocity;
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.HookedToCharacter;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{ 
		switch (newStateRequest)
		{
			case EGameCharacterState.PullCharacterOnHorizontalLevel: return EGameCharacterState.PullCharacterOnHorizontalLevel;
			case EGameCharacterState.Freez: return EGameCharacterState.Freez;
			default: break;
		}
		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		if (ShouldLeaveState())
		{
			GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
			return;
		}

		GameCharacter.MovementComponent.MovementVelocity = GameCharacter.CombatComponent.HookedToCharacter.MovementComponent.MovementVelocity;
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

	bool ShouldLeaveState()
	{
		if (GameCharacter == null || GameCharacter.CombatComponent == null || GameCharacter.CombatComponent.HookedToCharacter  == null|| GameCharacter.CombatComponent.HookedToCharacter.MovementComponent == null) return true;
		return false;
	}
}