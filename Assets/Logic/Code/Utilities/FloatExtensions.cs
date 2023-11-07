using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions 
{
	public static bool IsNearlyEqual(this float a, float b, float epsilon)
	{
		return Ultra.Utilities.IsNearlyEqual(a, b, epsilon);
	}
}
