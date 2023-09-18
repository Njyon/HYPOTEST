using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Condition")]
public class BTFollowPlayerCondition : BTHyppoliteConditionDecoratorBase
{
	[Header("FollowPlayerCondition")]
	public float minDistance = 1f;
	public float treshhold = 1f;
	public bool checkWithHight = false;

	protected override bool OnCheckCondition(object options = null)
	{
		float distance;
		if (checkWithHight)
			distance = Vector3.Distance(GameCharacter.transform.position, TargetGameCharacter.transform.position);
		else
			distance = Vector3.Distance(Ultra.Utilities.IgnoreAxis(GameCharacter.transform.position, EAxis.YZ), Ultra.Utilities.IgnoreAxis(TargetGameCharacter.transform.position, EAxis.YZ));

		return !Ultra.Utilities.IsNearlyEqual(distance, minDistance, treshhold);
	}
}
