using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterFlyAwayState : AGameCharacterState
{
	float slowDown = 0.1f;
	public GameCharacterFlyAwayState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		RotateCharacter();

		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;

		GameCharacter.CombatComponent.FlyAwayTimer.onTimerFinished += OnTimerFinished;
		GameCharacter.CombatComponent.FlyAwayTimer.Start(GameCharacter.CombatComponent.FlyAwayTime);
		GameCharacter.MovementComponent.StopGravityInterp();
		GameCharacter.MovementComponent.VariableGravityMultiplierOverTime = 1;

		GameCharacter.AnimController.ResetAnimStatesHARD();

		GameCharacter.AnimController.FlyAway = true;
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.FlyAway;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (newStateRequest != EGameCharacterState.Unknown) return newStateRequest;
		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;

		// MaybeAir Speed?
		float maxSpeed = GameCharacter.GameCharacterData.MaxMovementSpeed * 2;
		Vector3 targetVelocity = velocity.normalized * (maxSpeed / 2);
		Vector3 velocityDiff = (targetVelocity - velocity);
		Vector3 acceleration = velocityDiff * slowDown * deltaTime;
		velocity.x += acceleration.x;


		velocity = new Vector3(velocity.x, velocity.y, GameCharacter.MovementComponent.MovementVelocity.z);

		//Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, velocity.normalized, 1f, Color.red, 10f, 100, DebugAreas.Combat);
		GameCharacter.MovementComponent.MovementVelocity = velocity;

		RotateCharacter();
	}

	public override void FixedExecuteState(float deltaTime)
	{
	
	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(EGameCharacterState newState)
	{
		GameCharacter.CombatComponent.FlyAwayTimer.onTimerFinished -= OnTimerFinished;
		GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;

		// Rotate Chracter to normal position
		Vector3 normalDir = new Vector3(GameCharacter.transform.forward.x, 0f, 0f);
		GameCharacter.transform.rotation = Quaternion.LookRotation(normalDir.normalized, Vector3.up);

		GameCharacter.AnimController.FlyAway = false;
	}

	private void RotateCharacter()
	{
		Vector3 invertedMoveDir = GameCharacter.MovementComponent.MovementVelocity.normalized * -1;
		invertedMoveDir = new Vector3(invertedMoveDir.x, 0f, 0f);
		if (invertedMoveDir == Vector3.zero) return;
		GameCharacter.transform.rotation = Quaternion.LookRotation(invertedMoveDir.normalized, Vector3.up);
		SetFlyAwayDir();
	}

	void SetFlyAwayDir()
	{
		float dot = Vector3.Dot(GameCharacter.MovementComponent.MovementVelocity.normalized, Vector3.up);
		GameCharacter.AnimController.FlyAwayDir = dot;
	}

	void OnTimerFinished()
	{
		GameCharacter.RequestBestCharacterState();
	}

	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		if (collisionFlag == CollisionFlags.None) return;
		
		Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
		RaycastHit hit;
		bool groundBelowCharacter = GameCharacter.MovementComponent.CheckCharacterCapsulInDirection(GameCharacter.MovementComponent.CharacterCenter, GameCharacter.MovementComponent.MovementVelocity, out hit);

		//Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, velocity.normalized, 2f, Color.magenta, 10f, 100, DebugAreas.Combat);
		if (groundBelowCharacter) velocity = Vector3.Reflect(velocity, hit.normal);
		//Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, velocity.normalized, 2f, Color.green, 10f, 100, DebugAreas.Combat);

		velocity = velocity * 0.5f;

		velocity = new Vector3(velocity.x, velocity.y, GameCharacter.MovementComponent.MovementVelocity.z);
		GameCharacter.MovementComponent.MovementVelocity = velocity;
	}
}