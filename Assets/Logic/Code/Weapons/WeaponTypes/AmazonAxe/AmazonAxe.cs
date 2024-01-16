using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmazonAxe : WeaponBase
{
    public AmazonAxe() { }
	public AmazonAxe(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new AmazonAxe(gameCharacter, weapon);
	}
}