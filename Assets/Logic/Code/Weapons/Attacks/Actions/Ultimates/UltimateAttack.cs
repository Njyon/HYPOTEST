using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UlitmateAttackData : AttackData
{
	[SerializeField]
	public AnimationClip attackAnimation;
}

public class UltimateAttack : AttackBase
{
	public UlitmateAttackData attackData;

	public override void StartAction()
	{
		// Cant be a foreach because Detection count gets changed becasue of dead
		for (int i = 0; i < GameCharacter.CharacterDetection.OverlappingGameCharacter.Count; i++)
		{
			GameCharacter gc = GameCharacter.CharacterDetection.OverlappingGameCharacter[i];
			float health = gc.Health.CurrentValue;
			gc.DoDamage(GameCharacter, health);
		}
		GameCharacter.CombatComponent.CurrentWeapon.UltCharge = 0;
	}

	public override ActionBase CreateCopy()
	{
		UltimateAttack copy = new UltimateAttack();
		copy.attackData = attackData;
		return copy;
	}
}
