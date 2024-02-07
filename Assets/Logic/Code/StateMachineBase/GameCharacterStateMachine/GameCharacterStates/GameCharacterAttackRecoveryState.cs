using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterAttackRecoveryState : AGameCharacterState
{
	float lerpTimeY;
	float lerpTimeX;
	float currentYPosAnimCurve;
	Vector3 initVelocity;
	public GameCharacterAttackRecoveryState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		initVelocity = GameCharacter.MovementComponent.MovementVelocity;
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.AttackRecovery;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		switch (newStateRequest)
		{
			case EGameCharacterState.Attack: return EGameCharacterState.Attack;
			case EGameCharacterState.HookedToCharacter: return EGameCharacterState.HookedToCharacter;
			case EGameCharacterState.PullCharacterOnHorizontalLevel: return EGameCharacterState.PullCharacterOnHorizontalLevel;
			case EGameCharacterState.DefensiveAction: return EGameCharacterState.DefensiveAction;
			case EGameCharacterState.FlyAway: return EGameCharacterState.FlyAway;
			case EGameCharacterState.InAir: return EGameCharacterState.InAir;
			case EGameCharacterState.Dodge: return EGameCharacterState.Dodge;
			case EGameCharacterState.Freez: return EGameCharacterState.Freez;
			default: break;
		}


		if (GameCharacter.MovementComponent.IsInJump)
			return GameCharacter.GetBestCharacterState();

		if (GameCharacter.CombatComponent.AttackTimer.IsFinished)
		{
			return GameCharacter.GetBestCharacterState();
		}

		if (GameCharacter.CombatComponent.AllowEarlyLeaveAttackRecovery)
		{
			if (GameCharacter.MovementComponent.IsInJump)
				return EGameCharacterState.InAir;
			
			if (GameCharacter.GetHorizontalMovementInputDir().magnitude > 0)
				return EGameCharacterState.Moving;

			if (!GameCharacter.AnimController.IsInValidAttackState() && !GameCharacter.AnimController.IsInValid3BlendAimState() && !GameCharacter.AnimController.IsInValid3BlendAttackState() && !GameCharacter.AnimController.IsInValidAttackTriggerState() && !GameCharacter.AnimController.IsInValidAttackHoldState())
			{
				return EGameCharacterState.Standing;
			}
		}
		

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		GameCharacter.CombatComponent.CurrentWeapon.PreAttackStateLogic(deltaTime);

		if (GameCharacter.MovementComponent.IsGrounded) {
			//OnGroundMovement();
			CombatMovement(deltaTime, initVelocity.y, initVelocity.x, ref lerpTimeY, ref lerpTimeX, ref currentYPosAnimCurve, GameCharacter.MovementComponent.IsGrounded);
		} else {
			InAirMovement();
			//CombatMovement(deltaTime, GameCharacter.MovementComponent.CalculateGravity() /*initYVelocity*/, GameCharacter.MovementComponent.MovementVelocity.x /*initXVelocity*/, ref lerpTimeY, ref lerpTimeX, ref currentYPosAnimCurve, false);
		}


		GameCharacter.CombatComponent.CurrentWeapon.PostAttackStateLogic(deltaTime);
	}
	
	public override void FixedExecuteState(float deltaTime)
	{
	
	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(EGameCharacterState newState)
	{
		switch (newState)
		{
			case EGameCharacterState.Attack:
				// Force Remove Hit Detection so new Hit can be processed from NextAttack
				GameCharacter.CombatComponent.CurrentWeapon.HitDetectionEnd();
				break;
			default:
				if (!GameCharacter.MovementComponent.IsGrounded)
					GameCharacter.BuffComponent.AddBuff(new NoGravityBuff(this.GameCharacter, Ultra.HypoUttilies.GameMode.GetDefaultGameModeData().AfterCombatNoGravityTime));
				break;
		}

		GameCharacter.AnimController.InAttack = false;
		GameCharacter.AnimController.HoldAttack = false;
		GameCharacter.AnimController.TriggerAttack = false;
		GameCharacter.AnimController.InDefensiveAction = false;
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(1, 10);
		GameCharacter.CombatComponent.CurrentWeapon.AttackRecoveryEnd();
		GameCharacter.AnimController.BlockRotation = false;
	}
}