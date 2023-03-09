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
		if (!GameCharacter.IsGrounded || GameCharacter.IsInJump)
			return EGameCharacterState.InAir;

		if (GameCharacter.GetPossibleGroundAngle() > GameCharacter.CharacterController.slopeLimit)
			return EGameCharacterState.Sliding;

		if (GameCharacter.Veloctiy.magnitude <= 0 && GameCharacter.GetHorizontalMovementInputDir().magnitude <= 0)
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
		Vector3 targetVelocity = inputVector * maxSpeed;

		if (inputVector.magnitude > 0 && !FutureInclineToHigh(velocity))
		{
			// Beschleunigung
			Vector3 deltaV = targetVelocity - velocity;
			deltaV = Vector3.ClampMagnitude(deltaV, acceleration);
			//targetVelocity = Vector3.ClampMagnitude(targetVelocity, deltaV.magnitude);
			//Ultra.Utilities.DrawArrow(GameCharacter.transform.position, deltaV, 10, Color.black);
			//Ultra.Utilities.DrawArrow(GameCharacter.transform.position, targetVelocity, 10, Color.green);
			//Ultra.Utilities.DrawArrow(GameCharacter.transform.position, velocity, 10, Color.white);
			velocity += deltaV;
		}
		else
		{
			// Bremsen
			if (Ultra.Utilities.IsNearlyEqual(velocity, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f)))
			{
				velocity = Vector3.zero;
			} 
			else
			{
				float drag = GameCharacter.CharacterData.Drag;
				float deceleration = drag;
				velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration);
			}
		}

		// Anwenden der Geschwindigkeit
		velocity = new Vector3(velocity.x, velocity.y, GameCharacter.MovementVelocity.z);
		GameCharacter.MovementVelocity = velocity;
	}

	private bool FutureInclineToHigh(Vector3 velocity)
	{
		// calculate Ray height
		Vector3 rayOrigin = GameCharacter.transform.position + Vector3.up * (GameCharacter.CharacterController.height + 1);
		Vector3 velDir = new Vector3(velocity.x, 0, 0).normalized;
		// calculate Ray forwad position
		rayOrigin = rayOrigin + velDir * 0.6f;
		Debug.DrawRay(rayOrigin, Vector3.down * (GameCharacter.CharacterController.height + 2f), Color.red);
		Ray ray = new Ray(rayOrigin, Vector3.down);
		RaycastHit firstHit;
		if (Physics.Raycast(ray, out firstHit, GameCharacter.CharacterController.height + 2f))
		{
			rayOrigin = rayOrigin + Vector3.up * 0.5f;
			rayOrigin = rayOrigin + velDir * 0.2f;
			Debug.DrawRay(rayOrigin, Vector3.down * (GameCharacter.CharacterController.height + 2f), Color.blue);
			Ray ray2 = new Ray(rayOrigin, Vector3.down);
			RaycastHit secondHit;
			if (Physics.Raycast(ray2, out secondHit, GameCharacter.CharacterController.height + 2f))
			{
				if (Vector3.Angle(firstHit.normal, Vector3.up) >= GameCharacter.CharacterData.MaxSlopAngle && Ultra.Utilities.IsNearlyEqual(Vector3.Angle(firstHit.normal, Vector3.up), Vector3.Angle(secondHit.normal, Vector3.up), 0.02f))
				{
					SetSlopStrenghToZero();
					return true;
				}
			}
			Vector3 stepheightCheckOrigin = GameCharacter.PossibleGround.hit.point + Vector3.up * (GameCharacter.CharacterController.stepOffset + 0.001f);
			Ray ray3 = new Ray(stepheightCheckOrigin, velDir);
			RaycastHit thirdHit;
			Debug.DrawRay(stepheightCheckOrigin, velDir * (GameCharacter.CharacterController.radius + 0.3f), Color.green);
			if (Physics.Raycast(ray3, out thirdHit, GameCharacter.CharacterController.radius + 0.3f))
			{
				if (Vector3.Angle(firstHit.normal, Vector3.up) >= GameCharacter.CharacterData.MaxSlopAngle && Vector3.Angle(thirdHit.normal, Vector3.up) >= GameCharacter.CharacterData.MaxSlopAngle)
				{
					SetSlopStrenghToZero();
					return true;
				}
				Ultra.Utilities.Instance.DebugLogOnScreen(Vector3.Distance(firstHit.point, thirdHit.point).ToString());
				if (Vector3.Distance(firstHit.point, thirdHit.point) > 0.9f)
				{
					SetSlopStrenghToZero();
					return true;
				}
			}
		}
		return false;
	}

	private void SetSlopStrenghToZero()
	{
		GameCharacter.SlopStrengh = 0f;
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
