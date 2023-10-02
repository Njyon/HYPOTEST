using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.MeshOperations;

[Serializable]
public class CharacterSpawnData
{
	public ScriptableCharacter character;
	public int spawnAmount;
	public int maxOnField = -1;

	int currentlySpawned;
	public int CurrentlySpawned { get { return currentlySpawned; } set {  currentlySpawned = value; } }
	int currentlyAlive;
	public int CurrentlyAlive { get {  return currentlyAlive; } set { currentlyAlive = value; } }
}

[Serializable]
public class SpawnData
{
	public List<CharacterSpawnData> charactersToSpawn;
}

public class ArenaSpawner : MonoBehaviour
{
	public delegate void ArenaSpawnerEvent();
	public ArenaSpawnerEvent onFinishedSpawning;
	public ArenaSpawnerEvent onLastEnemyKilled;

	[SerializeField] List<SpawnData> spawnDataBasedOnDifficulty;
	[SerializeField] List<GameObject> randomSpawnLocations;
	[SerializeField] SerializableCharacterDictionary<string, GameObject> characterSpecificSpawnPoints;
	[SerializeField] int maxSpawnEnemies = 5;
	public UnityEvent onStartSpawningEvent;
	public UnityEvent onLastEnemyKilledEvent;
	List<GameCharacter> spawnedGameCharacters;

	int spawnIndex = 0;
	bool startedSpawning = false;
	GameModeBase gameMode;
	public GameModeBase GameMode { 
		get { 
			if (gameMode == null)
			{
				gameMode = Ultra.HypoUttilies.GetGameMode();
			}
			return gameMode; 
		} 
	}
	SpawnData dataToSpawn;
	bool finishedSpawning = false;

	private void Start()
	{
		GameDifficultyLevel difficultyLevel = gameMode.GameDifficultyLevel;
		if (spawnDataBasedOnDifficulty.Count <= (int)difficultyLevel)
		{
			difficultyLevel = (GameDifficultyLevel)spawnDataBasedOnDifficulty.Count;
		}
		dataToSpawn = spawnDataBasedOnDifficulty[(int)difficultyLevel];
	}

	public void StartSpawningCharacters()
	{
		if (!startedSpawning)
		{
			startedSpawning = true;
			SpawnCharacters();
			onStartSpawningEvent.Invoke();
		}
	}

	void SpawnCharacters()
	{	
		for (int i = spawnedGameCharacters.Count - 1; i < maxSpawnEnemies; i++) 
		{
			// Horrible solution but my brain is to fried to find a bettert way right now
			// Try to not to restart the spawning index when here. (Foreach restarts the index everytime it hits here)
			bool started = true;
			int startIndex = spawnIndex;
			for (int j = startIndex; j < dataToSpawn.charactersToSpawn.Count; j++)
			{
				if (j == startIndex && !started)
					break;
				if (started) started = false;

				var data = dataToSpawn.charactersToSpawn[spawnIndex];

				if (spawnedGameCharacters.Count > maxSpawnEnemies) break;
				if (data.CurrentlySpawned >= data.spawnAmount) { spawnIndex++; continue; } 
				if (data.CurrentlyAlive >= data.maxOnField) { spawnIndex++; continue; }

				Vector3 spawnLocation = Vector3.zero;
				Quaternion spawnRotation = Quaternion.identity;
				DeterminSpawnLocation(data, out spawnLocation, out spawnRotation);
				SpawnGameCharacterAndController(data, spawnLocation, spawnRotation);

				spawnIndex++;
				if (spawnIndex >= dataToSpawn.charactersToSpawn.Count)
				{
					spawnIndex = 0;
					j = 0;	
				}
			}
			//foreach (CharacterSpawnData data in dataToSpawn.charactersToSpawn)
			//{
			//	if (spawnedGameCharacters.Count > maxSpawnEnemies) continue;
			//	if (data.CurrentlySpawned >= data.spawnAmount) continue;
			//	if (data.CurrentlyAlive >= data.maxOnField) continue;
			//
			//	Vector3 spawnLocation = Vector3.zero;
			//	Quaternion spawnRotation = Quaternion.identity;
			//	DeterminSpawnLocation(data, out spawnLocation, out spawnRotation);
			//	SpawnGameCharacterAndController(data, spawnLocation, spawnRotation);
			//}
		}
		bool finished = false;
		foreach (CharacterSpawnData data in dataToSpawn.charactersToSpawn)
		{
			if (data.CurrentlySpawned != data.spawnAmount) {
				finished = false; break;
			}
			else 
				finished = true;
		}
		if (finished)
			FinishedSpawning();
	}

	void FinishedSpawning()
	{
		finishedSpawning = true;
		if (onFinishedSpawning != null) onFinishedSpawning();
	}

	private void DeterminSpawnLocation(CharacterSpawnData data, out Vector3 spawnLocation, out Quaternion spawnRotation)
	{
		if (characterSpecificSpawnPoints.ContainsKey(data.character.name))
		{
			var SpecificSpawnPointData = characterSpecificSpawnPoints[data.character.name];
			spawnLocation = SpecificSpawnPointData.transform.position;
			spawnRotation = SpecificSpawnPointData.transform.rotation;
		}
		else if (randomSpawnLocations.Count > 0)
		{
			GameObject randomSpawnLocation = randomSpawnLocations[UnityEngine.Random.Range(0, randomSpawnLocations.Count)];
			spawnLocation = randomSpawnLocation.transform.position;
			spawnRotation = randomSpawnLocation.transform.rotation;
		}
		else
		{
			spawnLocation = transform.position;
			spawnRotation = transform.rotation;
		}
	}

	private void SpawnGameCharacterAndController(CharacterSpawnData data, Vector3 spawnLocation, Quaternion spawnRotation)
	{
		GameObject spawnedCharacter = GameObject.Instantiate(data.character.CharacterPrefab, spawnLocation, spawnRotation);
		ControllerBase controller = Ultra.HypoUttilies.Instance.SpawnController(spawnedCharacter, data.character);
		data.CurrentlySpawned++;
		data.CurrentlyAlive++;
		GameCharacter spawnedGameCharacter = spawnedCharacter.GetComponent<GameCharacter>();
		spawnedGameCharacter.onGameCharacterDied += OnGameCharacterDied;
		spawnedGameCharacters.Add(spawnedGameCharacter);
	}

	void OnGameCharacterDied(GameCharacter gameCharacter)
	{
		if (gameCharacter == null) return;
		spawnedGameCharacters.Remove(gameCharacter);
		var index = dataToSpawn.charactersToSpawn.FindIndex((e) => { return e.character.Name == gameCharacter.CharacterData.Name; });
		if (index >= 0) 
			dataToSpawn.charactersToSpawn[index].CurrentlyAlive--;
		SpawnCharacters();
		if (finishedSpawning && spawnedGameCharacters.Count <= 0)
			LastEnemyDied();
	}

	void LastEnemyDied()
	{
		if (onLastEnemyKilled != null) onLastEnemyKilled();
		onLastEnemyKilledEvent.Invoke();
	}
}
