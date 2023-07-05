using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterSlidingState : AGameCharacterState
{
	public GameCharacterSlidingState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(EGameCharacterState oldState)
	{

	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		switch (newStateRequest)
		{
			case EGameCharacterState.Attack: return EGameCharacterState.Attack;
			case EGameCharacterState.HookedToCharacter: return EGameCharacterState.HookedToCharacter;
			default: break;
		}

		if (GameCharacter.CheckIfCharacterIsInAir())
			return EGameCharacterState.InAir;

		if (!GameCharacter.CheckIfCharacterIsOnSteepGround())
		{
			if (GameCharacter.CheckIfCharacterIsMoving())
				return EGameCharacterState.Moving;

			if (GameCharacter.CheckIfCharacterIsStanding())
				return EGameCharacterState.Standing;
		}

		return GetStateType();
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Sliding;
	}

	public override void ExecuteState(float deltaTime)
	{
		 //MaybeAir Speed?
		float maxSpeed = GameCharacter.GameCharacterData.MaxSlidingSpeed;
		float acceleration = GameCharacter.GameCharacterData.SlidingAcceleration;
		
		Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
		Vector3 inputDir = Vector3.down * GameCharacter.GameCharacterData.MovmentGravity * Time.deltaTime;
		if (GameCharacter.MovementComponent.PossibleGround != null) inputDir = Vector3.ProjectOnPlane(inputDir.normalized, GameCharacter.MovementComponent.PossibleGround.hit.normal);

		Vector3 targetVelocity = inputDir.normalized * maxSpeed;
		Vector3 deltaV = targetVelocity - velocity;
		deltaV = Vector3.ClampMagnitude(deltaV, acceleration);
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, deltaV, 10, Color.black);
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, targetVelocity, 10, Color.green);
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, velocity, 10, Color.white);
		velocity += deltaV;
		
		// Anwenden der Geschwindigkeit
		velocity = new Vector3(velocity.x, velocity.y, GameCharacter.MovementComponent.MovementVelocity.z);
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

	}
}
