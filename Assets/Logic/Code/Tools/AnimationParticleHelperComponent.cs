using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationParticleHelperComponent : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particleSystem;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

	public void StartParticleEffect(AnimationEvent evt)
	{
        particleSystem[evt.intParameter].Play();

	}
}
