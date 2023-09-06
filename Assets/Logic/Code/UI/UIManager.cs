using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct UIs
{
	public UIs(string name, UIBase uiClass)
	{
		this.name = name;
		this.uiClass = uiClass;
	}

	public string name;
	public UIBase uiClass;
}

public class UIManager : Singelton<UIManager>
{
	public delegate void SceneEvent();

	[Header("SceneNames")]
	[SerializeField] string titelScreenName = "TitelScreen";
	[SerializeField] string mainMenuName = "MainMenu";
	[SerializeField] string startSceneName = "StartScene";


	Stack<UIs> uiStack = new Stack<UIs>();

	public Stack<UIs> UIStack { get { return uiStack; } }	

	bool showTitelScreen = true;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		LoadTitelScreenIfWanted();
	}

	void Start()
	{

	}

	void LoadTitelScreenIfWanted()
	{
		Scene startScene = SceneManager.GetSceneByName(startSceneName);
		if (startScene.IsValid())
		{
			if (showTitelScreen)
			{
				LoadSceneAsync(titelScreenName, LoadSceneMode.Additive, null);
				showTitelScreen = false;
			}
		}
	}

	async private void LoadSceneAsync(string name, LoadSceneMode loadMode, SceneEvent sceneEvent)
	{
		UIs currentUI;
		if (uiStack.TryPeek(out currentUI))
		{
			if (currentUI.uiClass != null)
			{
				currentUI.uiClass.StartRemovingUI();
				await new WaitUntil(() => currentUI.uiClass.CanBeRemoved());
				UnloadScene(currentUI.name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, null);
			}
		}

		Scene titelScreenScene = SceneManager.GetSceneByName(name);
		if (!titelScreenScene.isLoaded)
		{
			var asyncOperation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
			asyncOperation.completed += (e) => { if (sceneEvent != null) sceneEvent(); };
			uiStack.Push(new UIs(name, null));
		}
	}


	private void UnloadScene(string sceneName, UnloadSceneOptions unloadOptions, SceneEvent sceneEvent)
	{
		Scene titelScreenScene = SceneManager.GetSceneByName(sceneName);
		if (titelScreenScene.isLoaded)
		{
			var asyncOperation = SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
			asyncOperation.completed += (e) => { if (sceneEvent != null) sceneEvent(); };
		}
	}

	public void LoadMainMenu()
	{
		LoadSceneAsync(mainMenuName, LoadSceneMode.Additive, null);
	}

	public void RemoveMainMenu()
	{
		Scene mainMenuScene = SceneManager.GetSceneByName(mainMenuName);
		if (mainMenuScene.isLoaded)
		{
			SceneManager.UnloadSceneAsync(mainMenuName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
		}
	}

	public void UIBack()
	{
		UIs currentUI;
		if (uiStack.TryPeek(out currentUI))
		{
			if (currentUI.uiClass == null) return;
			currentUI.uiClass.StartRemovingUI();
			currentUI.uiClass.onRemoveUI += RemoveUI;
		}
	}

	void RemoveUI()
	{
		uiStack.Peek().uiClass.onRemoveUI -= RemoveUI;
		UnloadScene(uiStack.Pop().name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, null);
		LoadSceneAsync(uiStack.Peek().name, LoadSceneMode.Additive, null);
	}
}
