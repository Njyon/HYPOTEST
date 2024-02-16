using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyppoliteShotgun : WeaponBase
{
    public HyppoliteShotgun() { }
	public HyppoliteShotgun(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new HyppoliteShotgun(gameCharacter, weapon);
	}
}