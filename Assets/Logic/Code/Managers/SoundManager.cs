using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;

[Serializable]
public class SoundEffect
{
	public AudioClip audioClip;

	[Range(0f, 1f)]
	public float volume = 1f;
	[Range(0.1f, 3f)]
	public float pitch = 1f;

	public bool randomPitch;
	[ConditionalField("randomPitch")][Range(0f, 1f)] public float randPitch = 0f;
	public bool randomValue;
	[ConditionalField("randomValue")][Range(0f, 1f)] public float randVolume = 0f;

	public float Volume { get { return randomValue ? UnityEngine.Random.Range(volume - randVolume, volume + randVolume) : volume; } }
	public float Pitch { get { return randomPitch ? UnityEngine.Random.Range(pitch - randPitch, pitch + randPitch) : pitch; } }
}

public class SoundManager : Singelton<SoundManager> 
{
	AudioSourcePool audioSourcePool;
	GameObject SoundHolder;

	void Awake()
	{
		SoundHolder = CreateHolder();
		audioSourcePool = new AudioSourcePool(GameAssets.Instance.SoundObject, SoundHolder, 10);
	}

	public void PlaySound(SoundEffect soundEffect)
	{
		if (soundEffect == null) return;
		if (audioSourcePool.Parent == null) audioSourcePool.SetParent(CreateHolder());
		AudioComponent ac = audioSourcePool.GetValue();
		ac.Play(soundEffect);
	}

	GameObject CreateHolder()
	{
		return new GameObject(">> Sound Holder");
	}
}
