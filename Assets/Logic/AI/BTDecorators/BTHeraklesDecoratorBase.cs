using Megumin.Binding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTHeraklesDecoratorBase : BTHyppoliteConditionDecoratorBase
{
	const string teleportAttackForceString = "TeleportAttackForce";

	public bool TeleportAttackForce
	{
		get
		{
			if (Tree.RootTree.Variable.TryGetParam<bool>(teleportAttackForceString, out var v))
			{
				return v.Value;
			}
			else
			{
				RefVar<bool> teleportAttackForce = new RefVar<bool>();
				teleportAttackForce.RefName = teleportAttackForceString;
				teleportAttackForce.Value = false;

				Tree.RootTree.InitAddVariable(teleportAttackForce);
			}
			return false;
		}
		set
		{
			if (Tree.RootTree.Variable.Contains(teleportAttackForceString))
			{
				Tree.RootTree.Variable.TrySetValue<bool>(teleportAttackForceString, value);
			}
		}
	}
}
