using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EChargeAttackType
{
	unknown,
	chargeStart,
	chargeHold,
	chargeHitTarget,
	chargeHitWall,
}

[Serializable]
public class ChargeAttackData : AttackData
{
	public AnimationClip startChargeAnim;
	public AnimationClip holdChargeAnim;
	public AnimationClip hitTargetAnim;
	public AnimationClip hitWallAnim;
	public float chargeSpeed = 5f;
	public float kickAwayStrenght = 20f;
	public float kickAwayStunTime = 1f;
	public float maxChargeTime = 1f;
}

public class ChargeAttack : AttackBase
{
	public ChargeAttackData attackData;
	EChargeAttackType chargeAttackType = EChargeAttackType.unknown;
	GameCharacter target;
	Vector3 chargeDir;
	Ultra.Timer chargeTimer;

	public override void StartAction()
	{
		StartAttack(attackData.startChargeAnim);
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += OnStartChargeTimerFinished;
		chargeAttackType = EChargeAttackType.chargeStart;
		target = Ultra.HypoUttilies.FindCharactereNearestToDirection(GameCharacter.MovementComponent.CharacterCenter, GameCharacter.MovementInput.magnitude > 0 ? GameCharacter.MovementInput : GameCharacter.transform.forward, GameCharacter.Team, ref GameCharacter.CharacterDetection.DetectedGameCharacters);

		chargeTimer = new Ultra.Timer(attackData.maxChargeTime);

		if (target != null)
		{
			GameCharacter.RotateToDir((target.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter).IgnoreAxis(EAxis.YZ).normalized);
		}
		chargeDir = target != null ? (target.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter).IgnoreAxis(EAxis.YZ).normalized : GameCharacter.transform.forward;
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		switch (chargeAttackType)
		{
			case EChargeAttackType.chargeHold:
				chargeTimer.Update(Time.deltaTime);

				
				GameCharacter.MovementComponent.MovementVelocity = chargeDir * attackData.chargeSpeed;

				break;
			default:break;
		}
	}

	public override void OnHit(GameObject hitObj)
	{
		GameCharacter gc = hitObj.GetComponent<GameCharacter>();
		if (gc != null)
		{
			if (gc.PluginStateMachine.IsPluginStatePlugedIn(EPluginCharacterState.Parry))
			{
				GameCharacter.DoDamage(gc, attackData.Damage, false, false, true);
				ChargeAtWall();
				return;
			}
			else if (gc.PluginStateMachine.IsPluginStatePlugedIn(EPluginCharacterState.Block))
			{
				ChargeAtWall();
				return;
			}
		}

		GameCharacter.AnimController.Attack(attackData.hitTargetAnim);
		GameCharacter.AnimController.InAttack = true;
		GameCharacter.AnimController.HoldAttack = false;
		chargeAttackType = EChargeAttackType.chargeHitTarget;

		DoDamage(hitObj, attackData.Damage);

		if (gc == null) return;

		GameCharacter.CombatComponent.KickAway(gc, attackData.kickAwayStunTime, Vector3.up, attackData.kickAwayStrenght, true);
	}

	void OnStartChargeTimerFinished()
	{
		GameCharacter.AnimController.SetHoldAttack(attackData.holdChargeAnim);
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= OnStartChargeTimerFinished;
		chargeTimer.Start();
		chargeTimer.onTimerFinished += OnChargeTimerFinished;
		GameCharacter.AnimController.HoldAttack = true;
		GameCharacter.AnimController.InAttack = false;
		chargeAttackType = EChargeAttackType.chargeHold;
		GameCharacter.CombatComponent.CurrentWeapon.HitDetectionStart();

		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;

	}

	void OnChargeTimerFinished()
	{
		chargeTimer.onTimerFinished -= OnChargeTimerFinished;

		GameCharacter.RequestBestCharacterState();
	}

	public override void ActionInterupted()
	{
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.AnimController.HoldAttack = false;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= OnStartChargeTimerFinished;
		GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;
		chargeTimer.onTimerFinished -= OnChargeTimerFinished;
		GameCharacter.CombatComponent.CurrentWeapon.HitDetectionEnd();

	}

	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		if ((collisionFlag & CollisionFlags.CollidedSides) != 0)
		{
			ChargeAtWall();
		}
	}

	void ChargeAtWall()
	{
		GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;
		chargeTimer.onTimerFinished -= OnChargeTimerFinished;
		GameCharacter.CombatComponent.CurrentWeapon.HitDetectionEnd();
		GameCharacter.AnimController.Attack(attackData.hitWallAnim);
		GameCharacter.AnimController.InAttack = true;
		GameCharacter.AnimController.HoldAttack = false;
		GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
		chargeAttackType = EChargeAttackType.chargeHitWall;
	}

	public override ActionBase CreateCopy()
	{
		ChargeAttack copy = new ChargeAttack();
		copy.attackData = attackData;
		return copy;
	}
}
