using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameCharacterMovementComponent;

public class GameCharacterHookedToCharacterState : AGameCharacterState
{
	bool enemyHitGround = false;
	public GameCharacterHookedToCharacterState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		if (ShouldLeaveState())
		{
			GameCharacter.RequestBestCharacterState();
			return;
		}
		enemyHitGround = false;

		GameCharacter.MovementComponent.MovementVelocity = GameCharacter.CombatComponent.HookedToCharacter.MovementComponent.MovementVelocity;

		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;
		GameCharacter.CombatComponent.HookedToCharacter.MovementComponent.onMoveCollisionFlag += OnEnemyMoveCollisionFlag;
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.HookedToCharacter;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (newStateRequest != EGameCharacterState.Unknown) return newStateRequest;
		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		if (ShouldLeaveState())
		{
			GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
			return;
		}

		if (enemyHitGround)
		{
			// Gravity when enemy hits ground but u didnt
			float yGravity = GameCharacter.MovementComponent.CalculateGravity();
			GameCharacter.MovementComponent.MovementVelocity = new Vector3(GameCharacter.MovementComponent.MovementVelocity.x, GameCharacter.MovementComponent.MovementVelocity.y - yGravity, GameCharacter.MovementComponent.MovementVelocity.z);
			GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
		} else
		{
			// Follow Enemy
			GameCharacter.MovementComponent.MovementVelocity = GameCharacter.CombatComponent.HookedToCharacter.MovementComponent.MovementVelocity;
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

	}

	bool ShouldLeaveState()
	{
		if (GameCharacter == null || GameCharacter.CombatComponent == null || GameCharacter.CombatComponent.HookedToCharacter  == null|| GameCharacter.CombatComponent.HookedToCharacter.MovementComponent == null) return true;
		return false;
	}

	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		if ((collisionFlag & CollisionFlags.Above) != 0)
		{
			GameCharacter.RequestBestCharacterState();
		}
		else if ((collisionFlag & CollisionFlags.Below) != 0)
		{

		}
	}

	void OnEnemyMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		if ((collisionFlag & CollisionFlags.Above) != 0)
		{

		}
		else if ((collisionFlag & CollisionFlags.Below) != 0)
		{
			enemyHitGround = true;
		}
	}
}