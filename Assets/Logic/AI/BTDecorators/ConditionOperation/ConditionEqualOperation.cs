using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionEqualOperation : ConditionOperation
{
	[Header("TargetDistanceCondition")]
	public float minDistance = 1f;
	public float treshhold = 1f;
	public bool checkWithHight = false;
	public bool inverseCheck = false;

	public override bool DoOperation()
	{
		float distance;
		if (checkWithHight)
			distance = Vector3.Distance(GameCharacter.transform.position, Target.transform.position);
		else
			distance = Vector3.Distance(Ultra.Utilities.IgnoreAxis(GameCharacter.transform.position, EAxis.YZ), Ultra.Utilities.IgnoreAxis(Target.transform.position, EAxis.YZ));

		if (inverseCheck)
			return !Ultra.Utilities.IsNearlyEqual(distance, minDistance, treshhold);
		else
			return Ultra.Utilities.IsNearlyEqual(distance, minDistance, treshhold);
	}
}
