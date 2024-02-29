using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : WeaponBase
{
    public Bazooka() { }
	public Bazooka(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

	public override void EquipWeapon()
    {
        base.EquipWeapon();
    }

    public override void UnEquipWeapon()
    {
        base.UnEquipWeapon();
	}

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new Bazooka(gameCharacter, weapon);
	}
}