using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TommyGun : WeaponBase
{
    public TommyGun() { }
	public TommyGun(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }


}