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
	AttackUp,
	AttackDown
}

public class CombatComponent
{
	public delegate void OnWeaponChanged(WeaponBase newWeapon, WeaponBase oldWeapon);
	public OnWeaponChanged onWeaponChanged;

	GameCharacter gameCharacter;
	ScriptableWeapon[] weapons = new ScriptableWeapon[4];
	WeaponBase currentWeapon;
	WeaponBase nextWeapon;
	int currentWeaponIndex;
	int equipedWeaponAmount;
	Ultra.Timer attackTimer;
	Ultra.Timer defensiveTimer;
	GameCharacter hookedToCharacter;
	GameCharacter hookedCharacter;
	GameCharacter aimToCharacter;
	Vector3 moveToPosition;

	public Ultra.Timer AttackTimer { get { return attackTimer; } }
	public Ultra.Timer DefensiveTimer { get { return defensiveTimer; } }
	public GameCharacter HookedToCharacter { get { return hookedToCharacter; } set { hookedToCharacter = value; } }
	public GameCharacter HookedCharacter { get { return hookedCharacter; } set { hookedCharacter = value; } }
	public GameCharacter AimCharacter { get { return aimToCharacter; } set { aimToCharacter = value; } }
	public Vector3 MoveToPosition { get { return moveToPosition; } set { moveToPosition = value; } }

	public WeaponBase NextWeapon
	{
		get { return nextWeapon; }
		set
		{
			if (nextWeapon == value) return;

			nextWeapon = value;
		}
	}

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
		nextWeapon = null;
	}

	~CombatComponent()
	{
		if (gameCharacter != null)
		{
			gameCharacter.StateMachine.onStateChanged -= OnStateChanged;
		}
	}

	public void StartComponent()
	{
		InitWeapons();
		attackTimer = new Ultra.Timer(0f, true);
		defensiveTimer = new Ultra.Timer(0f, true);

		gameCharacter.StateMachine.onStateChanged += OnStateChanged;
	}

	public void UpdateComponent(float deltaTime) 
	{
		if (attackTimer != null) attackTimer.Update(deltaTime);
		if (defensiveTimer != null) defensiveTimer.Update(deltaTime);
		if (CurrentWeapon != null) CurrentWeapon.UpdateWeapon(deltaTime);

		if (NextWeapon != null)
		{
			CheckIfCanSwitchWeapon(gameCharacter.StateMachine.GetCurrentStateType());
		}
	}

	private void CheckIfCanSwitchWeapon(EGameCharacterState state)
	{
		switch (state)
		{
			case EGameCharacterState.Attack:
			case EGameCharacterState.DefensiveAction:
				break;
			default:
				UpdateWeapon();
				break;
		}
	}

	private void UpdateWeapon()
	{
		CurrentWeapon = NextWeapon;
		NextWeapon = null;
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
			for (int i = 0; i < weapons.Length; i++) 
			{
				weapons[i]?.Weapon?.InitWeapon();
			}

			currentWeaponIndex = 0;
			NextWeapon = weapons[currentWeaponIndex].Weapon;
		}
	}

	public void SwitchWeapon(int index)
	{
		if (weapons.Length >= index && weapons[index] != null && weapons[index].Weapon != null)
		{
			currentWeaponIndex = index;
			NextWeapon = weapons[currentWeaponIndex].Weapon;
		}
	}

	public void EquipNextWeapon()
	{
		int index = currentWeaponIndex + 1;
		index %= equipedWeaponAmount;
		SwitchWeapon(index);
	}

	public void EquipPreviousWeapon()
	{
		int index = currentWeaponIndex - 1;
		if (index < 0) index = equipedWeaponAmount - 1;
		SwitchWeapon(index);
	}

	public void Attack(EAttackType attackType)
	{
		if (NextWeapon != null)
			UpdateWeapon();

		if (gameCharacter.MovementComponent.IsGrounded)
		{
			switch (attackType)
			{
				case EAttackType.Default: CurrentWeapon?.GroundAttack(); break;
				case EAttackType.AttackHorizontal: CurrentWeapon?.GroundDirectionAttack(); break;
				case EAttackType.AttackUp: CurrentWeapon?.GroundUpAttack(); break;
				case EAttackType.AttackDown: CurrentWeapon?.GroundDownAttack(); break;
				default: break;
			}
		}else
		{
			switch (attackType)
			{
				case EAttackType.Default: CurrentWeapon?.AirAttack(); break;
				case EAttackType.AttackHorizontal: CurrentWeapon?.AirDirectionAttack(); break;
				case EAttackType.AttackUp: CurrentWeapon?.AirUpAttack(); break;
				case EAttackType.AttackDown: CurrentWeapon?.AirDownAttack(); break;
				default: break;
			}
		}
	}

	public void DefensiveAction()
	{
		if (NextWeapon != null)
			UpdateWeapon();

		CurrentWeapon?.DefensiveAction();
	}

	void OnStateChanged(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		if (NextWeapon != null) 
			CheckIfCanSwitchWeapon(newState.GetStateType());
	}
}
