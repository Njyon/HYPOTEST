using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubWeapon : WeaponBase
{
    public ClubWeapon() { }
	public ClubWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new ClubWeapon(gameCharacter, weapon);
	}
}