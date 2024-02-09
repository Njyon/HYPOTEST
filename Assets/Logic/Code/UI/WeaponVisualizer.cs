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
	[SerializeField] Color hightLightUltColor = Color.red;
	Color defaultColor;
	WeaponBase weapon;
	PlayerGameCharacter gameCharacter;


	public void InitWeaponVisualizer(WeaponBase weaponBase, PlayerGameCharacter player)
	{
		weapon = weaponBase?.This;
		gameCharacter = player;

		gameCharacter.CombatComponent.onNextWeapon += OnNextWeapon;
		//weapon.onChargeValueChanged += OnChargeValueChanged;
		weapon.onUltChargeValueChanged += OnChargeValueChanged;

		if (weapon != null && weapon.WeaponData.WeaponImage != null)
			weaponImage.sprite = weapon.WeaponData.WeaponImage;

		defaultColor = backGround.color;

		OnNextWeapon(gameCharacter.CombatComponent.NextWeapon != null ? gameCharacter.CombatComponent.NextWeapon : gameCharacter.CombatComponent.CurrentWeapon, null, gameCharacter);
		SetWeaponChargeFill();
	}

	void OnDestroy()
	{
		if (gameCharacter != null) gameCharacter.CombatComponent.onNextWeapon -= OnNextWeapon;
		//if (weapon != null) weapon.onChargeValueChanged -= OnChargeValueChanged;
		if (weapon != null) weapon.onUltChargeValueChanged -= OnChargeValueChanged;
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

	void OnNextWeapon(WeaponBase newWeapon, WeaponBase oldWeapon, GameCharacter gameCharacter)
	{
		if (newWeapon == null) return;
		if (newWeapon == weapon) HightLightUI();
		else RemoveHighlight();
	}

	void SetWeaponChargeFill()
	{
		//backGround.fillAmount = Ultra.Utilities.Remap(weapon.Charge, 0, weapon.WeaponData.MaxChargeAmount, 0f, 1f);
		backGround.fillAmount = Ultra.Utilities.Remap(weapon.UltCharge, 0, weapon.WeaponData.MaxUltChargeAmount, 0f, 1f);
		if (backGround.fillAmount == 1f)
		{
			backGround.color = hightLightUltColor;
		}
		else
		{
			backGround.color = defaultColor;
		}
	}

	void OnChargeValueChanged(float newCharge, float oldCharge)
	{
		SetWeaponChargeFill();
	}
}
