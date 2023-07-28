using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationParticleHelperComponent : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

	public void StartParticleEffect()
	{
        particleSystem.Play();

	}
}
