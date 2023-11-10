using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


public abstract class AGameCharacterState : IState<EGameCharacterState>
{
	GameCharacterStateMachine stateMachine;
	GameCharacter gameCharacter;
	public GameCharacterStateMachine StateMachine { get { return stateMachine; } }
	public GameCharacter GameCharacter { get { return gameCharacter; } }
	public const EGameCharacterState gameCharacterState = EGameCharacterState.Unknown;

	public AGameCharacterState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter)
	{
		this.stateMachine = stateMachine;
		this.gameCharacter = gameCharacter;
	}


	public abstract void EndState(EGameCharacterState newState);
	public abstract void ExecuteState(float deltaTime);
	public abstract void FixedExecuteState(float deltaTime);
	public abstract EGameCharacterState GetStateType();
	public abstract void LateExecuteState(float deltaTime);
	public abstract void StartState(EGameCharacterState oldState);
	public abstract EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest);


	public void SetSlopStrenghToZero(EGameCharacterState oldState)
	{
		switch (oldState)
		{
			default: break;
		}
	}

	protected void RotateCharacter(Quaternion newDir)
	{
		Quaternion targetRot = Quaternion.Slerp(GameCharacter.transform.rotation, newDir, Time.deltaTime * GameCharacter.GameCharacterData.RoationSpeed);
		GameCharacter.transform.rotation = targetRot;
	}

	protected void CombatMovement(float deltaTime, float initYVelocity, float initXVelocity, ref float lerpTimeY, ref float lerpTimeX, ref float currentYPosAnimCurve, bool isGrounded)
	{
		bool isValidHit;
		RaycastHit validHit;
		gameCharacter.MovementComponent.CheckIfACharacterIsToCloseToMoveTo(out isValidHit, out validHit);

		float yPosCurve = GameCharacter.AnimController.GetUpMovementCurve;
		float yPosFromAnimCurveDelta = 0;
		if (yPosCurve > 0 && GameCharacter.CombatComponent.CurrentWeapon != null && GameCharacter.CombatComponent.CurrentWeapon.CurrentAction != null)
		{
			float yPosFromAnimCurve = math.remap(0, 1, 0, GameCharacter.CombatComponent.CurrentWeapon.CurrentAction.Action.MaxVerticalMovement(), yPosCurve);
			yPosFromAnimCurveDelta = yPosFromAnimCurve - currentYPosAnimCurve;
			currentYPosAnimCurve = yPosFromAnimCurve;

			// FIX LOWFramerate Scenarios
			float deltaTimeScale = 1f / Time.deltaTime;
			yPosFromAnimCurveDelta *= deltaTimeScale;
		}

		lerpTimeY += deltaTime * (GameCharacter.MovementComponent.IsGrounded ? GameCharacter.GameCharacterData.GroundToZeroVelYInAttackSpeed : GameCharacter.GameCharacterData.AirToZeroVelYInAttackSpeed);
		lerpTimeX += deltaTime * (GameCharacter.MovementComponent.IsGrounded ? GameCharacter.GameCharacterData.GroundToZeroVelXInAttackSpeed : GameCharacter.GameCharacterData.AirToZeroVelXInAttackSpeed);
		float yMotion = GameCharacter.MovementComponent.RootmotionVector.y + Mathf.Lerp(initYVelocity, 0, lerpTimeY) + yPosFromAnimCurveDelta;
		float xMotion = isValidHit ? 0 : Mathf.Clamp(GameCharacter.MovementComponent.RootmotionVector.x + Mathf.Lerp(initXVelocity, 0, lerpTimeX), -GameCharacter.GameCharacterData.MaxMovementSpeed, GameCharacter.GameCharacterData.MaxMovementSpeed);
		Vector3 rootmotionVector = new Vector3(xMotion, yMotion, 0);

		// Move Along Ground
		if (GameCharacter.MovementComponent.IsGrounded && yPosCurve <= 0)
		{
			Vector2 newInputVector = Vector3.ProjectOnPlane(rootmotionVector, GameCharacter.MovementComponent.RayCastGroundHit != null ? GameCharacter.MovementComponent.RayCastGroundHit.hit.normal : GameCharacter.MovementComponent.PossibleGround.hit.normal);
			if (Mathf.Abs(newInputVector.normalized.x) > 0.1f) rootmotionVector = newInputVector;
		}

		GameCharacter.MovementComponent.MovementVelocity = rootmotionVector;
	}

	protected void OnGroundMovement()
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

	protected void InAirMovement()
	{
		RaycastHit hit;
		bool groundBelowCharacter = GameCharacter.MovementComponent.IsGroundedCheck(GameCharacter.MovementComponent.CharacterCenter, out hit);

		// MaybeAir Speed?
		float maxSpeed = GameCharacter.GameCharacterData.MaxMovementSpeed;

		Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
		Vector3 inputDir = new Vector3(GameCharacter.MovementInput.x, 0f, 0f);
		Vector3 targetVelocity = inputDir.normalized * maxSpeed;
		Vector3 velocityDiff = (targetVelocity - velocity);
		Vector3 acceleration = velocityDiff * GameCharacter.GameCharacterData.InAirControll;
		velocity.x += acceleration.x;
		if (groundBelowCharacter /*&& GameCharacter.MovementComponent.PossibleGround != null*/) velocity = Vector3.ProjectOnPlane(velocity, hit.normal);

		// Anwenden der Geschwindigkeit
		velocity = new Vector3(velocity.x, GameCharacter.MovementComponent.MovementVelocity.y, GameCharacter.MovementComponent.MovementVelocity.z);
		GameCharacter.MovementComponent.MovementVelocity = velocity;
	}
}
