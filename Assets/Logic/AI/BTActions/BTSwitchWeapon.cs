using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Action")]
public class BTSwitchWeapon : BTHyppoliteActionNodeBase
{
	protected override void OnEnter(object options = null)
	{
		base.OnEnter(options);

		GameCharacter.CombatComponent.EquipNextWeapon();
	}
}
