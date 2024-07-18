using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HoldAttackWithKickAwayData : AttackData
{
	public AnimationClip attackAnimation;
	public AnimationClip holdAttackAnimation;
	public float moveSpeed = 5f;
	public float stunTime = 0.2f;
	public float kickAwayStrenght = 20f;
}

public class HoldAttackWithKickAway : AttackBase
{
	public HoldAttackWithKickAwayData attackData;

	public override void StartAction()
	{
		StartAttack(attackData.attackAnimation);
	}

	public override void TriggerAnimationEvent()
	{
		GameCharacter.AnimController.SetHoldAttack(attackData.holdAttackAnimation);
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.AnimController.HoldAttack = true;
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		if (GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.HoldAttack))
		{
			//GameCharacter target = Ultra.HypoUttilies.FindCharactereNearestToDirection(GameCharacter.MovementComponent.CharacterCenter, 
			//	GameCharacter.MovementInput.magnitude > 0 ? GameCharacter.MovementInput : GameCharacter.transform.forward,
			//	ref GameCharacter.CharacterDetection.TargetGameCharacters);

			Vector3 moveDir = GameCharacter.MovementInput.normalized; //(target.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter).normalized;
			GameCharacter.MovementComponent.MovementVelocity = moveDir * attackData.moveSpeed;
		}else
		{
			ActionInterupted();
			return;
		}
	}

	public override void OnHit(GameObject hitObj)
	{
		if (hitObj == GameCharacter.gameObject) return;

		GameCharacter gc = hitObj.GetComponent<GameCharacter>();
		if (gc != null)
		{
			DoDamage(hitObj, attackData.Damage);
			// Kickaway needs to happen after damage
			GameCharacter.CombatComponent.KickAway(gc, attackData.stunTime, (gc.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter).normalized, attackData.kickAwayStrenght, true);
			Weapon.SpawnDamageHitEffect(gc);
		}
		else
		{
			DoDamage(hitObj, attackData.Damage);
		}
	}

	public override void ActionInterupted()
	{
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.AnimController.HoldAttack = false;
		GameCharacter.CombatComponent.CurrentWeapon.HitDetectionEnd();
		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.AttackRecovery);
	}

	public override ActionBase CreateCopy()
	{
		HoldAttackWithKickAway copy = new HoldAttackWithKickAway();
		copy.attackData = attackData;
		return copy;
	}
}
