using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class SceneLoader : MonoBehaviour
{
	public bool isMasterLoader = true;
	[SerializeField] List<string> scenes = new List<string>();
	List<AsyncOperation> asyncOperations = new List<AsyncOperation>();

	void Awake()
	{
		if (Application.isPlaying && isMasterLoader)
			UIManager.Instance.LoadLoadingScreen();
		for(int i = 0; i < scenes.Count; i++)
		{
			if (Application.isPlaying)
			{
				if (SceneManager.GetSceneByPath(scenes[i]).IsValid()) continue;
				asyncOperations.Add(SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive));
			}
#if UNITY_EDITOR
			else
				EditorSceneManager.OpenScene(scenes[i], OpenSceneMode.Additive);
#endif
		}
		if (Application.isPlaying)
			StartCoroutine(LoadScenes());
	}

	void LoadingDone()
	{
		if (Application.isPlaying)
			UIManager.Instance.UnloadLoadingScreen();
		if (isMasterLoader)
			SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenes[0]));
	}

	IEnumerator LoadScenes()
	{
		bool loading = true;
		while(loading)
		{
			loading = false;
			foreach(AsyncOperation asyncOperation in asyncOperations)
			{
				if (asyncOperation == null) { loading = true; continue; }
				if (!asyncOperation.isDone) loading = true;
			}
			yield return new WaitForEndOfFrame();
		}
		LoadingDone();
		yield return null;
	}
}
