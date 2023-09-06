using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySelection : UIBase
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

	public void SelectDifficulty(int difficulty)
	{
		GameDifficultyLevel difficultyLevel = (GameDifficultyLevel)difficulty;
		GameModeBase.Instance.GameDifficultyLevel = difficultyLevel;
	}

	public override void StartRemovingUI()
	{
		StartRemovingDifficultySelection();
	}

	void StartRemovingDifficultySelection()
	{
		fadeOut.PlayFeedbacks();
	}

	public void RemoveDifficultySelection()
	{
		RemoveUI();
	}
}
