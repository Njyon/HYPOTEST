using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class SceneLoader : MonoBehaviour
{
	public bool isMasterLoader = true;
	[SerializeField] List<string> scenes = new List<string>();

	void Start()
	{
		if (Application.isPlaying && isMasterLoader)
			UIManager.Instance.LoadLoadingScreen();
		for (int i = 0; i < scenes.Count; i++)
		{
			if (Application.isPlaying)
			{
				string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenes[i]);
				if (SceneManager.GetSceneByName(sceneName).IsValid() || SceneManager.GetSceneByPath(scenes[i]).IsValid()) continue;
				LoadingChecker.Instance.AsyncOperations.Add(SceneManager.LoadSceneAsync(System.IO.Path.GetFileNameWithoutExtension(scenes[i]), LoadSceneMode.Additive));
			}
#if UNITY_EDITOR
			else
				EditorSceneManager.OpenScene(scenes[i], OpenSceneMode.Additive);
#endif
		}
		if (Application.isPlaying)
		{
			// Load Ingame Pause Menu
			LoadingChecker.Instance.AsyncOperations.Add(SceneManager.LoadSceneAsync("InGameSettingsMenu", LoadSceneMode.Additive));

			LoadingChecker.Instance.onLoadingFinished += LoadingDone;
			LoadingChecker.Instance.StartCheckingLoading();
		}
	}

	async void LoadingDone()
	{
		if (isMasterLoader)
		{
			if (scenes.Count > 0)
			{
				if (Application.isPlaying)
				{
					string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenes[0]);
					SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
				}
				else
					SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenes[0]));
			}
		
		}

		await new WaitForEndOfFrame();

		if (Application.isPlaying)
			UIManager.Instance.UnloadLoadingScreen();
	}


}
