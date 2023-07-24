using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameCharacterAttackState : AGameCharacterState
{
	float initYVelocity;
	float initXVelocity;
	float lerpTimeY;
	float lerpTimeX;
	float currentYPosAnimCurve;
	Quaternion newDir;
	public GameCharacterAttackState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		GameCharacter.AnimController.InAttack = true;
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(0, 10f);
		initYVelocity = GameCharacter.MovementComponent.MovementVelocity.y;
		initXVelocity = GameCharacter.MovementComponent.MovementVelocity.x;
		lerpTimeY = 0;
		lerpTimeX = 0;

		GameCharacter.AnimController.RotationTrarget = 0f;
		if (GameCharacter.MovementInput.x != 0)
		{
			Vector3 currentDir = new Vector3(GameCharacter.MovementInput.x, 0, 0);
			newDir = Quaternion.LookRotation(currentDir.normalized, Vector3.up);
		}else
		{
			newDir = GameCharacter.RotationTarget;
		}
		GameCharacter.CombatComponent.CurrentWeapon.StartAttackStateLogic();
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Attack;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		switch (newStateRequest)
		{
 			case EGameCharacterState.AttackRecovery: return EGameCharacterState.AttackRecovery;
			case EGameCharacterState.HookedToCharacter: return EGameCharacterState.HookedToCharacter;
			case EGameCharacterState.PullCharacterOnHorizontalLevel: return EGameCharacterState.PullCharacterOnHorizontalLevel;
			default: break;
		}

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		GameCharacter.CombatComponent.CurrentWeapon.PreAttackStateLogic(deltaTime);
		RotateCharacter();

		float yPosCurve = GameCharacter.AnimController.GetUpMovementCurve;
		float yPosFromAnimCurve = math.remap(0, 1, 0, GameCharacter.CombatComponent.CurrentWeapon.LastAttack.maxVerticalMovement, yPosCurve);
		float yPosFromAnimCurveDelta = yPosFromAnimCurve - currentYPosAnimCurve;
		currentYPosAnimCurve = yPosFromAnimCurve;

		// FIX LOWFramerate Scenarios
		float deltaTimeScale = 1f / Time.deltaTime;
		yPosFromAnimCurveDelta *= deltaTimeScale;

		lerpTimeY += deltaTime * GameCharacter.GameCharacterData.AirToZeroVelYInAttackSpeed;
		lerpTimeX += deltaTime * GameCharacter.GameCharacterData.AirToZeroVelXInAttackSpeed;
		float yMotion = GameCharacter.MovementComponent.RootmotionVector.y + Mathf.Lerp(initYVelocity, 0, lerpTimeY) + yPosFromAnimCurveDelta;
		float xMotion = GameCharacter.MovementComponent.RootmotionVector.x + Mathf.Lerp(initXVelocity, 0, lerpTimeX);
		Vector3 rootmotionVector = new Vector3(xMotion, yMotion, 0);

		// Move Along Ground
		if (GameCharacter.MovementComponent.IsGrounded)
		{
			Vector2 newInputVector = Vector3.ProjectOnPlane(rootmotionVector, GameCharacter.MovementComponent.RayCastGroundHit != null ? GameCharacter.MovementComponent.RayCastGroundHit.hit.normal : GameCharacter.MovementComponent.PossibleGround.hit.normal);
			if (Mathf.Abs(newInputVector.normalized.x) > 0.1f) rootmotionVector = newInputVector;
		}

		GameCharacter.MovementComponent.MovementVelocity = rootmotionVector;
		GameCharacter.CombatComponent.CurrentWeapon.PostAttackStateLogic(deltaTime);
	}

	private void RotateCharacter()
	{
		Quaternion targetRot = Quaternion.Slerp(GameCharacter.transform.rotation, newDir, Time.deltaTime * GameCharacter.GameCharacterData.RoationSpeed);
		GameCharacter.transform.rotation = targetRot;
	}

	public override void FixedExecuteState(float deltaTime)
	{
	
	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(EGameCharacterState newState)
	{
		GameCharacter.transform.rotation = newDir;
		GameCharacter.CombatComponent.CurrentWeapon.EndAttackStateLogic();
	}
}