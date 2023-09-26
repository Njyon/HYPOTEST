using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Condition")]
public class BTCanMeleeAttackCondition : BTHyppoliteConditionDecoratorBase
{

	protected override bool OnCheckCondition(object options = null)
	{
		if (Tree.Variable.TryGetParam<bool>("CanMeleeAttack", out var canMeleeAttack))
		{
			return canMeleeAttack.Value;
		}
		return false;
	}
}
