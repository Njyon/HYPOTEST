using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Vector3Extension 
{
	public static Vector3 IgnoreAxis(this Vector3 vector, EAxis ignoreAxis)
	{
		return Ultra.Utilities.IgnoreAxis(vector, ignoreAxis);
	}

	public static bool IsNearlyEqual(this Vector3 vector, Vector3 otherVector, float epsilon)
	{
		return Ultra.Utilities.IsNearlyEqual(vector, otherVector, epsilon);
	}
}
