using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterMovingState : AGameCharacterState
{
	public GameCharacterMovingState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base(stateMachine, gameCharacter)
	{ }

	public override void StartState(EGameCharacterState oldState)
	{
		SetSlopStrenghToZero(oldState);
		GameCharacter.AnimController.SetSecondaryMotionLayerWeight(0);

		GameCharacter.CombatComponent.CurrentWeapon.SetWeaponReadyPoseBasedOnStates();
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
			case EGameCharacterState.PullCharacterOnHorizontalLevel: return EGameCharacterState.PullCharacterOnHorizontalLevel;
			case EGameCharacterState.DefensiveAction: return EGameCharacterState.DefensiveAction;
			case EGameCharacterState.MoveToPosition: return EGameCharacterState.MoveToPosition;
			case EGameCharacterState.FlyAway: return EGameCharacterState.FlyAway;
			case EGameCharacterState.Dodge: return EGameCharacterState.Dodge;
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
		OnGroundMovement();
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
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(1, 10f);

		GameCharacter.CombatComponent.CurrentWeapon.SetWeaponReadyPose();
	}
}
