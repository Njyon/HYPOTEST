using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeMenu : UIBase
{
    [SerializeField] MMF_Player fadeIn;
    [SerializeField] MMF_Player fadeOut;

	

	void Awake()
    {
        LoadedUI();
		fadeIn.Initialization();
		fadeOut.Initialization();
    }

	void Start()
	{
		fadeIn.PlayFeedbacks();
	}

	public void OnStoryModusPressed()
    {
        GameObject.Destroy(GameModeBase.Instance.gameObject);
        StoryGameMode.Instance.Create();
        UIManager.Instance.LoadDifficultySelection();
    }

    public void OnTrainingModusPressed()
    {
		GameObject.Destroy(GameModeBase.Instance);
		TrainingGameMode.Instance.Create();
	}

	public void Back()
	{
		UIManager.Instance.UIBack();
	}

	public override void StartRemovingUI()
	{
        StartRemovingGameModeMenu();
	}

    void StartRemovingGameModeMenu()
    {
		fadeOut.PlayFeedbacks();
	}

    public void RemoveingGameModeMenu()
    {
        RemoveUI();
    }
}
