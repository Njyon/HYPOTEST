using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviourPoolBase<WeaponProjectile>
{

	public ProjectilePool(WeaponProjectile instance, GameObject parent) : base(instance, parent)
	{ }

	public ProjectilePool(WeaponProjectile instance, GameObject parent, int minSize) : base(instance, parent, minSize)
	{ }

	protected override void ActivateValue(WeaponProjectile value)
	{
		value?.gameObject.SetActive(true);
	}

	protected override void DeactivateValue(WeaponProjectile value)
	{
		value?.TurnOff();
		value?.gameObject.SetActive(false);
	}
}
