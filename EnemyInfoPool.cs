using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfoPool : MonoBehaviourPoolBase<EnemyInfoPool>
{
	public EnemyInfoPool(EnemyInfoPool instance, GameObject parent) : base(instance, parent)
	{
	}

	protected override void DestroyElement(EnemyInfoPool element)
	{
		throw new System.NotImplementedException();
	}
}
