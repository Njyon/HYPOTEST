using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : Singelton<SceneLoaderManager>
{
	public void LoadStoryLevel00()
	{
		LoadScene("TestLevelMain01");
	}

	public void LoadStoryLevel01()
	{
		LoadScene("StoryLevelArt");
	}

	public void LoadTrainingsMap()
	{
		LoadScene("TrainingMap");
	}

	public void LoadLevel(int index)
	{
		switch (index)
		{
			case -1:
				LoadTrainingsMap();
				break;
			case 0:
				LoadStoryLevel00();
				break;
			default: 
				LoadStoryLevel01();
				break;
		}
	}

	private void LoadScene(string sceneName)
	{
		UIManager.Instance.LoadLoadingScreen();
		SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
	}

	IEnumerator LoadSceneAsync(AsyncOperation operation)
	{
		while (!operation.isDone)
		{
			yield return new WaitForEndOfFrame();
		}
	}
}
