using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HoldAttackWithHookData : AttackData
{
	[SerializeField]
	public AnimationClip attackAnimation;
	public AnimationClip holdAttackAnimation;
}

public class HoldAttackWithHook : AttackBase
{
	public HoldAttackWithHookData attackData;
	bool inHoldAttack = false;

	public override void StartAction()
	{
		StartAttack(attackData.attackAnimation);
		GameCharacter?.CombatComponent?.AttackTimer.Stop();
		GameCharacter.MovementComponent.onCharacterGroundedChanged += OnCharacterGroundedChanged;
		inHoldAttack = false;	
	}

	public override void SwitchAnimationEvent()
	{
		GameCharacter.AnimController.SetHoldAttack(attackData.holdAttackAnimation);
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.AnimController.HoldAttack = true;
		inHoldAttack = true;
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		if (inHoldAttack)
		{
			if (!GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.HoldAttack))
			{
				ActionInterupted();
				return;
			}
			else
			{
				Vector3 vel = GameCharacter.MovementComponent.MovementVelocity;
				GameCharacter.MovementComponent.MovementVelocity = new Vector3(vel.x, -5, vel.z);
			}
		}
	}

	public override void ActionInterupted()
	{
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.AnimController.HoldAttack = false;
		inHoldAttack = false;
		GameCharacter.MovementComponent.onCharacterGroundedChanged += OnCharacterGroundedChanged; 
		GameCharacter.CombatComponent.CurrentWeapon.HitDetectionEnd();
		GameCharacter.CombatComponent.AllowEarlyLeaveAttackRecovery = true;
		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.AttackRecovery);
		Weapon.UnHookAllHookedCharacerts();
	}

	public override void OnHit(GameObject hitObj)
	{
		IDamage damageInterface = DoDamage(hitObj, attackData.Damage);

		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null) return;

		if (enemyCharacter.CombatComponent.CanGetHooked())
		{
			enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			enemyCharacter.AnimController.ResetAnimStatesHARD();
			Weapon.HookCharacterToCharacter(enemyCharacter);
			if (enemyCharacter.StateMachine != null) enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.HookedToCharacter);
		}
	}

	void OnCharacterGroundedChanged(bool newState)
	{
		if (newState)
		{
			ActionInterupted();
		}
	}

	public override ActionBase CreateCopy()
	{
		HoldAttackWithHook copy = new HoldAttackWithHook();
		copy.attackData = attackData;
		return copy;
	}
}
