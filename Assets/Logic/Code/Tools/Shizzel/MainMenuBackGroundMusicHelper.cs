using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackGroundMusicHelper : MonoBehaviour
{
    [SerializeField] AudioSource backGroundMusic;
    [SerializeField] float backGroundMusicTargetVolume;
    [SerializeField] AnimationCurve backGroundMusicCurve;
    float timeStamp;

    void Awake()
    {
        backGroundMusic.volume = 0f;
        timeStamp = Time.time;

	}


    void Update()
    {
        if (backGroundMusic.volume < backGroundMusicTargetVolume)
        {
            backGroundMusic.volume = backGroundMusicCurve.Evaluate(Time.time - timeStamp);
        }
    }
}
