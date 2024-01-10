using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct UIStackELement
{
	public UIStackELement(string name, UIBase uiClass)
	{
		this.name = name;
		this.uiClass = uiClass;
	}

	public string name;
	public UIBase uiClass;
}

public class UIManager : Singelton<UIManager>
{
	public delegate void SceneLoadEvent();
	public delegate void OnAllUisUnloaded();
	public OnAllUisUnloaded onAllUIsUnloaded;

	[Header("SceneNames")]
	[SerializeField] string titelScreenName = "TitelScreen";
	[SerializeField] string mainMenuName = "MainMenu(CrossPlatform)";
	[SerializeField] string startSceneName = "StartScene";
	[SerializeField] string gameModeSelectionName = "GameModeSelector";
	[SerializeField] string difficultySelectionName = "DifficultySelection";
	[SerializeField] string loadingScreenName = "LoadingSceneUI";
	[SerializeField] string playerUISceneName = "PlayerUIScene";

	Canvas canvas;
	public Canvas Canvas { 
		get 
		{ 
			return canvas; 
		} 
		set
		{
			canvas = value;
		}
	}

	Stack<UIStackELement> uiStack = new Stack<UIStackELement>();

	public Stack<UIStackELement> UIStack { get { return uiStack; } }	

	bool showTitelScreen = true;

	GameObject enemyInfoHolder;
	GameObject EnemyInfoHolder { 
		get {
			if (enemyInfoHolder == null)
			{
				enemyInfoHolder = new GameObject(">> EnemyInfoHolder");
				enemyInfoHolder.transform.parent = Canvas.transform;
			}
			return enemyInfoHolder;
		} 
	}
	EnemyInfoPool enemyInfoPool;
	public EnemyInfoPool EnemyInfoPool { 
		get { 
			if (enemyInfoPool == null)
			{
				enemyInfoPool = new EnemyInfoPool(GameAssets.Instance.EnemyInfo, EnemyInfoHolder);
			}
			if (enemyInfoPool.Parent.Equals(null)) enemyInfoPool.SetParent(EnemyInfoHolder);
			return enemyInfoPool;
		} 
	}

	void Awake()
	{
		Application.targetFrameRate = 150;

		// Might be wrong
		Object[] lol = FindObjectsOfType<UIManager>();
		if (lol.Length > 1)
		{
			GameObject.Destroy(this);
		}

		DontDestroyOnLoad(gameObject);
		//LoadTitelScreenIfWanted();
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
			}else
			{
				LoadMainMenu();
			}
		}
	}

	async private void LoadSceneAsync(string name, LoadSceneMode loadMode, SceneLoadEvent sceneEvent)
	{
		Scene sceneToBeLoaded = SceneManager.GetSceneByName(name);
		if (!sceneToBeLoaded.isLoaded)
		{
			UIStackELement currentUI;
			if (uiStack.TryPeek(out currentUI))
			{
				if (currentUI.uiClass != null)
				{
					currentUI.uiClass.StartRemovingUI();
					await new WaitUntil(() => currentUI.uiClass.CanBeRemoved());
					UnloadScene(currentUI.name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, null);
				}
			}

			var asyncOperation = SceneManager.LoadSceneAsync(name, loadMode);
			asyncOperation.completed += (e) => { if (sceneEvent != null) sceneEvent(); };
			uiStack.Push(new UIStackELement(name, null));
		}
	}


	private void UnloadScene(string sceneName, UnloadSceneOptions unloadOptions, SceneLoadEvent sceneEvent)
	{
		Scene scene = SceneManager.GetSceneByName(sceneName);
		if (scene.isLoaded)
		{
			var asyncOperation = SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
			asyncOperation.completed += (e) => { if (sceneEvent != null) sceneEvent(); };
		} else {
			if (sceneEvent != null) sceneEvent();
		}
	}

	public void LoadMainMenu()
	{
		LoadSceneAsync(mainMenuName, LoadSceneMode.Single, null);
	}

	public void LoadGameModeSelection()
	{
		LoadSceneAsync(gameModeSelectionName, LoadSceneMode.Additive, null);
	}

	public void LoadDifficultySelection()
	{
		LoadSceneAsync(difficultySelectionName, LoadSceneMode.Additive, null);
	}

	public void LoadLoadingScreen()
	{
		LoadSceneAsync(loadingScreenName, LoadSceneMode.Additive, null);
	}

	public void UnloadLoadingScreen()
	{
		UnloadScene(loadingScreenName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, null);
	}

	public void LoadPlayerUI(SceneLoadEvent sceneLoadEvent)
	{
		LoadSceneAsync(playerUISceneName, LoadSceneMode.Additive, sceneLoadEvent);
	}

	public void UIBack()
	{
		UIStackELement currentUI;
		if (uiStack.TryPeek(out currentUI))
		{
			if (currentUI.uiClass == null) return;
			currentUI.uiClass.StartRemovingUI();
			currentUI.uiClass.onRemoveUI += RemoveUI;
		}
	}

	public void UnloadAll()
	{
		if (uiStack != null && uiStack.Count > 0)
		{
			UIStackELement uiElement = uiStack.Pop();
			UnloadScene(uiElement.name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, UnloadAll);
		}
		else
		{
			AllUIsUnloaded();
		}
	}

	void AllUIsUnloaded()
	{
		uiStack.Clear();
		if (onAllUIsUnloaded != null) onAllUIsUnloaded();
	}

	void RemoveUI()
	{
		uiStack.Peek().uiClass.onRemoveUI -= RemoveUI;
		UnloadScene(uiStack.Pop().name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, null);
		LoadSceneAsync(uiStack.Peek().name, LoadSceneMode.Additive, null);
	}

	public EnemyInfo GetEnemyInfo(GameCharacter character)
	{
		EnemyInfo enemyInfo = EnemyInfoPool.GetValue();
		enemyInfo.Init(character);
		return enemyInfo;
	}

	public void ReturnEnemyInfo(EnemyInfo enemyInfo)
	{
		EnemyInfoPool.ReturnValue(enemyInfo);
	}
}
