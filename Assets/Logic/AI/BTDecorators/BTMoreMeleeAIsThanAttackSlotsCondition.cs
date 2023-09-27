using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Condition")]
public class BTMoreMeleeAIsThanAttackSlotsCondition : BTHyppoliteConditionDecoratorBase
{
	protected override bool OnCheckCondition(object options = null)
	{
		return AIManager.Instance.MeleeAIsCount > AIManager.Instance.MeleeCharacterAttackAmount;
	}
}
