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
		if (!GameCharacter.IsGrounded || GameCharacter.IsInJump)
			return EGameCharacterState.InAir;

		if (GameCharacter.GetPossibleGroundAngle() <= GameCharacter.CharacterController.slopeLimit)
		{
			if (GameCharacter.Veloctiy.magnitude > 0 || GameCharacter.GetHorizontalMovementInputDir().magnitude > 0)
				return EGameCharacterState.Moving;

			if (GameCharacter.Veloctiy.magnitude <= 0 && GameCharacter.GetHorizontalMovementInputDir().magnitude <= 0)
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
		float maxSpeed = GameCharacter.CharacterData.MaxSlidingSpeed;
		float acceleration = GameCharacter.CharacterData.SlidingAcceleration;
		
		Vector3 velocity = GameCharacter.MovementVelocity;
		Vector3 inputDir = Vector3.down * GameCharacter.CharacterData.MovmentGravity * Time.deltaTime;
		if (GameCharacter.PossibleGround != null) inputDir = Vector3.ProjectOnPlane(inputDir.normalized, GameCharacter.PossibleGround.hit.normal);

		Vector3 targetVelocity = inputDir.normalized * maxSpeed;
		Vector3 deltaV = targetVelocity - velocity;
		deltaV = Vector3.ClampMagnitude(deltaV, acceleration);
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, deltaV, 10, Color.black);
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, targetVelocity, 10, Color.green);
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, velocity, 10, Color.white);
		velocity += deltaV;
		
		// Anwenden der Geschwindigkeit
		velocity = new Vector3(velocity.x, velocity.y, GameCharacter.MovementVelocity.z);
		GameCharacter.MovementVelocity = velocity;
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
