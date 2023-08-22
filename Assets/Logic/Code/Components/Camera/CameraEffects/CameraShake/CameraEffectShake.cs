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

			Vector3 shake = ((Mathf.PerlinNoise(Timer.CurrentTime * this.shake.Frequency.x, Timer.CurrentTime * this.shake.Frequency.x * Random.Range(this.shake.RandomValue.x, this.shake.RandomValue.y)) - 0.5f) * this.shake.Amplitude.x * Vector3.right +
							(Mathf.PerlinNoise(Timer.CurrentTime * this.shake.Frequency.y * Random.Range(this.shake.RandomValue.x, this.shake.RandomValue.y), Timer.CurrentTime * this.shake.Frequency.y) - 0.5f) * this.shake.Amplitude.y * Vector3.up);

			shake.x = Mathf.Clamp(shake.x, this.shake.Clamp.x, this.shake.Clamp.y);
			shake.y = Mathf.Clamp(shake.y, this.shake.Clamp.x, this.shake.Clamp.y);

			switch (this.shake.ShakeAxis)
			{
				case ECameraShakeAxis.X:
					CameraController.CameraEffectOffset += new Vector3(shake.x, 0f, 0f);
					break;
				case ECameraShakeAxis.Y:
					CameraController.CameraEffectOffset += new Vector3(0f, shake.y, 0f);
					break;
				default:
					CameraController.CameraEffectOffset += new Vector3(shake.x, shake.y, 0f);
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
