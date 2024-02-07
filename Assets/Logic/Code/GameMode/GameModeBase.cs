using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum GameDifficultyLevel
{
	Easy,
	Normal,
	Hard,
	HyppoliteMustDie
}

public abstract class GameModeBase : MonoBehaviour
{

	PlayerGameCharacter playerCharacter;
	GameDifficultyLevel gameDifficultyLevel = GameDifficultyLevel.Normal;

	public PlayerGameCharacter PlayerGameCharacter 
	{ 
		get {
			if (playerCharacter == null) playerCharacter = Ultra.HypoUttilies.GetPlayerGameCharacter();
			return playerCharacter; 
		} 
	}

	public GameDifficultyLevel GameDifficultyLevel { get { return gameDifficultyLevel; } set { gameDifficultyLevel = value; } }
	public virtual void Create()
	{

	}

	public DefaultGameModeData GetDefaultGameModeData()
	{
		return GameAssets.Instance.gameModeData;
	}

	public abstract bool AllowDamage();
}
