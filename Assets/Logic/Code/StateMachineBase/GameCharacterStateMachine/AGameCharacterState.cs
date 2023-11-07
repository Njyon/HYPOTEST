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

	protected void CombatMovement(float deltaTime, float initYVelocity, float initXVelocity, ref float lerpTimeY, ref float lerpTimeX, ref float currentYPosAnimCurve)
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


		lerpTimeY += deltaTime * GameCharacter.GameCharacterData.AirToZeroVelYInAttackSpeed;
		lerpTimeX += deltaTime * GameCharacter.GameCharacterData.AirToZeroVelXInAttackSpeed;
		float yMotion = GameCharacter.MovementComponent.RootmotionVector.y + Mathf.Lerp(initYVelocity, 0, lerpTimeY) + yPosFromAnimCurveDelta;
		float xMotion = isValidHit ? 0 : GameCharacter.MovementComponent.RootmotionVector.x + Mathf.Lerp(initXVelocity, 0, lerpTimeX);
		Vector3 rootmotionVector = new Vector3(xMotion, yMotion, 0);

		// Move Along Ground
		if (GameCharacter.MovementComponent.IsGrounded && yPosCurve <= 0)
		{
			Vector2 newInputVector = Vector3.ProjectOnPlane(rootmotionVector, GameCharacter.MovementComponent.RayCastGroundHit != null ? GameCharacter.MovementComponent.RayCastGroundHit.hit.normal : GameCharacter.MovementComponent.PossibleGround.hit.normal);
			if (Mathf.Abs(newInputVector.normalized.x) > 0.1f) rootmotionVector = newInputVector;
		}

		GameCharacter.MovementComponent.MovementVelocity = rootmotionVector;
	}
}
