using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyppoliteShotgun : WeaponBase
{
    public HyppoliteShotgun() { }
	public HyppoliteShotgun(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

	public override void EquipWeapon()
	{
		base.EquipWeapon();

		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.AimIKCorrection);
	}

	public override void UnEquipWeapon()
	{
		base.UnEquipWeapon();

		GameCharacter.BuffComponent.RemoveBuff(EBuff.ForceAim);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.AimIKCorrection);
	}

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new HyppoliteShotgun(gameCharacter, weapon);
	}
}