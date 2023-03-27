using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterAttackRecoveryState : AGameCharacterState
{
	public GameCharacterAttackRecoveryState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
	
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
			default: break;
		}

		if (!GameCharacter.IsGrounded || GameCharacter.IsInJump)
			return EGameCharacterState.InAir;

		if (GameCharacter.GetPossibleGroundAngle() > GameCharacter.CharacterController.slopeLimit)
			return EGameCharacterState.Sliding;

		if (GameCharacter.Veloctiy.magnitude > 0 || GameCharacter.GetHorizontalMovementInputDir().magnitude > 0)
			return EGameCharacterState.Moving;

		if (GameCharacter.CombatComponent.AttackTimer.IsFinished)
			return EGameCharacterState.Standing;

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
	
	}
	
	public override void FixedExecuteState(float deltaTime)
	{
	
	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(EGameCharacterState newState)
	{
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.AnimController.InterpRotationLayerWeight(1, 10f);
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(1, 10f);
	}
}