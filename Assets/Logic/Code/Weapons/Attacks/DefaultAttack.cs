using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DefaultAttackData : AttackData
{
	[SerializeField]
	public AnimationClip attackAnimation;
}

[Serializable]
[CreateAssetMenu(fileName = "New DefaultAttack", menuName = "Assets/Weapons/Attacks/DefaultAttack")]
public class DefaultAttack : BaseAttack
{
	public DefaultAttackData attackData = new DefaultAttackData();

	public override void StartAttack()
	{
		if (attackData.attackAnimation == null) return;

		GameCharacter.AnimController.Attack(attackData.attackAnimation);
		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Attack);
		Weapon.AttackAnimType = EAttackAnimType.Default;
		GameCharacter.CombatComponent.AttackTimer.Start(attackData.attackAnimation.length);
	}

	public override void OnHit(GameObject hitObj)
	{
		IDamage damageInterface = Weapon.GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, Weapon.GetDamage());
	}

	public override Type GetAttackDataType()
	{
		return attackData.GetType();
	}

	public override void SetData(AttackData data)
	{
		this.attackData = (DefaultAttackData)data;
	}
}


