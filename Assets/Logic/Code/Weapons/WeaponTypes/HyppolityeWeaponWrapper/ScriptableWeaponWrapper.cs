using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScriptableWeaponWrapper", menuName = "Assets/Weapons/WeaponWrapper")]
public class ScriptableWeaponWrapper : ScriptableWeapon
{
	public List<ScriptableWeapon> weapons = new List<ScriptableWeapon>();

	public override void CreateWeapon(GameCharacter gameCharacter)
	{
		base.CreateWeapon(gameCharacter);
		for (int i = 0; i < weapons.Count; i++)
		{
			weapons[i].CreateWeapon(gameCharacter);
		}
	}

	public override ScriptableWeapon CreateCopy()
	{
		ScriptableWeaponWrapper instance = ScriptableObject.CreateInstance<ScriptableWeaponWrapper>();
		CopyData(ref instance);
		instance.weapons = weapons;
		return instance;
	}
}
