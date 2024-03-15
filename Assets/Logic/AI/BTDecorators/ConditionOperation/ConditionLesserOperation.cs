using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionLesserOperation : ConditionOperation
{
	[Header("TargetDistanceCondition")]
	public bool greaterEqualOperation = false;
	public float checkDistance = 5f;
	public bool checkWithHight = false;
	public bool inverseCheck = false;

	public override bool DoOperation()
	{
		float distance;
		if (checkWithHight)
			distance = Vector3.Distance(GameCharacter.transform.position, Target.transform.position);
		else
			distance = Vector3.Distance(GameCharacter.transform.position.IgnoreAxis(EAxis.YZ), Target.transform.position.IgnoreAxis(EAxis.YZ));

		if (inverseCheck)
			return !(greaterEqualOperation ? distance <= checkDistance : distance < checkDistance);
		else
			return (greaterEqualOperation ? distance <= checkDistance : distance < checkDistance);
	}
}
