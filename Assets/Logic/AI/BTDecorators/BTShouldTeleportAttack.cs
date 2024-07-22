using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Megumin.Binding;

public class BTShouldTeleportAttack : BTHeraklesDecoratorBase
{
	protected override bool OnCheckCondition(object options = null)
	{
		return TeleportAttackForce;
	}
}
