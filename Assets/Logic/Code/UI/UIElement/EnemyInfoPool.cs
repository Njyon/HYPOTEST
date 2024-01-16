using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfoPool : MonoBehaviourPoolBase<EnemyInfo>
{
	public EnemyInfoPool(EnemyInfo instance, GameObject parent) : base(instance, parent)
	{ }

	public EnemyInfoPool(EnemyInfo instance, GameObject parent, int minSize) : base(instance, parent, minSize)
	{ }

	protected override void ActivateValue(EnemyInfo value)
	{
		value.gameObject.SetActive(true);
	}

	protected override void DeactivateValue(EnemyInfo value)
	{
		value.gameObject.SetActive(false);
	}

	protected override void DestroyElement(EnemyInfo element)
	{
		if (element.Equals(null)) return;
		GameObject.Destroy(element.gameObject);	
	}
}
