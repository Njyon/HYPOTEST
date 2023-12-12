using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryModusPanelHelper : MonoBehaviour
{
	[SerializeField] ChapterManager chapterManager;

	private void Awake()
	{
		if (chapterManager == null)
		{
			chapterManager = GetComponent<ChapterManager>();
		}
		if (chapterManager == null) 
		{
			Ultra.Utilities.Instance.DebugErrorString("StoryModusPanelHelper", "Awake", "chapterManager was null!");
		}
	}

	public void OnPlay()
	{
		if (chapterManager == null) return;
		StoryGameMode storyGameMode = DestroyAllAndCreateNewGameMode<StoryGameMode>();

		GameDifficultyLevel difficultyLevel = GetDifficultySetting();
		storyGameMode.GameDifficultyLevel = difficultyLevel;

		SceneLoaderManager.Instance.LoadStoryLevel01();
	}

	public void OnRepeat()
	{
		if (chapterManager == null) return;

		StoryGameMode storyGameMode = DestroyAllAndCreateNewGameMode<StoryGameMode>();

		GameDifficultyLevel difficultyLevel = GetDifficultySetting();
		storyGameMode.GameDifficultyLevel = difficultyLevel;

		SceneLoaderManager.Instance.LoadStoryLevel01();
	}

	public void onContinue()
	{
		if (chapterManager == null) return;
		// Get Last DificultyLevel or discord option

		SceneLoaderManager.Instance.LoadStoryLevel01();
	}

	GameDifficultyLevel GetDifficultySetting()
	{
		ChapterIdentifier ci = chapterManager.Identifiers[chapterManager.currentChapterIndex];
		return (GameDifficultyLevel)ci.difficultySelector.currentModeIndex;
	}

	T DestroyAllAndCreateNewGameMode<T>() where T : GameModeBase
	{
		Ultra.HypoUttilies.DeleteAllGameModes();
		return Ultra.HypoUttilies.CreateGameMode<T>();
	}
}
