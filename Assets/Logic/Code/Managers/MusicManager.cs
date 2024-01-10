using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class MusicManager : Singelton<MusicManager>
{
    bool shouldStop = false;
    float volumeTarget = 0;
    float lerpSpeedUp = 0.05f;
    float lerpSpeedDown = 0.008f;
    int lastIndex = -1;

    AudioSource musicSource;
	AudioSource MusicSource { 
        get { 
            if (musicSource == null)
            {
                musicSource = GameObject.Instantiate(GameAssets.Instance.MusicObject);
                musicSource.gameObject.name = ">> " + musicSource.gameObject.name;
                musicSource.volume = 0;
            }
            return musicSource; 
        } 
    }

    void Update()
    {
        if (MusicSource.isPlaying)
        {
			if (MusicSource != null && MusicSource.volume != volumeTarget)
			{
				MusicSource.volume = Mathf.Lerp(MusicSource.volume, volumeTarget, Time.deltaTime * volumeTarget > MusicSource.volume ? lerpSpeedUp : lerpSpeedDown);
			}
            if (MusicSource.volume == 0)
            {
                Stop();
				MusicSource.Stop();
			}
		}
       
    }

    public void Play()
    {
        if (!MusicSource.isPlaying)
        {
            shouldStop = false;

			MusicSource.clip = GetMusicClip();

			MusicSource.Play();
        }
    }

    public void Stop()
    {
        if (MusicSource.isPlaying && !shouldStop)
        {
            shouldStop = true;
            volumeTarget = 0;
		}
    }

    public void SetVolumeTarget(float volume)
    {
        if (shouldStop) return;
		volumeTarget = volume;
	}

    AudioClip GetMusicClip()
    {
        int index = UnityEngine.Random.Range(0, GameAssets.Instance.MusicTracks.Count);
        if (index == lastIndex)
		{
            index++;
            index = index % GameAssets.Instance.MusicTracks.Count;
		}

		lastIndex = index;
        return GameAssets.Instance.MusicTracks[index];
    }
}
