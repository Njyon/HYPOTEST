using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTHasWeaponTypeEquiped : BTHyppoliteConditionDecoratorBase
{
    [Header("HasWeaponTypeEquiped")]
    public EWeaponType weaponType;

	protected override bool OnCheckCondition(object options = null)
	{
		bool check = GameCharacter.CombatComponent.CurrentWeapon.WeaponData.WeaponType == weaponType; 
		return Invert ? !check : check;
	}
}
