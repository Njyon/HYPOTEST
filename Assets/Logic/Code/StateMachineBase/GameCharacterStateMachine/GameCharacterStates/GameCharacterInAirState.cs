using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCharacterMovementComponent;

public class GameCharacterInAirState : AGameCharacterState
{
	public GameCharacterInAirState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(EGameCharacterState oldState)
	{
		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.InAir;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		switch (newStateRequest)
		{
			case EGameCharacterState.Attack: return EGameCharacterState.Attack;
			case EGameCharacterState.HookedToCharacter: return EGameCharacterState.HookedToCharacter;
			case EGameCharacterState.PullCharacterOnHorizontalLevel: return EGameCharacterState.PullCharacterOnHorizontalLevel;
			default: break;
		}

		if (GameCharacter.MovementComponent.GetPossibleGroundAngle() < GameCharacter.MovementComponent.SlopeLimit)
		{
			if (GameCharacter.MovementComponent.IsGrounded && GameCharacter.CheckIfCharacterIsMoving())
				return EGameCharacterState.Moving;
			else if (GameCharacter.MovementComponent.IsGrounded && GameCharacter.CheckIfCharacterIsStanding())
				return EGameCharacterState.Standing;
		}

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		RaycastHit hit;
		bool groundBelowCharacter = GameCharacter.MovementComponent.IsGroundedCheck(GameCharacter.MovementComponent.CharacterCenter, out hit);

		// MaybeAir Speed?
		float maxSpeed = GameCharacter.GameCharacterData.MaxMovementSpeed;

		Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
		Vector3 inputDir = new Vector3(GameCharacter.MovementInput.x, 0f, 0f);
		Vector3 targetVelocity = inputDir.normalized * maxSpeed;
		Vector3 velocityDiff = (targetVelocity - velocity);
		Vector3 acceleration = Vector3.ClampMagnitude(velocityDiff, maxSpeed) * GameCharacter.GameCharacterData.InAirControll;
		velocity += acceleration;
		if (groundBelowCharacter && GameCharacter.MovementComponent.PossibleGround != null) velocity = Vector3.ProjectOnPlane(velocity, GameCharacter.MovementComponent.PossibleGround.hit.normal);

		// Anwenden der Geschwindigkeit
		velocity = new Vector3(velocity.x, GameCharacter.MovementComponent.MovementVelocity.y, GameCharacter.MovementComponent.MovementVelocity.z);
		GameCharacter.MovementComponent.MovementVelocity = velocity;
	}

	public override void FixedExecuteState(float deltaTime)
	{
		Vector3[] points = Ultra.Utilities.CalculateTrijactoryPoints(5, 0.3f, GameCharacter.transform.position, GameCharacter.MovementComponent.MovementVelocity, Physics.gravity * 7);
		bool didHit = false;
		for (int i = 1; i < points.Length; i++)
		{
			Ultra.Utilities.DrawWireSphere(points[i], 0.2f, Color.red, 0, 100, DebugAreas.Movement);
			Vector3 pointA = points[i - 1];
			Vector3 pointB = points[i];
			Ray ray = new Ray(pointA, pointB - pointA);
			RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(pointA, pointB));
			foreach(RaycastHit hit in hits)
			{
				// Ignore self hit
				if (GameCharacter.gameObject == hit.collider.gameObject) continue;
				didHit = true;
				GameCharacter.MovementComponent.PossibleGround = new NullableHit(hit);
				break;
			}
		}
		if (!didHit) GameCharacter.MovementComponent.PossibleGround = null;
	}

	public override void LateExecuteState(float deltaTime)
	{

	}

	public override void EndState(EGameCharacterState newState)
	{
		GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;
		switch (newState)
		{
			case EGameCharacterState.Attack: break;
			default:
				Vector3 test = new Vector3(GameCharacter.MovementComponent.MovementVelocity.x, Physics.gravity.y / 2, GameCharacter.MovementComponent.MovementVelocity.z);
				GameCharacter.MovementComponent.MovementVelocity = test; 
				break;	
		}
	
		GameCharacter.MovementComponent.IsInJump = false;
	}


	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		// Remove Velocity in Y Axis when hit roof
		if ((collisionFlag & CollisionFlags.Above) != 0)
		{
			GameCharacter.MovementComponent.MovementVelocity = new Vector3(GameCharacter.MovementComponent.MovementVelocity.x, GameCharacter.MovementComponent.HeadBounceValue, GameCharacter.MovementComponent.MovementVelocity.y);
		}
	}
}
