using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterFreezState : AGameCharacterState
{
	public GameCharacterFreezState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		if (GameCharacter.FreezTimeOverride <= 0) 
			GameCharacter.FreezTimer.Start(GameCharacter.FreezTime);
		else
			GameCharacter.FreezTimer.Start(GameCharacter.FreezTimeOverride);

		GameCharacter.FreezTimeOverride = 0;
		GameCharacter.MovementComponent.VariableGravityMultiplierOverTime = GameCharacter.GameCharacterData.GravityMultiplierInAttack;
		GameCharacter.StateMachine.RemoveLazyState(EGameCharacterState.Freez);
		//GameCharacter.MovementComponent.UseGravity = false;

		GameCharacter.FreezTimer.onTimerPaused += TimerEnded;
		GameCharacter.FreezTimer.onTimerFinished += TimerEnded;
		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;

		GameCharacter.AnimController.InFreez = true;
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Freez;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (newStateRequest != EGameCharacterState.Unknown) 
			return newStateRequest;
		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
		Ultra.Utilities.Instance.DebugLogOnScreen("Velocity: " + velocity.ToString(), 0f, StringColor.Brown, 100, DebugAreas.Combat);
		float drag = 40f;
		velocity = Vector3.MoveTowards(velocity, Vector3.zero, drag);
		GameCharacter.MovementComponent.MovementVelocity = velocity;
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
		//GameCharacter.MovementComponent.UseGravity = true;
		GameCharacter.MovementComponent.InterpGravityUp();

		if (GameCharacter != null && GameCharacter.FreezTimer != null) GameCharacter.FreezTimer.onTimerPaused -= TimerEnded;
		if (GameCharacter != null && GameCharacter.FreezTimer != null) GameCharacter.FreezTimer.onTimerFinished -= TimerEnded;
		if (GameCharacter != null && GameCharacter.MovementComponent != null)  GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;
	}

	public void TimerEnded()
	{
		GameCharacter.RequestBestCharacterState();
	}

	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		if ((collisionFlag & CollisionFlags.Below) != 0)
		{
			if (GameCharacter.MovementComponent.Velocity.magnitude > 2f)
			{
				GameCharacter.MovementComponent.MovementVelocity = Vector3.Reflect(GameCharacter.MovementComponent.MovementVelocity, GameCharacter.MovementComponent.PossibleGround.hit.normal);
			}
		}
	}
}