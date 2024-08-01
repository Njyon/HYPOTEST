using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

[Serializable]
public class TeleportUltData : AttackData
{
	public int minTeleportAmount;
	public List<AnimationClip> teleportAnimations;
	public float timebetweenHits;
}

public class TeleportToEnemyUlt : AttackBase
{
	public TeleportUltData attackData;
	GameCharacter target;
	Vector3 teleportVector;
	bool shouldMove = false;
	int lastAttackIndex = 0;
	bool teleportRightSide = false;

	public override void StartAction()
	{
		int enemyAmount = GameCharacter.CharacterDetection.TargetGameCharacters.Count;
		if (enemyAmount == 0) return;

		int teleportAmount = Mathf.Max(attackData.minTeleportAmount, enemyAmount);

		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Attack);
		GameCharacter.CombatComponent.CurrentWeapon.AttackAnimType = EAttackAnimType.Default;
		lastAttackIndex = UnityEngine.Random.Range(0, attackData.teleportAnimations.Count);
		GameCharacter.AnimController.Attack(attackData.teleportAnimations[lastAttackIndex]);
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.IFrame);

		FreezAllEnemiesAndSubscribe();

		GameCharacter.MovementComponent.DeactiveStepup();
		TeleportAttacks(teleportAmount);
	}

	async void TeleportAttacks(int teleportAmount)
	{
		teleportRightSide = false;
		for (int i = 0; i < teleportAmount; i++)
		{
			if (GameCharacter.CharacterDetection.TargetGameCharacters.Count == 0) return; 

			int characterIndex = i % GameCharacter.CharacterDetection.TargetGameCharacters.Count;
			target = GameCharacter.CharacterDetection.TargetGameCharacters[characterIndex];
			Vector3 telportPos = target.MovementComponent.CharacterCenter + (teleportRightSide ? Vector3.right : Vector3.left) * (target.GameCharacterData.MinCharacterDistance + GameCharacter.GameCharacterData.MinCharacterDistance);
			Vector3 moveDir = telportPos - GameCharacter.MovementComponent.CharacterCenter;

			int attackIndex = UnityEngine.Random.Range(0, attackData.teleportAnimations.Count);
			if (attackIndex == lastAttackIndex)
			{
				attackIndex++;
				attackIndex = attackIndex % attackData.teleportAnimations.Count;
			}
			GameCharacter.AnimController.Attack(attackData.teleportAnimations[attackIndex]);
			lastAttackIndex = attackIndex;

			teleportVector = moveDir;
			shouldMove = true;

			GameCharacter.CombatComponent.CurrentWeapon.PlayDefaultAttackSound();
			DoDamage(target.gameObject, attackData.Damage);

			await new WaitForSeconds(attackData.timebetweenHits);

			teleportRightSide = !teleportRightSide;
		}

		ActionInterupted();
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		if (shouldMove)
		{
			float deltaTimeScale = 1f / Time.deltaTime;
			GameCharacter.MovementComponent.MovementVelocity = teleportVector * deltaTimeScale;
			shouldMove = false;
		}else
		{
			GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
		}

		//Vector3 rotationDir = target.MovementComponent.CharacterCenter - teleportVector;
		GameCharacter.RotateToDir(teleportRightSide ? Vector3.left : Vector3.right);
		GameCharacter.RotationTarget = GameCharacter.transform.rotation;
		//Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, rotationDir.normalized, rotationDir.magnitude, Color.red);
	}

	void FreezAllEnemiesAndSubscribe()
	{
		foreach (GameCharacter gc in GameCharacter.CharacterDetection.TargetGameCharacters)
		{
			gc.Animator.speed = 0;
			gc.onGameCharacterDied += GameCharacterDied;
		}
	}

	void UnfreezEnemiesAndUnsubscribe()
	{
		foreach (GameCharacter gc in GameCharacter.CharacterDetection.TargetGameCharacters)
		{
			EndEffectOnEnemy(gc);
		}
	}

	void GameCharacterDied(GameCharacter gc)
	{
		EndEffectOnEnemy(gc);
	}

	void EndEffectOnEnemy(GameCharacter gc)
	{
		gc.Animator.speed = 1;
		gc.onGameCharacterDied -= GameCharacterDied;
	}

	public override void ActionInterupted()
	{
		base.ActionInterupted();
		UnfreezEnemiesAndUnsubscribe();
		GameCharacter.MovementComponent.ActivateStepup();
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.CombatComponent.AllowEarlyLeaveAttackRecovery = true;
		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.AttackRecovery);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.IFrame);
	}

	public override ActionBase CreateCopy()
	{
		TeleportToEnemyUlt copy = new TeleportToEnemyUlt();
		copy.attackData = attackData;
		return copy;
	}
}
