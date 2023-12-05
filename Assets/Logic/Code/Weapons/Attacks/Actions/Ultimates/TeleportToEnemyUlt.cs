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
	Vector3 teleportVector;
	bool shouldMove = false;

	public override void StartAction()
	{
		int enemyAmount = GameCharacter.CharacterDetection.OverlappingGameCharacter.Count;
		int teleportAmount = Mathf.Max(attackData.minTeleportAmount, enemyAmount);

		FreezAllEnemiesAndSubscribe();

		GameCharacter.MovementComponent.DeactiveStepup();
		TeleportAttacks(teleportAmount);
	}

	async void TeleportAttacks(int teleportAmount)
	{
		bool teleportRightSide = false;
		for (int i = 0; i < teleportAmount; i++)
		{
			int characterIndex = i % GameCharacter.CharacterDetection.OverlappingGameCharacter.Count;
			GameCharacter gc = GameCharacter.CharacterDetection.OverlappingGameCharacter[characterIndex];
			Vector3 telportPos = gc.MovementComponent.CharacterCenter + (teleportRightSide ? Vector3.right : Vector3.left) * gc.GameCharacterData.MinCharacterDistance;
			Vector3 moveDir = telportPos - GameCharacter.MovementComponent.CharacterCenter;

			teleportVector = moveDir;
			shouldMove = true;

			await new WaitForSecondsRealtime(attackData.timebetweenHits);

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
	}

	void FreezAllEnemiesAndSubscribe()
	{
		foreach (GameCharacter gc in GameCharacter.CharacterDetection.OverlappingGameCharacter)
		{
			gc.Animator.speed = 0;
			gc.onGameCharacterDied += GameCharacterDied;
		}
	}

	void UnfreezEnemiesAndUnsubscribe()
	{
		foreach (GameCharacter gc in GameCharacter.CharacterDetection.OverlappingGameCharacter)
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
	}

	public override ActionBase CreateCopy()
	{
		TeleportToEnemyUlt copy = new TeleportToEnemyUlt();
		copy.attackData = attackData;
		return copy;
	}
}
