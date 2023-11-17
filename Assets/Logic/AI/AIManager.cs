using Megumin.Binding;
using Megumin.GameFramework.AI.BehaviorTree;
using Megumin.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using static Megumin.GameFramework.AI.BehaviorTree.BehaviorTreeRunner;

public class HyppoliteManagableAI
{
	public HyppoliteManagableAI(GameCharacter gameCharacter, BehaviorTreeRunner btr)
	{
		this.gameCharacter = gameCharacter;
		this.btr = btr;


		if (!this.btr.BehaviourTree.Variable.Contains("CanMeleeAttack"))
		{
			RefVar_Bool canMeleeAttack = new RefVar_Bool();
			canMeleeAttack.RefName = "CanMeleeAttack";
			canMeleeAttack.value = false;

			this.btr.BehaviourTree.InitAddVariable(canMeleeAttack);
		}
		if (!this.btr.BehaviourTree.Variable.Contains("MovementTarget"))
		{
			RefVar_Vector3 movementTarget = new RefVar_Vector3();
			movementTarget.RefName = "MovementTarget";
			movementTarget.value = Vector3.zero;

			this.btr.BehaviourTree.InitAddVariable(movementTarget);
		}
	}

	public GameCharacter gameCharacter;
	public BehaviorTreeRunner btr;
}

public class AIManager : Singelton<AIManager>
{
	List<HyppoliteManagableAI> managableAIs = new List<HyppoliteManagableAI>();
	List<HyppoliteManagableAI> meleeAIs = new List<HyppoliteManagableAI>();
	List<HyppoliteManagableAI> meleeAIsThatCanAttack = new List<HyppoliteManagableAI>();
	NativeArray<float> distances;
	JobHandle jobHandle;
	HyppoliteManagableAI[] sortedMeleeAIs;
	[SerializeField] int meleeCharacterAttackAmount = 1;
	GameModeBase gameMode;
	GameObject btrParent = null;

	Stack<BehaviorTreeRunner> behaviorTreeRunners;
	public bool NoMoreBehaviorTrees => behaviorTreeRunners.Count <= 0;
	public bool IsBehaviorTreeStackInit => behaviorTreeRunners != null;
	int behaviorStackMinSize = 10;
	int behaviourTreeInits = 0;
	int createdBehaviourTrees = 0;
	public bool AllBehaviourTreesInit { get { return behaviourTreeInits >= (createdBehaviourTrees < behaviorStackMinSize ? behaviorStackMinSize : createdBehaviourTrees); } }

	public int MeleeCharacterAttackAmount { get { return meleeCharacterAttackAmount; } }
	public int ManagableAIsCount { get { return managableAIs.Count; } }
	public int MeleeAIsCount { get { return meleeAIs.Count; } }
	public int MeleeAIsThatCanAttackCount { get { return meleeAIsThatCanAttack.Count; } }
	GameObject BTRParent
	{
		get
		{
			if (btrParent == null)
			{
				btrParent = new GameObject(">> AIs");
			}
			return btrParent;
		}
	}
	GameModeBase GameMode
	{
		get
		{
			if (gameMode == null)
				gameMode = Ultra.HypoUttilies.GetGameMode();
			return gameMode;
		}
	}

	private void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void Update()
	{
		if (GameMode.PlayerGameCharacter == null) return;
		if (managableAIs.Count <= 0) return;

		for (int i = 0; i < meleeAIsThatCanAttack.Count; i++)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen(meleeAIsThatCanAttack[i].btr.name + " " + i, 0f, StringColor.Black);
		}

		PrepareDistanceCalculationJobForMeleeAIs();
	}

	private void PrepareDistanceCalculationJobForMeleeAIs()
	{
		distances = new NativeArray<float>(meleeAIs.Count, Allocator.Persistent);
		sortedMeleeAIs = meleeAIs.ToArray();

		var job = new CalculateDistancesJob
		{
			targetPosition = Ultra.HypoUttilies.GetGameMode().PlayerGameCharacter.transform.position,
			otherPositions = new NativeArray<Vector3>(meleeAIs.Count, Allocator.TempJob),
			distances = distances
		};

		for (int i = 0; i < meleeAIs.Count; i++)
		{
			job.otherPositions[i] = meleeAIs[i].gameCharacter.transform.position;
		}

		jobHandle = job.Schedule(meleeAIs.Count, 32);
	}

	void LateUpdate()
	{
		if (GameMode.PlayerGameCharacter == null) return;
		if (managableAIs.Count <= 0) return;

		if (!jobHandle.IsUnityNull())
			jobHandle.Complete();
		if (distances != null && distances.IsCreated)
		{
			SortDistanceNativeArray();
			SetCanAttackFlagOnValidAIsAndRemoveOnOld();
			OrderLeftAndRightMeleeAIsInARow();

			if (distances != null && distances.IsCreated) distances.Dispose();
		}
	}

	private void OrderLeftAndRightMeleeAIsInARow()
	{
		List<HyppoliteManagableAI> leftFromPlayerSortedAIList = new List<HyppoliteManagableAI>();
		List<HyppoliteManagableAI> rightFromPlayerSortedAiList = new List<HyppoliteManagableAI>();

		if (sortedMeleeAIs != null)
		{
			for (int i = 0; i < sortedMeleeAIs.Length; i++)
			{
				if (sortedMeleeAIs[i].gameCharacter.MovementComponent == null || GameMode.PlayerGameCharacter.MovementComponent == null) continue;
				if (AIIsInStaggerState(sortedMeleeAIs[i])) continue;
				if (sortedMeleeAIs[i].gameCharacter.MovementComponent.CharacterCenter.x >= GameMode.PlayerGameCharacter.MovementComponent.CharacterCenter.x)
					rightFromPlayerSortedAiList.Add(sortedMeleeAIs[i]);
				else
					leftFromPlayerSortedAIList.Add(sortedMeleeAIs[i]);
			}

			SetMovementTarget(leftFromPlayerSortedAIList, -1);
			SetMovementTarget(rightFromPlayerSortedAiList, 1);
		}
	}

	void SetMovementTarget(List<HyppoliteManagableAI> aiList, float xModifier)
	{
		for (int i = 0; i < aiList.Count; i++)
		{
			float minDistance = aiList[i].gameCharacter.MovementComponent.Radius + (i == 0 ? GameMode.PlayerGameCharacter.MovementComponent.Radius : aiList[i - 1].gameCharacter.MovementComponent.Radius);
			minDistance += aiList[i].gameCharacter.GameCharacterData.MinCharacterDistance;
			float newXLocationTarget = (i == 0 ? GameMode.PlayerGameCharacter.MovementComponent.CharacterCenter.x : aiList[i - 1].gameCharacter.MovementComponent.CharacterCenter.x) + xModifier * minDistance;

			aiList[i]?.btr?.BehaviourTree?.Variable?.TrySetValue<Vector3>("MovementTarget", new Vector3(newXLocationTarget, 0, 0));
		}
	}

	void OnDestroy()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;

	}

	bool AIIsInStaggerState(HyppoliteManagableAI ai)
	{
		switch (ai.gameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Freez:
			case EGameCharacterState.FlyAway:
			case EGameCharacterState.MoveToPosition:
			case EGameCharacterState.HookedToCharacter:
			case EGameCharacterState.PullCharacterOnHorizontalLevel:
				return true;
			default: 
				return false;
		}
	}

	private void SetCanAttackFlagOnValidAIsAndRemoveOnOld()
	{
		if (sortedMeleeAIs == null) return;
		List<HyppoliteManagableAI> newMeleeAIs = new List<HyppoliteManagableAI>();
		///Debug Log Sorted Melee Array
		//for (int i = 0; i < sortedMeleeAIs.Length; i++)
		//{
		//	Ultra.Utilities.Instance.DebugLogOnScreen(sortedMeleeAIs[i].btr.name + " " + i, 0f, StringColor.Black);
		//}
		for (int i = 0; i < meleeCharacterAttackAmount; i++)
		{
			if (i >= sortedMeleeAIs.Length) break;
			HyppoliteManagableAI ai = sortedMeleeAIs[i];
			if (ai == null) continue;
			if (AIIsInStaggerState(ai)) continue;
			newMeleeAIs.Add(ai);
		}
		foreach (HyppoliteManagableAI ai in newMeleeAIs)
		{
			//ai.btr.BehaviourTree.Variable.TrySetValue<bool>("CanMeleeAttack", true);
			if (!meleeAIsThatCanAttack.Contains(ai)) meleeAIsThatCanAttack.Add(ai);
		}
		List<HyppoliteManagableAI> oldMeleeAIs = meleeAIsThatCanAttack.Except(newMeleeAIs).ToList();
		foreach (HyppoliteManagableAI oldAI in oldMeleeAIs)
		{
			if (oldAI.gameCharacter == null || oldAI.gameCharacter.IsGameCharacterDead)
			{
				oldAI?.btr?.BehaviourTree?.Variable?.TrySetValue<bool>("CanMeleeAttack", false);
				meleeAIsThatCanAttack.Remove(oldAI);
				continue;
			}

			if (oldAI.gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Attack && newMeleeAIs.Count > 0)
			{
				HyppoliteManagableAI ai = newMeleeAIs[newMeleeAIs.Count - 1];
				ai?.btr?.BehaviourTree?.Variable?.TrySetValue<bool>("CanMeleeAttack", false);
				meleeAIsThatCanAttack.Remove(ai);
				continue;
			}

			oldAI?.btr?.BehaviourTree?.Variable?.TrySetValue<bool>("CanMeleeAttack", false);
			meleeAIsThatCanAttack.Remove(oldAI);
		}
		foreach (HyppoliteManagableAI ai in meleeAIsThatCanAttack)
		{
			ai?.btr?.BehaviourTree?.Variable?.TrySetValue<bool>("CanMeleeAttack", true);
		}
	}

	private void SortDistanceNativeArray()
	{
		for (int i = 0; i < distances.Length - 1; i++)
		{
			for (int j = i + 1; j < distances.Length; j++)
			{
				if (distances[i] > distances[j])
				{
					// Tausche die Werte aus, um aufsteigende Reihenfolge zu erhalten
					float temp = distances[i];
					distances[i] = distances[j];
					distances[j] = temp;

					HyppoliteManagableAI tempAI = sortedMeleeAIs[i];
					sortedMeleeAIs[i] = sortedMeleeAIs[j];
					sortedMeleeAIs[j] = tempAI;
				}
			}
		}
	}

	public void AddManagableAI(HyppoliteManagableAI hyppoliteManagableAI)
	{
		if (managableAIs != null && !managableAIs.Contains(hyppoliteManagableAI))
		{
			hyppoliteManagableAI.gameCharacter.onGameCharacterDied += OnGameCharacterDied;
			hyppoliteManagableAI.gameCharacter.CombatComponent.onWeaponChanged += OnWeaponChanged;
			hyppoliteManagableAI.gameCharacter.onGameCharacterDestroyed += OnGameCharacterDestoyed;
			managableAIs.Add(hyppoliteManagableAI);
			OnWeaponChanged(hyppoliteManagableAI.gameCharacter.CombatComponent.CurrentWeapon, null, hyppoliteManagableAI.gameCharacter);
		}
	}

	void RemoveGameCharacterFromAIListsAndUnsubscribeToEvents(GameCharacter gameCharacter)
	{
		HyppoliteManagableAI ai = managableAIs.Find((HyppoliteManagableAI ai) => ai.gameCharacter == gameCharacter);
		if (ai != null)
		{
			ai.gameCharacter.onGameCharacterDied -= OnGameCharacterDied;
			ai.gameCharacter.CombatComponent.onWeaponChanged -= OnWeaponChanged;
			ai.gameCharacter.onGameCharacterDestroyed -= OnGameCharacterDestoyed;
			managableAIs.Remove(ai);
			if (meleeAIs.Contains(ai)) meleeAIs.Remove(ai);
		}
		else
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("AIManager", "RemoveGameCharacterFromAIListsAndUnsubscribeToEvents", "How can AI be null, lol!"));
	}

	void OnGameCharacterDied(GameCharacter gameCharacter)
	{
		RemoveGameCharacterFromAIListsAndUnsubscribeToEvents(gameCharacter);
	}

	void OnGameCharacterDestoyed(GameCharacter gameCharacter)
	{
		RemoveGameCharacterFromAIListsAndUnsubscribeToEvents(gameCharacter);
	}

	void OnWeaponChanged(WeaponBase newWeapon, WeaponBase oldWeapon, GameCharacter gameCharacter)
	{
		HyppoliteManagableAI ai = managableAIs.Find((HyppoliteManagableAI ai) => ai.gameCharacter == gameCharacter);
		if (oldWeapon != null)
		{
			switch (oldWeapon.WeaponData.WeaponType)
			{
				case EWeaponType.Melee:
					if (meleeAIs.Contains(ai)) meleeAIs.Remove(ai);
					break;
				default: break;
			}
		}

		if (newWeapon != null)
		{
			switch (newWeapon.WeaponData.WeaponType)
			{
				case EWeaponType.Melee:
					if (!meleeAIs.Contains(ai)) meleeAIs.Add(ai);
					break;
				default: break;
			}
		}
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (mode == LoadSceneMode.Single)
		{
			Destroy(this.gameObject);
		}
	}

	public BehaviorTreeRunner GetBehaviorTreeRunner(GameCharacter gameCharacter, ScriptableCharacter characterData, OnBehaviourTreeInit onBTRInit)
	{
		if (gameCharacter == null) return null;
		if (!IsBehaviorTreeStackInit) InitBehaviorTreeStack();
		if (NoMoreBehaviorTrees)
			SpawnBehaviorTreeRunner();

		BehaviorTreeRunner btr = behaviorTreeRunners.Pop();
		btr.BehaviorTreeAsset = characterData.behaviourTree;
		btr.onBehaviourTreeInit += onBTRInit;
		if (!btr.gameObject.activeInHierarchy)
			btr.gameObject.SetActive(true);
		else 
			btr.EnableTree();
		return btr;
	}

	public void ReturnBehaviorTreeRunner(BehaviorTreeRunner btr)
	{
		btr.DisableTree();
		btr.BehaviorTreeAsset = null;
		btr.RemoveTree();
		behaviorTreeRunners.Push(btr);
		btr.gameObject.SetActive(false);
	}

	public void InitBehaviorTreeStack()
	{
		TypeCache.CacheAllTypes();
		behaviorTreeRunners = new Stack<BehaviorTreeRunner>();
		LoadingChecker.Instance.Tasks.Add(Task.Run(async () => {
			while (!AllBehaviourTreesInit)
			{
				await Task.Yield();
			}
		}));
		for (int i = 0; i < behaviorStackMinSize; i++)
		{
			SpawnBehaviorTreeRunner(i);
		}
	}

	void SpawnBehaviorTreeRunner(int index = -1)
	{
		GameObject go = new GameObject(">> BehaviorTreeRunner_" + behaviorTreeRunners.Count);
		go.transform.parent = BTRParent.transform;
		BehaviorTreeRunner btr = go.AddComponent<BehaviorTreeRunner>();
		createdBehaviourTrees++;
		btr.RunOption = new Megumin.GameFramework.AI.RunOption();
		btr.InitOption = new Megumin.GameFramework.AI.InitOption();
		btr.InitOption.DelayRandomFrame = new(true, 15);
		if (index >= 0)
		{
			if (GameAssets.Instance.BehaviorTrees.Count > index)
			{
				btr.BehaviorTreeAsset = GameAssets.Instance.BehaviorTrees[index];
				btr.EnableTree();
			}
		}
		behaviorTreeRunners.Push(btr);
		if (btr.BehaviorTreeAsset != null)
			btr.onBehaviourTreeInit += OnBehaviourTreeInit;
		else
			OnBehaviourTreeInit(btr);
	}

	async void OnBehaviourTreeInit(BehaviorTreeRunner btr)
	{
		btr.onBehaviourTreeInit -= OnBehaviourTreeInit;
		behaviourTreeInits++;
		await new WaitForEndOfFrame();
		if (behaviorTreeRunners.Contains(btr))
			btr.gameObject.SetActive(false);
	}
}

[BurstCompile]
public struct CalculateDistancesJob : IJobParallelFor
{
	public Vector3 targetPosition;
	[DeallocateOnJobCompletionAttribute]
	[ReadOnly] public NativeArray<Vector3> otherPositions;
	public NativeArray<float> distances;

	public void Execute(int index)
	{
		distances[index] = Vector3.Distance(targetPosition, otherPositions[index]);
	}
}
