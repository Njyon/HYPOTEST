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
	public int maxSpawnEnemies = 5;
	public List<CharacterSpawnData> charactersToSpawn;
}

public class ArenaSpawner : MonoBehaviour
{
	public delegate void ArenaSpawnerEvent();
	public ArenaSpawnerEvent onFinishedSpawning;
	public ArenaSpawnerEvent onLastEnemyKilled;
	public ArenaSpawnerEvent onPlayerDiedAndRespawned;
	
	[SerializeField] List<SpawnData> spawnDataBasedOnDifficulty;
	[SerializeField] List<GameObject> randomSpawnLocations;
	[SerializeField] SerializableCharacterDictionary<string, GameObject> characterSpecificSpawnPoints;
	public UnityEvent onStartSpawningEvent;
	public UnityEvent onLastEnemyKilledEvent;
	public UnityEvent onPlayerDiedAndRespawnedEvent;
	List<GameCharacter> spawnedGameCharacters = new List<GameCharacter>();

	int spawnIndex = 0;
	int lastSpawnPointIndex = -1;
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

	int SpawnIndex { get { return spawnIndex; }  
		set {
			spawnIndex = value;
			if (dataToSpawn != null && spawnIndex >= dataToSpawn.charactersToSpawn.Count)
			{
				spawnIndex = 0;
			}
		}
	}
	SpawnData dataToSpawn;
	bool finishedSpawning = false;

	private void Start()
	{
		GameDifficultyLevel difficultyLevel = GameMode.GameDifficultyLevel;
		if (spawnDataBasedOnDifficulty.Count <= (int)difficultyLevel)
		{
			difficultyLevel = (GameDifficultyLevel)spawnDataBasedOnDifficulty.Count - 1;
		}
		if (spawnDataBasedOnDifficulty.Count > 0)
			dataToSpawn = spawnDataBasedOnDifficulty[(int)difficultyLevel];
	}

	public void StartSpawningCharacters()
	{
		if (!enabled) return;

		if (!startedSpawning)
		{ 
			startedSpawning = true;
			SpawnCharacters();
			onStartSpawningEvent.Invoke();

			PlayerGameCharacter playerGC = Ultra.HypoUttilies.GetPlayerGameCharacter();
			playerGC.onGameCharacterRespawnes += OnPlayerRespawnes;
		}
	}

	void SpawnCharacters()
	{	
		for (int i = spawnedGameCharacters.Count > 0 ? spawnedGameCharacters.Count - 1 : 0; i < dataToSpawn.maxSpawnEnemies; i++) 
		{
			// Horrible solution but my brain is to fried to find a bettert way right now
			// Try to not to restart the spawning index when here. (Foreach restarts the index everytime it hits here)
			bool started = true;
			int startIndex = SpawnIndex;
			for (int j = startIndex; j < dataToSpawn.charactersToSpawn.Count; j++)
			{
				if (j == startIndex && !started)
					break;
				if (started) started = false;

				var data = dataToSpawn.charactersToSpawn[j];

				if (spawnedGameCharacters.Count > dataToSpawn.maxSpawnEnemies) break;
				if (data.CurrentlySpawned >= data.spawnAmount) { SpawnIndex++; continue; } 
				if (data.CurrentlyAlive >= data.maxOnField) { SpawnIndex++; continue; }

				Vector3 spawnLocation = Vector3.zero;
				Quaternion spawnRotation = Quaternion.identity;
				DeterminSpawnLocation(data, out spawnLocation, out spawnRotation);
				SpawnGameCharacterAndController(data, spawnLocation, spawnRotation);

				if (SpawnIndex + 1 >= dataToSpawn.charactersToSpawn.Count)
				{
					j = 0;	
				}
				SpawnIndex++;
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
			int index = UnityEngine.Random.Range(0, randomSpawnLocations.Count);
			if (index == lastSpawnPointIndex)
			{
				index++;
				index = index % randomSpawnLocations.Count;
			}
			GameObject randomSpawnLocation = randomSpawnLocations[index];
			spawnLocation = randomSpawnLocation.transform.position;
			spawnRotation = randomSpawnLocation.transform.rotation;

			lastSpawnPointIndex = index;
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
		PlayerGameCharacter playerGC = Ultra.HypoUttilies.GetPlayerGameCharacter();
		playerGC.onGameCharacterRespawnes -= OnPlayerRespawnes;

		if (onLastEnemyKilled != null) onLastEnemyKilled();
		onLastEnemyKilledEvent.Invoke();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, "SpanwerImage", true);
	}

	void OnPlayerRespawnes(GameCharacter gameCharacter)
	{
		startedSpawning = false;

		foreach (GameCharacter gc in spawnedGameCharacters)
		{
			Destroy(gc);
		}
		spawnedGameCharacters.Clear();

		if (onPlayerDiedAndRespawned != null) onPlayerDiedAndRespawned();
		onPlayerDiedAndRespawnedEvent.Invoke();
	}
}
