using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : UIBase
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

	public void PlayButtonPressed()
	{
		UIManager.Instance.LoadGameModeSelection();
	}

	public void SettingButtonPressed()
	{
		
	}

	public void ExitButtonPressed()
	{
		Application.Quit();
	}

	public override void StartRemovingUI()
	{
		StartRemovingMainMenu();
	}

	void StartRemovingMainMenu()
	{
		fadeOut.PlayFeedbacks();
	}

	public void RemovingMainMenu()
	{
		RemoveUI();
	}
}
