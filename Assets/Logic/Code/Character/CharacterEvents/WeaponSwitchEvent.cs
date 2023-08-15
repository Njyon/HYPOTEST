using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeaponSwitchType
{
	Normal,
	NextWeapon,
	PreviousWeapon,
}

public class WeaponSwitchEvent : CharacterEvent
{
	int weaponIndex;
	EWeaponSwitchType weaponSwitchType;
	public WeaponSwitchEvent(GameCharacter gameCharacter, int weaponIndex, EWeaponSwitchType type = EWeaponSwitchType.Normal, float time = 0.1F) : base(gameCharacter, time)
	{
		this.weaponIndex = weaponIndex;
		weaponSwitchType = type;
	}

	public override bool CanBeExecuted()
	{
		return true;
	}

	public override EGameCharacterEvent GetGameCharacterEvenetType()
	{
		return EGameCharacterEvent.WeaponSwitchEvent;
	}

	public override void StartEvent()
	{
		switch (weaponSwitchType)
		{
			case EWeaponSwitchType.NextWeapon:
				gameCharacter?.CombatComponent?.EquipNextWeapon();
				break;
				case EWeaponSwitchType.PreviousWeapon:
				gameCharacter?.CombatComponent?.EquipPreviousWeapon();
				break;
			default:
				gameCharacter?.CombatComponent?.SwitchWeapon(weaponIndex);
				break;
		}
	}
}
