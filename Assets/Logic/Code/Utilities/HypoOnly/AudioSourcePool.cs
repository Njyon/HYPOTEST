using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviourPoolBase<AudioComponent>
{
	public AudioSourcePool(AudioComponent instance, GameObject parent) : base(instance, parent)
	{ }

	public AudioSourcePool(AudioComponent instance, GameObject parent, int minSize) : base(instance, parent, minSize)
	{ }

	public override AudioComponent GetValue()
	{
		if (!IsTStackInit) InitStack();

		if (NoMoreTInStack)
			SpawnValue();

		AudioComponent ac = stack.Pop();

		ac.onAudioSpurceFinished += OnAudioSourceFinished;
		ActivateValue(ac);
		return ac;
	}

	protected override void ActivateValue(AudioComponent value)
	{
		value.gameObject.SetActive(true);
	}

	protected override void DeactivateValue(AudioComponent value)
	{
		value.gameObject.SetActive(false);
	}

	void OnAudioSourceFinished(AudioComponent value)
	{
		value.onAudioSpurceFinished -= OnAudioSourceFinished;
		ReturnValue(value);
	}
}
