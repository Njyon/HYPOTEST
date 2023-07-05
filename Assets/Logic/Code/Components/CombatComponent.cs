using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum EAttackType
{
	Default,
	AttackHorizontal,
	AttckUp,
	AttackDown
}

public class CombatComponent
{
	public delegate void OnWeaponChanged(WeaponBase newWeapon, WeaponBase oldWeapon);
	public OnWeaponChanged onWeaponChanged;

	GameCharacter gameCharacter;
	ScriptableWeapon[] weapons = new ScriptableWeapon[4];
	WeaponBase currentWeapon;
	int currentWeaponIndex;
	int equipedWeaponAmount;
	Ultra.Timer attackTimer;
	GameCharacter hookedToCharacter;

	public Ultra.Timer AttackTimer { get { return attackTimer; } }
	public GameCharacter HookedToCharacter { get { return hookedToCharacter; } set { hookedToCharacter = value; } }

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
		attackTimer = new Ultra.Timer(0f, true);
	}

	public void UpdateComponent(float deltaTime) 
	{
		if (attackTimer != null) attackTimer.Update(deltaTime);
		if (CurrentWeapon != null) CurrentWeapon.UpdateWeapon(deltaTime);
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

	public void Attack(EAttackType attackType)
	{
		if (gameCharacter.MovementComponent.IsGrounded)
		{
			switch (attackType)
			{
				case EAttackType.Default: CurrentWeapon?.GroundAttack(); break;
				case EAttackType.AttackHorizontal: CurrentWeapon?.GroundDirectionAttack(); break;
				case EAttackType.AttckUp: CurrentWeapon?.GroundUpAttack(); break;
				case EAttackType.AttackDown: CurrentWeapon?.GroundDownAttack(); break;
				default: break;
			}
		}else
		{
			switch (attackType)
			{
				case EAttackType.Default: CurrentWeapon?.AirAttack(); break;
				case EAttackType.AttackHorizontal: CurrentWeapon?.AirDirectionAttack(); break;
				case EAttackType.AttckUp: CurrentWeapon?.AirUpAttack(); break;
				case EAttackType.AttackDown: CurrentWeapon?.AirDownAttack(); break;
				default: break;
			}
		}
	}
}
