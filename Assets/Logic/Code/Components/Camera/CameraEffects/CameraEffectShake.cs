using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraEffectShake : ACameraEffect
{
	public float amplitude = 0.7f;   
	public float frequency = 10.0f;

	public CameraEffectShake(CameraController controller, float effectDuration = 1f, bool isPaused = false) : base(controller, effectDuration, isPaused)
	{ }

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
			float xOffset = Mathf.Sin(Time.time * frequency) * amplitude;
			float yOffset = Mathf.Cos(Time.time * frequency) * amplitude;

			Vector3 lol = ((Mathf.PerlinNoise(Timer.CurrentTime * frequency, Timer.CurrentTime * frequency * Random.Range(.5f, 1.5f)) - 0.5f) * amplitude * Vector3.right +
							(Mathf.PerlinNoise(Timer.CurrentTime * frequency * Random.Range(.5f, 1.5f), Timer.CurrentTime * frequency) - 0.5f) * amplitude * Vector3.up);

			lol.x = Mathf.Clamp(lol.x, -1f, 1f);
			lol.y = Mathf.Clamp(lol.y, -1f, 1f);

			CameraController.CameraEffectOffset += new Vector3(lol.x, lol.y, 0f);
		}
		else
		{
			EndEffect();
		}
		Timer.Update(deltaTime);
	}
}
