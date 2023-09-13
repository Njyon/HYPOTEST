using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterMoveToPositionState : AGameCharacterState
{
	float interpolationSpeed = 2f;
	float distenceMultiplier = 10f;
	bool sideHit = false;
	public GameCharacterMoveToPositionState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		GameCharacter.AnimController.SwitchFreezState();
		GameCharacter.AnimController.InFreez = true;
		GameCharacter.MovementComponent.UseGravity = false;
		GameCharacter.MovementComponent.MoveThroughCharacterLayer();

		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;
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
		GameCharacter.MovementComponent.MovementVelocity = (GameCharacter.CombatComponent.MoveToPosition - GameCharacter.transform.position).normalized * (interpolationSpeed * (Vector3.Distance(GameCharacter.transform.position, GameCharacter.CombatComponent.MoveToPosition) * distenceMultiplier));
		if (Ultra.Utilities.IsNearlyEqual(GameCharacter.transform.position, GameCharacter.CombatComponent.MoveToPosition, 0.4f))
		{
			// Arived at Location
			if (GameCharacter.CombatComponent.HookedToCharacter != null) GameCharacter.CombatComponent.HookedToCharacter.CharacterMoveToPositionStateCharacterOnDestination(GameCharacter);
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
		GameCharacter.MovementComponent.InterpGravityUp();
		GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
		if (GameCharacter.CombatComponent.HookedToCharacter != null) GameCharacter.CombatComponent.HookedToCharacter.CharacterMoveToPositionStateAbort(GameCharacter);
		GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;
		GameCharacter.MovementComponent.SetLayerToDefault();
	}

	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		if ((collisionFlag & CollisionFlags.Sides) != 0)
		{
			if (!sideHit)
				sideHit = true;
			else
			{
				if (GameCharacter.CombatComponent.HookedToCharacter != null) GameCharacter.CombatComponent.HookedToCharacter.CharacterMoveToPositionStateAbort(GameCharacter);
				GameCharacter.CombatComponent.HookedToCharacter = null;
				GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
			}
		}
		else
		{
			sideHit = false;
		}
	}
}