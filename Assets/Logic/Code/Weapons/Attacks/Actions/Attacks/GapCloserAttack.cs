using Megumin.GameFramework.AI.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GapCloserAttackData : AttackData
{
	public AnimationClip gapCloserAttack;
	public AnimationClip gapCloserAttackHold;
	public AnimationClip gapCloserAttackTrigger;
	public float speed = 50;
	public float time = 1;
}

public class GapCloserAttack : AttackBase
{
	public GapCloserAttackData attackData;

	bool gapCloserAttackhit = false;
	bool gapCloserAttackMove = false;
	Ultra.Timer gapCloserTimer = new Ultra.Timer();

	public override void StartAction()
	{
		StartAttack(attackData.gapCloserAttack);

		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += OnAttackTimerFinished;
		gapCloserAttackhit = false;
		gapCloserAttackMove = false;
	}

	public override void OnHit(GameObject hitObj)
	{
		DoDamage(hitObj, attackData.Damage);
		if (!gapCloserAttackhit)
			GapCLoserAttackEnd();
	}

	void OnAttackTimerFinished()
	{
		if (Weapon.CurrentAttackType == EExplicitAttackType.GroundedDownAttack)
		{
			GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= OnAttackTimerFinished;

			Weapon.SetHoldAttack(attackData.gapCloserAttackHold);
			GameCharacter.HitDetectionEventStart(new AnimationEvent());
			gapCloserAttackMove = true;
			gapCloserTimer.Start(attackData.time);
			gapCloserTimer.onTimerFinished += OnGapCloserTimerFinished;
		}
	}
	void OnGapCloserTimerFinished()
	{
		if (!gapCloserAttackhit)
		{
			GapCLoserAttackEnd();
		}
	}
	void GapCLoserAttackEnd()
	{
		if (gapCloserTimer.IsRunning) gapCloserTimer.Stop();
		gapCloserTimer.onTimerFinished -= OnGapCloserTimerFinished;

		gapCloserAttackhit = true;
		Weapon.SetTriggerAttack(attackData.gapCloserAttackTrigger);
		gapCloserAttackMove = false;
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		if (gapCloserAttackMove)
		{
			gapCloserTimer.Update(deltaTime);
			Vector3 velocity = GameCharacter.transform.forward.normalized * attackData.speed;
			GameCharacter.MovementComponent.MovementVelocity = velocity;
		}
	}

	public override ActionBase CreateCopy()
	{
		GapCloserAttack copy = new GapCloserAttack();
		copy.attackData = attackData;
		return copy;
	}
}
