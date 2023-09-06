using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	[SerializeField] List<string> scenes = new List<string>();
	List<AsyncOperation> asyncOperations = new List<AsyncOperation>();

	void Awake()
	{
		UIManager.Instance.LoadLoadingScreen();
		for(int i = 0; i < scenes.Count; i++)
		{
			asyncOperations.Add(SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive));
		}	
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
		LoadingDone();
		yield return null;
	}
}
