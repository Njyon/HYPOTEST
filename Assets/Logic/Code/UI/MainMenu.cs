using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : UIBase
{

	void Awake()
	{
		LoadedUI();

	}

	public void PlayButtonPressed()
	{

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
		RemovingMainMenu();
	}

	public void RemovingMainMenu()
	{
		RemoveUI();
	}
}
