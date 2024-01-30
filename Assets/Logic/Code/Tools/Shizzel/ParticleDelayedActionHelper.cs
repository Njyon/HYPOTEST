using System.Collections;
using System.Collections.Generic;
using Ultra;
using UnityEngine;

public class ParticleDelayedActionHelper : MonoBehaviour
{
    Timer timer;
    ParticleSystem ps;

    void Update()
    {
        if (timer != null)
            timer.Update(Time.deltaTime);
    }

	void OnDestroy()
	{
		timer.onTimerFinished -= OnDetachFromParentTimerFinished;
	}

	public void DetachParticleEffectFromParentAfterTime(ParticleSystem particleSystem, float time)
    {
        timer = new Timer(time);
        timer.Start();

        timer.onTimerFinished += OnDetachFromParentTimerFinished;

        ps = particleSystem;
	}

	void OnDetachFromParentTimerFinished()
	{
		timer.onTimerFinished -= OnDetachFromParentTimerFinished;

        ps.transform.parent = null;
	}
}
