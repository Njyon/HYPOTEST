using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IntExtensions
{
    public static bool IsNearlyEqual(this int a, int b, int epsilon)
    {
        return Ultra.Utilities.IsNearlyEqual(a, b, epsilon);
    } 
}
