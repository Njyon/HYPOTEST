using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Condition")]
public class BTTargetIsSetCondition : BTHyppoliteConditionDecoratorBase
{
	protected override bool OnCheckCondition(object options = null)
	{
		return TargetGameCharacter != null;
	}
}
