using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECameraShakeAxis
{
	Both,
	X,
	Y,
}

[CreateAssetMenu(fileName = "New CameraShake", menuName = "Assets/CameraEffects/Shakes")]
public class CameraShakeScriptableObject : ScriptableObject
{
	public Vector2 Amplitude = new Vector2(0.7f, 0.7f);
	public Vector2 Frequency = new Vector2(10.0f, 10f);
	public float Duration = 1f;
	public Vector2 Clamp = new Vector2(-1f, 1f);
	public Vector2 RandomValue = new Vector2(0.5f, 1.5f);
	public ECameraShakeAxis ShakeAxis = ECameraShakeAxis.Both;
}
