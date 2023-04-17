using System.Runtime.CompilerServices;
using UnityEngine;

public static class ColorExtensions
{
	public static Color WithAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static Color Random
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 1f);
		}
	}
}