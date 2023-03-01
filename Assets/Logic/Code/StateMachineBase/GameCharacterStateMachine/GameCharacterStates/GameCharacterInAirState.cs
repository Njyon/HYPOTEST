using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterInAirState : AGameCharacterState
{
	public GameCharacterInAirState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(GameCharacterState oldState)
	{

	}

	public override GameCharacterState GetStateType()
	{
		return GameCharacterState.InAir;
	}

	public override GameCharacterState UpdateState(float deltaTime, GameCharacterState newStateRequest)
	{
		if (GameCharacter.IsGrounded && GameCharacter.MovementVelocity.magnitude > 0)
			return GameCharacterState.Moving;
		else if (GameCharacter.IsGrounded)
			return GameCharacterState.Standing;

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		// MaybeAir Speed?
		float maxSpeed = GameCharacter.CharacterData.MaxMovementSpeed;

		Vector3 velocity = GameCharacter.MovementVelocity;
		Vector3 inputDir = new Vector3(GameCharacter.MovementInput.x, 0f, 0f);
		Vector3 targetVelocity = inputDir.normalized * maxSpeed;
		Vector3 velocityDiff = (targetVelocity - velocity);
		Vector3 acceleration = Vector3.ClampMagnitude(velocityDiff, maxSpeed) * GameCharacter.CharacterData.InAirControll;
		velocity += acceleration;

		// Anwenden der Geschwindigkeit
		velocity = new Vector3(velocity.x, GameCharacter.MovementVelocity.y, GameCharacter.MovementVelocity.z);
		GameCharacter.MovementVelocity = velocity;
	}

	public override void FixedExecuteState(float deltaTime)
	{
		Vector3[] points = Ultra.Utilities.CalculateTrijactoryPoints(5, 0.3f, GameCharacter.transform.position, GameCharacter.MovementVelocity, Physics.gravity * 7);
		bool didHit = false;
		for (int i = 1; i < points.Length; i++)
		{
			Ultra.Utilities.DrawWireSphere(points[i], 0.2f, Color.red, 0, 100, DebugAreas.Movement);
			Vector3 pointA = points[i - 1];
			Vector3 pointB = points[i];
			Ray ray = new Ray(pointA, pointB - pointA);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Vector3.Distance(pointA, pointB)))
			{
				didHit = true;
				GameCharacter.PossibleGround = new PredictedLandingPoint(hit);
				break;
			}
		}
		if (!didHit) GameCharacter.PossibleGround = null;
	}

	public override void LateExecuteState(float deltaTime)
	{

	}

	public override void EndState(GameCharacterState newState)
	{
		GameCharacter.PossibleGround = null;
	}
}
