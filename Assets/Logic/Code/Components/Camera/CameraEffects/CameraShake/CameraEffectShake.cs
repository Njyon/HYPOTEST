using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class CameraEffectShake : ACameraEffect
{
	CameraShakeScriptableObject shake;

	public CameraEffectShake(CameraController controller, CameraShakeScriptableObject shake, bool isPaused = false) : base(controller, shake.Duration, isPaused)
	{
		this.shake = shake;
	}

	public override void DoEffect()
	{
		if (IsPaused) return;
	}

	public override void CleanUp()
	{

	}

	public override void Update(float deltaTime)
	{
		if (IsPaused) return;
		if (!Timer.IsFinished)
		{
			//float xOffset = Mathf.Sin(Time.time * shake.Frequency) * shake.Amplitude;
			//float yOffset = Mathf.Cos(Time.time * shake.Frequency) * shake.Amplitude;

			Vector3 lol = ((Mathf.PerlinNoise(Timer.CurrentTime * shake.Frequency.x, Timer.CurrentTime * shake.Frequency.x * Random.Range(shake.RandomValue.x, shake.RandomValue.y)) - 0.5f) * shake.Amplitude.x * Vector3.right +
							(Mathf.PerlinNoise(Timer.CurrentTime * shake.Frequency.y * Random.Range(shake.RandomValue.x, shake.RandomValue.y), Timer.CurrentTime * shake.Frequency.y) - 0.5f) * shake.Amplitude.y * Vector3.up);

			lol.x = Mathf.Clamp(lol.x, shake.Clamp.x, shake.Clamp.y);
			lol.y = Mathf.Clamp(lol.y, shake.Clamp.x, shake.Clamp.y);

			switch (shake.ShakeAxis)
			{
				case ECameraShakeAxis.X:
					CameraController.CameraEffectOffset += new Vector3(lol.x, 0f, 0f);
					break;
				case ECameraShakeAxis.Y:
					CameraController.CameraEffectOffset += new Vector3(0f, lol.y, 0f);
					break;
				default:
					CameraController.CameraEffectOffset += new Vector3(lol.x, lol.y, 0f);
					break;
			}
		}
		else
		{
			EndEffect();
		}
		Timer.Update(deltaTime);
	}
}
