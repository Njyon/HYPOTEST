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
		GameCharacter.AnimController.SwitchFreezState();
		GameCharacter.AnimController.InFreez = true;
		GameCharacter.MovementComponent.UseGravity = false;
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
		if (Ultra.Utilities.IsNearlyEqual(GameCharacter.transform.position, GameCharacter.CombatComponent.MoveToPosition, 0.1f))
		{
			// Arived at Location
			GameCharacter.CombatComponent.HookedToCharacter.CharacterMoveToPositionStateCharacterOnDestination(GameCharacter);
			GameCharacter.CombatComponent.HookedToCharacter = null;
			if (GameCharacter.StateMachine.CanSwitchToStateOrIsState(EGameCharacterState.Freez))
				GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
			else 
				GameCharacter.RequestBestCharacterState();
		}
	}

	public override void FixedExecuteState(float deltaTime)
	{
	
	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(EGameCharacterState newState)
	{
		GameCharacter.AnimController.InFreez = false;
		GameCharacter.MovementComponent.UseGravity = true;
		GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
	}
}