using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class SceneLoader : MonoBehaviour
{
	[SerializeField] List<string> scenes = new List<string>();
	List<AsyncOperation> asyncOperations = new List<AsyncOperation>();

	void Awake()
	{
		if (Application.isPlaying)
			UIManager.Instance.LoadLoadingScreen();
		for(int i = 0; i < scenes.Count; i++)
		{
			if (Application.isPlaying)
			{
				if (SceneManager.GetSceneByPath(scenes[i]).IsValid()) continue;
				asyncOperations.Add(SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive));
			}
			else
				EditorSceneManager.OpenScene(scenes[i], OpenSceneMode.Additive);
		}
		if (Application.isPlaying)
			StartCoroutine(LoadScenes());
	}

	void LoadingDone()
	{
		UIManager.Instance.UnloadLoadingScreen();
	}

	IEnumerator LoadScenes()
	{
		bool loading = true;
		while(loading)
		{
			loading = false;
			foreach(AsyncOperation asyncOperation in asyncOperations)
			{
				if (!asyncOperation.isDone) loading = true;
			}
			yield return new WaitForEndOfFrame();
		}
		if (Application.isPlaying)
			LoadingDone();
		yield return null;
	}
}
