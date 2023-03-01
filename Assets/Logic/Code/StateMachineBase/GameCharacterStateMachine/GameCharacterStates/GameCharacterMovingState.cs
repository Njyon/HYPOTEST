using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterMovingState : AGameCharacterState
{
	public GameCharacterMovingState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(GameCharacterState oldState)
	{
		ResetJumps();
	}

	public override GameCharacterState GetStateType()
	{
		return GameCharacterState.Moving;
	}

	public override GameCharacterState UpdateState(float deltaTime, GameCharacterState newStateRequest)
	{
		if (!GameCharacter.IsGrounded)
			return GameCharacterState.InAir;

		if (GameCharacter.Veloctiy.magnitude <= 0 && GameCharacter.GetMovementInputDir().magnitude <= 0)
			return GameCharacterState.Standing;

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		Vector2 inputVector = GameCharacter.MovementInput;
		inputVector.y = 0;

		Ultra.Utilities.Instance.DebugLogOnScreen("InputVector: " + inputVector.ToString());

		if (GameCharacter.PossibleGround != null)
			inputVector = Vector3.ProjectOnPlane(inputVector, GameCharacter.PossibleGround.hit.normal);

		Ultra.Utilities.Instance.DebugLogOnScreen("InputVector after Normal: " + inputVector.ToString());

		float maxSpeed = GameCharacter.CharacterData.MaxMovementSpeed;
		float acceleration = GameCharacter.CharacterData.Acceleration;

		Vector3 velocity = GameCharacter.MovementVelocity;
		//velocity = new Vector3(velocity.x, 0f, 0f);
		Vector3 input3D = new Vector3(inputVector.x, 0, 0);
		Vector3 targetVelocity = inputVector.normalized * maxSpeed;
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, targetVelocity, 10f, Color.green);

		if (inputVector.magnitude > 0)
		{
			// Beschleunigung
			Vector3 deltaV = targetVelocity - velocity;
			deltaV = Vector3.ClampMagnitude(deltaV, acceleration);
			velocity += deltaV;

			//velocity += targetVelocity * acceleration;
			//velocity.x = Vector3.ClampMagnitude(velocity, maxSpeed).x;
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
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, velocity, 10f, Color.magenta);
		GameCharacter.MovementVelocity = velocity;
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
