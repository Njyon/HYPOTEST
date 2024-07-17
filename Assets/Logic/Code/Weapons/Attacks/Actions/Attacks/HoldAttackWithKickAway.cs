using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HoldAttackWithKickAwayData : AttackData
{
	public AnimationClip attackAnimation;
	public AnimationClip holdAttackAnimation;
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
			// TODO Move to Enemy
		}else
		{
			ActionInterupted();
			return;
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
