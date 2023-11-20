using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[Serializable]
public class SmashDownAttackData : AttackData
{
	public AnimationClip downAttack;
	public AnimationClip downAttackHold;
	public AnimationClip downAttackTrigger;
	public float speed = 50;
}

public class SmashDownAttack : AttackBase
{
	public SmashDownAttackData attackData;

	bool landed = false;
	bool startFalling = false;

	public override void StartAction()
	{
		StartAttack(attackData.downAttack);

		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += AttackTimerFinished;
		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;
		landed = false;
		startFalling = false;
	}
	void AttackTimerFinished()
	{
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
		Weapon.SetHoldAttack(attackData.downAttackHold);
		startFalling = true;

	}

	public override void OnHit(GameObject hitObj)
	{
		Profiler.BeginSample("SmashOnHit");
		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null || enemyCharacter.StateMachine == null || enemyCharacter.CombatComponent == null) return;
		if (enemyCharacter.StateMachine.CanSwitchToStateOrIsState(EGameCharacterState.HookedToCharacter))
		{
			enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			Weapon.HookCharacterToCharacter(enemyCharacter);
			enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.HookedToCharacter);
		}
		Profiler.EndSample();
	}

	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{

		if ((collisionFlag & CollisionFlags.Below) != 0)
		{
			if (Weapon.CurrentAttackType == EExplicitAttackType.AirDownAttack)
			{
				if (!landed) OnAirDownHitLanding();
			}
		}
	}

	void OnAirDownHitLanding()
	{
		Profiler.BeginSample("SmashDOwnOnAirDownHitLanding");
		landed = true;
		GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;

		Weapon.SetTriggerAttack(attackData.downAttackTrigger);

		foreach (GameObject obj in Weapon.HitObjects)
		{
			OnGroundAttackHit(obj);
		}
		Profiler.EndSample();
	}

	void OnGroundAttackHit(GameObject hitObject)
	{
		Profiler.BeginSample("SmashDownDoDamage");
		DoDamage(hitObject, attackData.Damage);
		Profiler.EndSample();
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		Profiler.BeginSample("SmashDownPostAttackLogic");
		base.PostAttackStateLogic(deltaTime);
		UpdateAirDownAttack(deltaTime);
		Profiler.EndSample();
	}

	void UpdateAirDownAttack(float deltaTime)
	{
		Profiler.BeginSample("SmashDownUpdateAirDownAttack");
		if (!landed && startFalling)
		{
			Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
			velocity = new Vector3(velocity.x, velocity.y - attackData.speed, velocity.z);
			GameCharacter.MovementComponent.MovementVelocity = velocity;
		}
		Profiler.EndSample();
	}

	public override void AttackPhaseStart()
	{
		base.AttackPhaseStart();
		startFalling = true;
	}

	public override void EndAttackStateLogic()
	{
		ActionInterupted();
	}

	public override void ActionInterupted()
	{
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
		GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;
		Weapon.UnHookAllHookedCharacerts();
	}

	public override float GetActionRanting()
	{
		return attackData.Rating;
	}

	public override float GetActionDischarge()
	{
		return attackData.Discharge;
	}

	public override ActionBase CreateCopy()
	{
		SmashDownAttack copy = new SmashDownAttack();
		copy.attackData = attackData;
		return copy;
	}
}
