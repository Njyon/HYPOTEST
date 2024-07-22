using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Condition")]
public class BTCanBulletRainDebuffEnemy : BTHyppoliteConditionDecoratorBase
{
	protected override bool OnCheckCondition(object options = null)
	{
		return !TargetGameCharacter.BuffComponent.IsBuffActive(EBuff.BulletRain);
	}
}
