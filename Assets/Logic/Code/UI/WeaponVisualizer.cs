using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponVisualizer : MonoBehaviour
{
	[SerializeField] Image backGround;
	[SerializeField] Image weaponImage;
	WeaponBase weapon;

	int weaponIndex;
	public int WeaponIndex { get { return weaponIndex; } }	

	public void InitWeaponVisualizer(WeaponBase weaponBase, int index)
	{
		weapon = weaponBase;
		weaponIndex = index;

		if (weapon != null && weapon.WeaponData.WeaponImage != null)
			weaponImage.sprite = weapon.WeaponData.WeaponImage;
	}

	public void Deactivate()
	{
		backGround.color = Color.clear;
		weaponImage.color = Color.clear;
	}
}
