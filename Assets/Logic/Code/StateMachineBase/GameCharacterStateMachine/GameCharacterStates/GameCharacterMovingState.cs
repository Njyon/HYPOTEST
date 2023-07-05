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
		switch (newStateRequest)
		{
			case EGameCharacterState.Attack: return EGameCharacterState.Attack;
			case EGameCharacterState.HookedToCharacter: return EGameCharacterState.HookedToCharacter;
			default: break;
		}

		if (GameCharacter.CheckIfCharacterIsInAir())
			return EGameCharacterState.InAir;

		if (GameCharacter.CheckIfCharacterIsOnSteepGround())
			return EGameCharacterState.InAir;

		if (GameCharacter.CheckIfCharacterIsStanding())
			return EGameCharacterState.Standing;

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		Vector2 inputVector = GameCharacter.MovementInput;
		inputVector.y = 0;

		if (GameCharacter.MovementComponent.IsGrounded)
		{
			Vector2 newInputVector = Vector3.ProjectOnPlane(inputVector, GameCharacter.MovementComponent.RayCastGroundHit != null ? GameCharacter.MovementComponent.RayCastGroundHit.hit.normal : GameCharacter.MovementComponent.PossibleGround.hit.normal);
			if (Mathf.Abs(newInputVector.normalized.x) > 0.1f) inputVector = newInputVector;
		}

		float maxSpeed = GameCharacter.GameCharacterData.MaxMovementSpeed;
		float acceleration = GameCharacter.GameCharacterData.Acceleration;

		Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
		Vector3 targetVelocity = inputVector.normalized * maxSpeed;

		if (inputVector.magnitude > 0 /*&& !FutureInclineToHigh(velocity)*/)
		{
			// Beschleunigung
			Vector3 deltaV = targetVelocity - velocity;
			deltaV = Vector3.ClampMagnitude(deltaV, acceleration);
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, deltaV, 10, Color.black, 0f, 200, DebugAreas.Movement);
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, targetVelocity, 10, Color.green, 0f, 200, DebugAreas.Movement);
			velocity += deltaV;
			velocity = targetVelocity.normalized * velocity.magnitude;
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, velocity, 10, Color.white, 0f, 200, DebugAreas.Movement);
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
				float drag = GameCharacter.GameCharacterData.Drag;
				float deceleration = drag;
				velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration);
			}
		}

		// Anwenden der Geschwindigkeit
		velocity = new Vector3(velocity.x, velocity.y, GameCharacter.MovementComponent.MovementVelocity.z);
		GameCharacter.MovementComponent.MovementVelocity = velocity;
	}

	private bool FutureInclineToHigh(Vector3 velocity)
	{
		// calculate Ray height
		Vector3 rayOrigin = GameCharacter.transform.position + Vector3.up * (GameCharacter.MovementComponent.Height + 1);
		Vector3 velDir = new Vector3(velocity.x, 0, 0).normalized;
		// calculate Ray forwad position
		rayOrigin = rayOrigin + velDir * 0.6f;
		Debug.DrawRay(rayOrigin, Vector3.down * (GameCharacter.MovementComponent.Height + 2f), Color.red);
		Ray ray = new Ray(rayOrigin, Vector3.down);
		RaycastHit firstHit;
		if (Physics.Raycast(ray, out firstHit, GameCharacter.MovementComponent.Height + 2f))
		{
			rayOrigin = rayOrigin + Vector3.up * 0.5f;
			rayOrigin = rayOrigin + velDir * 0.2f;
			Debug.DrawRay(rayOrigin, Vector3.down * (GameCharacter.MovementComponent.Height + 2f), Color.blue);
			Ray ray2 = new Ray(rayOrigin, Vector3.down);
			RaycastHit secondHit;
			if (Physics.Raycast(ray2, out secondHit, GameCharacter.MovementComponent.Height + 2f))
			{
				if (Vector3.Angle(firstHit.normal, Vector3.up) >= GameCharacter.GameCharacterData.MaxSlopAngle && Ultra.Utilities.IsNearlyEqual(Vector3.Angle(firstHit.normal, Vector3.up), Vector3.Angle(secondHit.normal, Vector3.up), 0.02f))
				{
					return true;
				}
			}
			Vector3 stepheightCheckOrigin = GameCharacter.MovementComponent.PossibleGround.hit.point + Vector3.up * (GameCharacter.MovementComponent.StepHeight + 0.001f);
			Ray ray3 = new Ray(stepheightCheckOrigin, velDir);
			RaycastHit thirdHit;
			Debug.DrawRay(stepheightCheckOrigin, velDir * (GameCharacter.MovementComponent.Radius + 0.3f), Color.green);
			if (Physics.Raycast(ray3, out thirdHit, GameCharacter.MovementComponent.Radius + 0.3f))
			{
				if (Vector3.Angle(firstHit.normal, Vector3.up) >= GameCharacter.GameCharacterData.MaxSlopAngle && Vector3.Angle(thirdHit.normal, Vector3.up) >= GameCharacter.GameCharacterData.MaxSlopAngle)
				{
					return true;
				}
				if (GameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(Vector3.Distance(firstHit.point, thirdHit.point).ToString(), 10f);
				if (firstHit.point.y < thirdHit.point.y && Vector3.Distance(firstHit.point, thirdHit.point) > 0.9f)
				{
					return true;
				}
			}
		}
		return false;
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
