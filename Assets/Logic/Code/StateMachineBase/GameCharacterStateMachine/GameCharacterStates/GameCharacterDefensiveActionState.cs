using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GameCharacterDefensiveActionState : AGameCharacterState
{
	float initYVelocity;
	float initXVelocity;
	float lerpTimeY;
	float lerpTimeX;
	float currentYPosAnimCurve;
	Quaternion newDir;
	public GameCharacterDefensiveActionState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		switch (GameCharacter.CombatComponent.CurrentWeapon.AttackAnimType)
		{
			case EAttackAnimType.Combat3Blend:
			case EAttackAnimType.AimBlendSpace:
				GameCharacter.CombatComponent.DefensiveTimer.Start(GameCharacter.CombatComponent.CurrentWeapon.CurrentDefensiveAction.aimBlendTypes.blendAnimations.midAnimation.length);
				break;
			default:
				GameCharacter.AnimController.InDefensiveAction = true;
				GameCharacter.CombatComponent.DefensiveTimer.Start(GameCharacter.CombatComponent.CurrentWeapon.CurrentDefensiveAction.clip.length);
				break;
		}
		GameCharacter.MovementComponent.UseGravity = false;
		GameCharacter.AnimController.BlockRotation = true;

		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(0, 10f);
		initYVelocity = GameCharacter.MovementComponent.MovementVelocity.y;
		initXVelocity = GameCharacter.MovementComponent.MovementVelocity.x;
		lerpTimeY = 0;
		lerpTimeX = 0;

		GameCharacter.AnimController.RotationTrarget = 0f;
		if (GameCharacter.CombatComponent.HookedCharacters.Count > 0 )
		{
			GameCharacter.transform.rotation = Quaternion.LookRotation(new Vector3((GameCharacter.CombatComponent.HookedCharacters[0].transform.position - GameCharacter.transform.position).x, 0, 0).normalized, Vector3.up);
			GameCharacter.RotationTarget = GameCharacter.transform.rotation;
			GameCharacter.AnimController.RotationTrarget = 0f;
		}
		else
		{
			if (GameCharacter.MovementInput.x != 0)
			{
				Vector3 currentDir = new Vector3(GameCharacter.MovementInput.x, 0, 0);
				newDir = Quaternion.LookRotation(currentDir.normalized, Vector3.up);
			}
			else
			{
				newDir = GameCharacter.RotationTarget;
			}
		}

	
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.DefensiveAction;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (GameCharacter.CombatComponent.CurrentWeapon.CanLeaveDefensiveState())
		{
			if (newStateRequest != EGameCharacterState.Unknown)
				return newStateRequest;
		}

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		GameCharacter.CombatComponent.CurrentWeapon.PreAttackStateLogic(deltaTime);
		//RotateCharacter(newDir);

		CombatMovement(deltaTime, initYVelocity, initXVelocity, ref lerpTimeY, ref lerpTimeX, ref currentYPosAnimCurve);

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
		GameCharacter.MovementComponent.UseGravity = true;
		GameCharacter.AnimController.BlockRotation = false;
		GameCharacter.AnimController.InDefensiveAction = false;
		GameCharacter.LastDir = new Vector3(GameCharacter.transform.forward.x, 0, 0);
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(1);
		GameCharacter.CombatComponent.CurrentWeapon?.DefensiveActionStateEnd();
	}
}