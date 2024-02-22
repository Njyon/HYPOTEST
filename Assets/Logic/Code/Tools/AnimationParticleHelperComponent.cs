using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationParticleHelperComponent : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particleSystem;
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip clip;
    AnimatorOverrideController overrideController;


	void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null && clip != null)
		{
			overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
			overrideController["TestState"] = clip;
			animator.runtimeAnimatorController = overrideController;
		}
    }

    void Update()
    {
        
    }

	public void StartParticleEffect(AnimationEvent evt)
	{
        particleSystem[evt.intParameter].Play();

	}
}
