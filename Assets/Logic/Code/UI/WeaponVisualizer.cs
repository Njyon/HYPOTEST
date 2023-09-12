using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponVisualizer : MonoBehaviour
{
	[SerializeField] Image backGround;
	[SerializeField] Image weaponImage;
	[SerializeField] Image weaponSelectorHighLight;
	[SerializeField] Color hightLightColor = Color.yellow;
	WeaponBase weapon;
	PlayerGameCharacter gameCharacter;


	public void InitWeaponVisualizer(WeaponBase weaponBase, PlayerGameCharacter player)
	{
		weapon = weaponBase;
		gameCharacter = player;

		gameCharacter.CombatComponent.onNextWeapon += OnNextWeapon;
		weapon.onChargeValueChanged += OnChargeValueChanged;

		if (weapon != null && weapon.WeaponData.WeaponImage != null)
			weaponImage.sprite = weapon.WeaponData.WeaponImage;

		OnNextWeapon(gameCharacter.CombatComponent.NextWeapon != null ? gameCharacter.CombatComponent.NextWeapon : gameCharacter.CombatComponent.CurrentWeapon, null);
		SetWeaponChargeFill();
	}

	public void Deactivate()
	{
		backGround.color = Color.clear;
		weaponImage.color = Color.clear;
	}

	void HightLightUI()
	{
		weaponSelectorHighLight.color = hightLightColor;
	}

	void RemoveHighlight()
	{
		weaponSelectorHighLight.color = Color.clear;

	}

	void OnNextWeapon(WeaponBase newWeapon, WeaponBase oldWeapon)
	{
		if (newWeapon == null) return;
		if (newWeapon == weapon) HightLightUI();
		else RemoveHighlight();
	}

	void SetWeaponChargeFill()
	{
		backGround.fillAmount = Ultra.Utilities.Remap(weapon.Charge, 0, weapon.WeaponData.MaxChargeAmount, 0f, 1f);
	}

	void OnChargeValueChanged(float newCharge, float oldCharge)
	{
		SetWeaponChargeFill();
	}
}
