using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShootAttackData : AttackData
{
	[SerializeField]
	public AnimationClip shootAddativeAnimation;
}

public class ShootAttack : AttackBase
{
	public ShootAttackData attackData;

	public override void StartAction()
	{
		GameCharacter.AnimController.ApplyAddativeAnimationToAddativeState(attackData.shootAddativeAnimation);
		GameCharacter.AnimController.InAddativeState = true;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += OnTimerFinished;
		GameCharacter.CombatComponent.AttackTimer.Start(attackData.shootAddativeAnimation.length);
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Shoot);
	}

	public override void ActionInterupted()
	{
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= OnTimerFinished;
		GameCharacter.CombatComponent.AttackTimer.Stop();
		GameCharacter.AnimController.InAddativeState = false;
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
	}

	void OnTimerFinished()
	{
		GameCharacter.AnimController.InAddativeState = false;
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
	}

	public override ActionBase CreateCopy()
	{
		ShootAttack copy = new ShootAttack();
		copy.attackData = attackData;
		return copy;
	}
}
