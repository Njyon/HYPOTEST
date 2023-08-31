using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRatingCombonent : RecourceBase
{
	public CombatRatingCombonent(float startValue, float maxValue = float.PositiveInfinity, float minValue = 0, float defaultValueChangePerSecond = 0) : base(startValue, maxValue, minValue, defaultValueChangePerSecond)
	{
	}

}
