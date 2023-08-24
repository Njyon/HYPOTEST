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
	Ultra.Timer backupTimer = null;
	public GameCharacterAttackState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{
		backupTimer = new Ultra.Timer();
		backupTimer.onTimerFinished += OnBackupTimerFinished;
	}

    public override void StartState(EGameCharacterState oldState)
	{
		backupTimer.Start(5f);

		switch(GameCharacter.CombatComponent.CurrentWeapon.AttackAnimType)
		{
			case EAttackAnimType.Combat3Blend: 
				GameCharacter.AnimController.InCombat3Blend = true;
				GameCharacter.CombatComponent.AttackTimer.Start(GameCharacter.CombatComponent.CurrentWeapon.CurrentAttack.aimBlendTypes.blendAnimations.midAnimation.length); 
				break;
			case EAttackAnimType.AimBlendSpace: 
				GameCharacter.AnimController.InAimBlendTree = true;
				GameCharacter.CombatComponent.AttackTimer.Start(GameCharacter.CombatComponent.CurrentWeapon.CurrentAttack.aimBlendTypes.blendAnimations.midAnimation.length); 
				break;
			default:
				GameCharacter.AnimController.InAttack = true;
				GameCharacter.CombatComponent.AttackTimer.Start(GameCharacter.CombatComponent.CurrentWeapon.CurrentAttack.clip.length); 
				break;
		}
		GameCharacter.MovementComponent.UseGravity = false;
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(0, 10f);
		initYVelocity = GameCharacter.MovementComponent.MovementVelocity.y;
		initXVelocity = GameCharacter.MovementComponent.MovementVelocity.x;
		lerpTimeY = 0;
		lerpTimeX = 0;
		GameCharacter.AnimController.BlockRotation = true;

		GameCharacter.AnimController.RotationTrarget = 0f;
		if (GameCharacter.MovementInput.x != 0)
		{
			Vector3 currentDir = new Vector3(GameCharacter.MovementInput.x, 0, 0);
			newDir = Quaternion.LookRotation(currentDir.normalized, Vector3.up);
		}else
		{
			Vector3 bounds = new Vector3(4f, 1.5f, 1f);
			GameCharacter target = Ultra.HypoUttilies.FindCharactereInDirectionInRange(GameCharacter.MovementComponent.CharacterCenter, GameCharacter.transform.forward, bounds, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
			Ultra.Utilities.DrawBox(GameCharacter.MovementComponent.CharacterCenter, Quaternion.identity, bounds, Color.blue, 10f, 200, DebugAreas.Combat);
			if (target != null)
			{
				Vector3 targetDir = target.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter;
				targetDir = Ultra.Utilities.IgnoreAxis(targetDir, EAxis.YZ);
				newDir = Quaternion.LookRotation(targetDir.normalized, Vector3.up);
			}else
			{
				Vector3 currentDir = new Vector3(GameCharacter.transform.forward.x, 0, 0);
				newDir = Quaternion.LookRotation(currentDir.normalized, Vector3.up);
			}
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
			case EGameCharacterState.Freez: return EGameCharacterState.Freez;
			default: break;
		}

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		backupTimer.Update(deltaTime);

		GameCharacter.CombatComponent.CurrentWeapon.PreAttackStateLogic(deltaTime);
		RotateCharacter(newDir);

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
		GameCharacter.transform.rotation = newDir;
		GameCharacter.LastDir = new Vector3(GameCharacter.transform.forward.x, 0, 0); 
		GameCharacter.CombatComponent.CurrentWeapon.EndAttackStateLogic();
	}

	void OnBackupTimerFinished()
	{
		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.AttackRecovery);
	}
}