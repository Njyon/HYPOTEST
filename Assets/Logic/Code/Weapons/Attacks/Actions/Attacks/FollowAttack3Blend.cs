using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

[Serializable]
public class FollowAttack3BlendData : AttackData
{
	public AimBlendAnimations blendSpace;
	public float attackRange = 10f;
	public float rotationInterpSpeed = 10f;
	public float attackMovementSpeed = 40f;
}

public class FollowAttack3Blend : ActionBase
{
	[SerializeField] FollowAttack3BlendData attackData;
	GameCharacter targetCharacter;
	Vector3 characterDestinationIfTargetIsSet;
	Vector3 attackPositionIfNoEnemyFound;
	Vector3 targetDir;
	Vector3 startPostion;
	float targetAngle = 0;
	bool atDestination = false;
	float minDistanceCheck = 2f;
	bool move = false;

	public override void StartAction()
	{
		atDestination = false;
		move = true;

		startPostion = GameCharacter.MovementComponent.CharacterCenter;

		targetCharacter = Ultra.HypoUttilies.FindCharactereNearestToDirectionWithRangeWithAngleTreshHold(GameCharacter.MovementComponent.CharacterCenter, (GameCharacter.MovementInput.magnitude <= 0) ? GameCharacter.transform.forward : GameCharacter.MovementInput, attackData.attackRange, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
		if (targetCharacter != null)
		{
			Vector3 dirToPlayer = GameCharacter.MovementComponent.CharacterCenter - targetCharacter.MovementComponent.CharacterCenter;
			characterDestinationIfTargetIsSet = targetCharacter.MovementComponent.CharacterCenter + dirToPlayer.IgnoreAxis(EAxis.YZ).normalized * targetCharacter.GameCharacterData.MinCharacterDistance;
			targetDir = characterDestinationIfTargetIsSet - GameCharacter.MovementComponent.CharacterCenter;
		}
		else
		{
			ActionInterupted();
			return;
			attackPositionIfNoEnemyFound = GameCharacter.MovementComponent.CharacterCenter + GameCharacter.transform.forward * attackData.attackRange;
			targetDir = attackPositionIfNoEnemyFound - GameCharacter.MovementComponent.CharacterCenter;
		}

		Weapon.Attack3BlendSpace(attackData.blendSpace, EAnimationType.Default);
		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Attack);
		Weapon.AttackAnimType = EAttackAnimType.Combat3Blend;
		GameCharacter.AnimController.Combat3BlendDir = 0f;
		GameCharacter.AnimController.InCombat3Blend = true;

		targetAngle = Vector3.Angle(targetDir, GameCharacter.transform.forward);
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		if (!atDestination && move)
		{
			// Check like this to be safe should only be 1 possible at the same time
			bool nearAtTargetCharacter = targetCharacter != null ? Vector3.Distance(startPostion, characterDestinationIfTargetIsSet) <= Vector3.Distance(startPostion, GameCharacter.MovementComponent.CharacterCenter) : false;
			bool arrivedAtTargetLocation = targetCharacter == null ? Vector3.Distance(startPostion, attackPositionIfNoEnemyFound) <= Vector3.Distance(startPostion, GameCharacter.MovementComponent.CharacterCenter) : false;
			if (nearAtTargetCharacter || arrivedAtTargetLocation)
			{
				atDestination = true;
				GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.AttackRecovery);
				ReachedDestination();
			}
			else
			{
				if (targetCharacter != null)
				{
					Vector3 dirToPlayer = GameCharacter.MovementComponent.CharacterCenter - targetCharacter.MovementComponent.CharacterCenter;
					characterDestinationIfTargetIsSet = targetCharacter.MovementComponent.CharacterCenter + dirToPlayer.IgnoreAxis(EAxis.YZ).normalized * targetCharacter.GameCharacterData.MinCharacterDistance;
					targetDir = characterDestinationIfTargetIsSet - GameCharacter.MovementComponent.CharacterCenter;
				} else
					targetDir = attackPositionIfNoEnemyFound - GameCharacter.MovementComponent.CharacterCenter;

				targetAngle = Vector3.Angle(targetDir.normalized, GameCharacter.transform.forward);
				GameCharacter.AnimController.Combat3BlendDir = Mathf.Lerp(GameCharacter.AnimController.Combat3BlendDir, Ultra.Utilities.Remap(targetAngle, -90f, 90f, 1, -1), deltaTime * attackData.rotationInterpSpeed);

				Vector3 vel = targetDir.normalized * attackData.attackMovementSpeed;
				GameCharacter.MovementComponent.MovementVelocity = vel;
			}
		}
	}

	async void ReachedDestination()
	{
		await new WaitForEndOfFrame();
		await new WaitForEndOfFrame();

		if (targetCharacter != null)
		{
			if (!targetCharacter.MovementComponent.IsGrounded)
			{
				GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
				targetCharacter.MovementComponent.MovementVelocity = Vector3.zero;
				targetCharacter.CombatComponent.RequestFreez();
			}
			
			//GameTimeManager.Instance.AddDefaultFreezFrame();
			CameraController.Instance.ShakeCamerea(0);

			targetCharacter = null;
		}
		ActionInterupted();
	}

	public override void ActionInterupted()
	{
		base.ActionInterupted();
		GameCharacter.AnimController.InCombat3Blend = false;
		move = false;

	}

	public override ActionBase CreateCopy()
	{
		FollowAttack3Blend copy = new FollowAttack3Blend();
		copy.attackData = attackData;
		return copy;
	}
}
