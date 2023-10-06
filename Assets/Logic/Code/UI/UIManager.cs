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
	[SerializeField] string mainMenuName = "MainMenu";
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

	Stack<EnemyInfo> enemyInfoStack;
	bool NoMoreEnemyInfo => enemyInfoStack.Count <= 0;
	bool IsEnemyStackInit => enemyInfoStack != null;
	int enemyStackMinSize = 5;

	void Awake()
	{
		// Might be wrong
		Object[] lol = FindObjectsOfType<UIManager>();
		if (lol.Length > 1)
		{
			GameObject.Destroy(this);
		}

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
		}
	}

	public void LoadMainMenu()
	{
		LoadSceneAsync(mainMenuName, LoadSceneMode.Additive, null);
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
		if (character == null) return null;
		if (!IsEnemyStackInit) InitEnemyStack();
	
		if (NoMoreEnemyInfo)
			SpawnEnemyInfo();
		EnemyInfo enemyInfo = enemyInfoStack.Pop();
		enemyInfo.gameObject.SetActive(true);
		enemyInfo.Init(character);
		return enemyInfo;
	}

	public void ReturnEnemyInfo(EnemyInfo enemyInfo)
	{
		if (enemyInfo == null) return;
		enemyInfo.gameObject.SetActive(false);
		if (IsEnemyStackInit)
			enemyInfoStack.Push(enemyInfo);
	}

	void InitEnemyStack()
	{
		enemyInfoStack = new Stack<EnemyInfo>();
		for (int i = 0; i < enemyStackMinSize; i++)
		{
			SpawnEnemyInfo();
		}
	}

	private void SpawnEnemyInfo()
	{
		if (Canvas == null)
		{
			Ultra.Utilities.Instance.DebugErrorString("UIManager", "InitEnemyInfo", "Canvas was null!");
			return;
		}
		GameObject enemyInfoObj = GameObject.Instantiate(GameAssets.Instance.EnemyInfo, Canvas.transform);
		EnemyInfo enemyInfo = enemyInfoObj.GetComponent<EnemyInfo>();
		enemyInfoStack.Push(enemyInfo);
		enemyInfoObj.SetActive(false);
	}
}
