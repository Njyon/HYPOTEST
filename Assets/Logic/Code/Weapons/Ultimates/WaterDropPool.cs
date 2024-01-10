using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropPool : MonoBehaviourPoolBase<WaterDrop>
{
	public WaterDropPool(WaterDrop instance, GameObject parent) : base(instance, parent)
	{ }

	public WaterDropPool(WaterDrop instance, GameObject parent, int minSize) : base(instance, parent, minSize)
	{ }

	protected override void ActivateValue(WaterDrop value)
	{
		value.gameObject.SetActive(true);
	}

	protected override void DeactivateValue(WaterDrop value)
	{
		value.TurnOff();
		value.gameObject.SetActive(false);
	}

	protected override void DestroyElement(WaterDrop element)
	{
		if (element.Equals(null)) return;
		GameObject.Destroy(element.gameObject);
	}
}
