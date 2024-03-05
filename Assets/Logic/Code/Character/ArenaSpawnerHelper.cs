using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawnerHelper : MonoBehaviour
{
	[SerializeField] GameObject newPlayerSpawn;

	public void SetNewPlayerCharacterSpawn()
	{
		PlayerGameCharacter gameCharacter = Ultra.HypoUttilies.GetPlayerGameCharacter();
		gameCharacter.SetNewRespawnPoint(newPlayerSpawn);
	}
}
