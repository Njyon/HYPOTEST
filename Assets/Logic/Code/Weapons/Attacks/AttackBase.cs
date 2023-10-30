using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : ActionBase
{
	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, action);

	}

	public void StartAttack(AnimationClip clip)
	{
		if (clip == null) return;

		GameCharacter?.AnimController?.Attack(clip);
		GameCharacter?.StateMachine?.RequestStateChange(EGameCharacterState.Attack);
		Weapon.AttackAnimType = EAttackAnimType.Default;
		GameCharacter?.CombatComponent?.AttackTimer.Start(clip.length);
	}

	public IDamage DoDamage(GameObject hitObject, float damage)
	{
		IDamage damageInterface = Weapon.GetDamageInterface(hitObject);
		if (damageInterface == null) return damageInterface;
		damageInterface.DoDamage(GameCharacter, Weapon.GetDamage(damage));
		return damageInterface;
	}
}
