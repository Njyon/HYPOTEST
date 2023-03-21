using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitchEvent : CharacterEvent
{
	int weaponIndex;
	public WeaponSwitchEvent(GameCharacter gameCharacter, int weaponIndex, float time = 0.2F) : base(gameCharacter, time)
	{
		this.weaponIndex = weaponIndex;
	}

	public override bool CanBeExecuted()
	{
		return true;
	}

	public override void StartEvent()
	{

	}
}
