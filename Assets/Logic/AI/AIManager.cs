using Megumin.Binding;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

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
	int meleeCharacterAttackAmount = 1;

	public int MeleeCharacterAttackAmount { get { return meleeCharacterAttackAmount; } }
	public int ManagableAIsCount { get { return managableAIs.Count; } }
	public int MeleeAIsCount { get { return meleeAIs.Count; } }
	public int MeleeAIsThatCanAttackCount { get { return meleeAIsThatCanAttack.Count; } }

	private void Awake()
	{

	}

	void Update()
	{
		if (Ultra.HypoUttilies.GetGameMode().PlayerGameCharacter == null) return;
		if (managableAIs.Count <= 0) return;

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
		if (jobHandle != null)
			jobHandle.Complete();
		if (distances != null)
		{
			SortDistanceNativeArray();
			SetCanAttackFlagOnValidAIsAndRemoveOnOld();
			distances.Dispose();
		}
	}

	private void SetCanAttackFlagOnValidAIsAndRemoveOnOld()
	{
		List<HyppoliteManagableAI> newMeleeAIs = new List<HyppoliteManagableAI>();
		for (int i = 0; i < meleeCharacterAttackAmount; i++)
		{
			if (i >= meleeAIs.Count) break;
			newMeleeAIs.Add(sortedMeleeAIs[i]);
		}
		foreach (HyppoliteManagableAI ai in newMeleeAIs)
		{
			ai.btr.BehaviourTree.Variable.TrySetValue<bool>("CanMeleeAttack", true);
			if (!meleeAIsThatCanAttack.Contains(ai)) meleeAIsThatCanAttack.Add(ai);
		}
		List<HyppoliteManagableAI> oldMeleeAIs = meleeAIsThatCanAttack.Except(newMeleeAIs).ToList();
		foreach (HyppoliteManagableAI oldAI in oldMeleeAIs)
		{
			if (oldAI.gameCharacter != null || oldAI.gameCharacter.IsGameCharacterDead) continue;
			oldAI.btr.BehaviourTree.Variable.TrySetValue<bool>("CanMeleeAttack", false);
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
		if (managableAIs != null && !managableAIs.Contains(hyppoliteManagableAI)) {
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
			Debug.Log("How can AI be null, lol!");
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


	[BurstCompile]
	private struct CalculateDistancesJob : IJobParallelFor
	{
		public Vector3 targetPosition;
		[ReadOnly] public NativeArray<Vector3> otherPositions;
		public NativeArray<float> distances;

		public void Execute(int index)
		{
			distances[index] = Vector3.Distance(targetPosition, otherPositions[index]);
		}
	}
}
