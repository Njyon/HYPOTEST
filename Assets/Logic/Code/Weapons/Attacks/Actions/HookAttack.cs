using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HookAttackData : DefaultAttackData
{
	public float maxVerticalMovement = 5f;
}

public class HookAttack : AttackBase
{
	public HookAttackData attackData;

	public override void StartAction()
	{
		StartAttack(attackData.attackAnimation);
	}

	public override void OnHit(GameObject hitObj)
	{
		base.OnHit(hitObj);
		IDamage damageInterface = DoDamage(hitObj, attackData.Damage);

		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null) return;

		if (enemyCharacter.CombatComponent.CanGetHooked())
		{
			if (enemyCharacter.CombatComponent != null) enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			Weapon.HookCharacterToCharacter(enemyCharacter);
			if (enemyCharacter.StateMachine != null) enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.HookedToCharacter);
		}
	}

	public override float MaxVerticalMovement()
	{
		return attackData.maxVerticalMovement;
	}

	public override float GetActionRanting()
	{
		return attackData.Rating;
	}

	public override float GetActionDischarge()
	{
		return attackData.Discharge;
	}
}
