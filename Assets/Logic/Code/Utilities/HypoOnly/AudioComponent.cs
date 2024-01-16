using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioComponent : MonoBehaviour
{
	public delegate void OnAudioSpurceFinished(AudioComponent audioComponent);
	public OnAudioSpurceFinished onAudioSpurceFinished;

    [SerializeField] AudioSource audioSource;
	public AudioSource AudioSource { get { return audioSource; } }

	public void Play(SoundEffect soundEffect)
	{
		audioSource.volume = soundEffect.Volume;
		audioSource.pitch = soundEffect.Pitch;
		audioSource.clip = soundEffect.audioClip;
		audioSource.Play();

		WaitUntilFinished(audioSource.clip.length);
	}

	async void WaitUntilFinished(float time)
	{
		await new WaitForSecondsRealtime(time);
		if (this != null && onAudioSpurceFinished != null) onAudioSpurceFinished(this);
	}
}
