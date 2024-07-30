using UnityEngine;

public class AnimationCallback : StateMachineBehaviour
{
	public string triggerName;
	Ultra.Timer timer;
	
	Ultra.Timer Timer
	{
		get
		{
			if (timer == null)
			{
				timer = new Ultra.Timer();
			}
			return timer;
		}
	}

	Animator animator;

	// Wird aufgerufen, wenn ein Zustand dieses Verhaltens betritt
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Timer.Start(stateInfo.length);
		Timer.onTimerFinished += OnTimerFinished;
		this.animator = animator;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);
		Timer.Update(Time.deltaTime);
	}

	// Wird aufgerufen, wenn ein Zustand dieses Verhaltens verlässt
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	void OnTimerFinished()
	{
		Timer.onTimerFinished -= OnTimerFinished;
		animator.SetTrigger(triggerName);
	}
}
