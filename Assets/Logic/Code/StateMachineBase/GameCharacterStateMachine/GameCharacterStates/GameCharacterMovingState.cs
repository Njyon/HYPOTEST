using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterMovingState : AGameCharacterState
{
	public GameCharacterMovingState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(EGameCharacterState oldState)
	{
		ResetJumps();
		SetSlopStrenghToZero(oldState);
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Moving;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (!GameCharacter.IsGrounded)
			return EGameCharacterState.InAir;

		if (GameCharacter.GetPossibleGroundAngle() > GameCharacter.CharacterController.slopeLimit)
			return EGameCharacterState.Sliding;

		if (GameCharacter.Veloctiy.magnitude <= 0 && GameCharacter.GetMovementInputDir().magnitude <= 0)
			return EGameCharacterState.Standing;

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		Vector2 inputVector = GameCharacter.MovementInput;
		inputVector.y = 0;

		if (GameCharacter.PossibleGround != null)
			inputVector = Vector3.ProjectOnPlane(inputVector, GameCharacter.PossibleGround.hit.normal);

		float maxSpeed = GameCharacter.CharacterData.MaxMovementSpeed;
		float acceleration = GameCharacter.CharacterData.Acceleration;

		Vector3 velocity = GameCharacter.MovementVelocity;
		//velocity = new Vector3(velocity.x, 0f, 0f);
		Vector3 input3D = new Vector3(inputVector.x, 0, 0);
		Vector3 targetVelocity = inputVector.normalized * maxSpeed;

		if (inputVector.magnitude > 0)
		{
			// Beschleunigung
			Vector3 deltaV = targetVelocity - velocity;
			deltaV = Vector3.ClampMagnitude(deltaV, acceleration);
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, deltaV, 10, Color.black);
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, targetVelocity, 10, Color.green);
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, velocity, 10, Color.white);
			velocity += deltaV;
		}
		else
		{
			// Bremsen
			float drag = GameCharacter.CharacterData.Drag;
			float deceleration = drag;
			velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration);
		}

		// Anwenden der Geschwindigkeit
		velocity = new Vector3(velocity.x, velocity.y, GameCharacter.MovementVelocity.z);
		velocity.y += Physics.gravity.y * Time.deltaTime;
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
