using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterMovingState : AGameCharacterState
{
	public GameCharacterMovingState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(GameCharacterState oldState)
	{

	}

	public override GameCharacterState GetStateType()
	{
		return GameCharacterState.Moving;
	}

	public override GameCharacterState UpdateState(float deltaTime, GameCharacterState newStateRequest)
	{
		if (GameCharacter.Veloctiy.magnitude <= 0 && GameCharacter.GetMovementInputDir().magnitude <= 0)
			return GameCharacterState.Standing;

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		Vector2 inputVector = GameCharacter.MovementInput;
		inputVector.y = 0;

		float maxSpeed = GameCharacter.CharacterData.MaxMovementSpeed;
		float acceleration = GameCharacter.CharacterData.Acceleration;

		Vector3 velocity = GameCharacter.CharacterController.velocity;
		Vector3 input3D = new Vector3(inputVector.x, 0, 0);
		Vector3 targetVelocity = input3D.normalized * maxSpeed;

		if (inputVector.magnitude > 0)
		{
			// Beschleunigung
			Vector3 deltaV = targetVelocity - velocity;
			deltaV = Vector3.ClampMagnitude(deltaV, acceleration);
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
		GameCharacter.MovementVelocity = velocity * Time.deltaTime;
	}

	public override void FixedExecuteState(float deltaTime)
	{

	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(GameCharacterState newState)
	{

	}
}
