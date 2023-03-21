using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CombatComponent
{
	public delegate void OnWeaponChanged(WeaponBase newWeapon, WeaponBase oldWeapon);
	public OnWeaponChanged onWeaponChanged;

	GameCharacter gameCharacter;
	ScriptableWeapon[] weapons = new ScriptableWeapon[4];
	WeaponBase currentWeapon;
	int currentWeaponIndex;
	int equipedWeaponAmount;

	public WeaponBase CurrentWeapon { 
		get { return currentWeapon; } 
		private set
		{
			if (currentWeapon == value) return;

			WeaponBase oldWeapon = currentWeapon;
			if (oldWeapon != null) oldWeapon.UnEquipWeapon();
			currentWeapon = value;
			if (currentWeapon != null)
			{
				currentWeapon.EquipWeapon();
				if (onWeaponChanged != null) onWeaponChanged(currentWeapon, oldWeapon);
			}
		}
	}

	public CombatComponent(GameCharacter gameCharacter)
	{
		this.gameCharacter = gameCharacter;
	}

	public void StartComponent()
	{
		InitWeapons();
	}

	public void UpdateComponent() 
	{ 
	
	}

	void InitWeapons()
	{
		equipedWeaponAmount = gameCharacter.GameCharacterData.Weapons.Count;
		for (int i = 0; i < gameCharacter.GameCharacterData.Weapons.Count; i++)
		{
			if (i >= 4) break;
			weapons[i] = gameCharacter.GameCharacterData.Weapons[i];
			WeaponBase[] weaponArray = Ultra.Utilities.GetAll<WeaponBase>().ToArray();
			for (int j = 0; j < weaponArray.Length; j++)
			{
				if (weaponArray[j].GetType().Name == weapons[i].WeaponClassName)
				{
					weapons[i].Weapon = Activator.CreateInstance(weaponArray[j].GetType(), gameCharacter, weapons[i]) as WeaponBase;
					break;
				}
			}
		}
		if (weapons.Length >= 1 && weapons[0] != null && weapons[0].Weapon != null)
		{
			currentWeaponIndex = 0;
			CurrentWeapon = weapons[currentWeaponIndex].Weapon;
		}
	}

	public void SwitchWeapon(int index)
	{
		if (weapons.Length >= index && weapons[index] != null && weapons[index].Weapon != null)
		{
			currentWeaponIndex = index;
			CurrentWeapon = weapons[currentWeaponIndex].Weapon;
		}
	}

	public void NextWeapon()
	{
		int index = currentWeaponIndex + 1;
		index %= equipedWeaponAmount;
		SwitchWeapon(index);
	}

	public void PreviousWeapon()
	{
		int index = currentWeaponIndex - 1;
		if (index < 0) index = equipedWeaponAmount - 1;
		SwitchWeapon(index);
	}
}
