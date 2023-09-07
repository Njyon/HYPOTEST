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
		Ultra.HypoUttilies.DeleteAllGameModes();
		Ultra.HypoUttilies.CreateGameMode<StoryGameMode>();

        UIManager.Instance.LoadDifficultySelection();
    }

    public void OnTrainingModusPressed()
    {
		Ultra.HypoUttilies.DeleteAllGameModes();
		Ultra.HypoUttilies.CreateGameMode<TrainingGameMode>();

		SceneLoaderManager.Instance.LoadTrainingsMap();
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
