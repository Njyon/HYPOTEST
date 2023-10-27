using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemCallBackComponent : MonoBehaviour
{
	public delegate void ParticleEventCallBack(GameObject go);
	public ParticleEventCallBack onParticleSystemStopped;
	public ParticleEventCallBack onParticleSystemTriggered;
	public delegate void ParticleCollsionEventCallBack(GameObject go, GameObject other);
	public ParticleCollsionEventCallBack onParticleSystemCollision;


	private void OnParticleSystemStopped()
	{
		if (onParticleSystemStopped != null) onParticleSystemStopped(this.gameObject);
	}

	private void OnParticleTrigger()
	{
		if (onParticleSystemTriggered != null) onParticleSystemTriggered(this.gameObject);
	}

	private void OnParticleCollision(GameObject other)
	{
		if (onParticleSystemCollision != null) onParticleSystemCollision(this.gameObject, other);
	}
}
