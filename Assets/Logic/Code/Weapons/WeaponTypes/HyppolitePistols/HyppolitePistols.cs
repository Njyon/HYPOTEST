using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyppolitePistols : WeaponBase
{
    public HyppolitePistols() { }
	public HyppolitePistols(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

	public override void UnEquipWeapon()
	{
		base.UnEquipWeapon();

		GameCharacter.BuffComponent.RemoveBuff(EBuff.ForceAim);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
	}

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new HyppolitePistols(gameCharacter, weapon);
	}
}