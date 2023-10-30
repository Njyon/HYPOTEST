using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : Singelton<SceneLoaderManager>
{
	public void LoadStoryLevel01()
	{
		LoadScene("TestLevelMain01");
	}

	public void LoadTrainingsMap()
	{
		LoadScene("TrainingMap");
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
