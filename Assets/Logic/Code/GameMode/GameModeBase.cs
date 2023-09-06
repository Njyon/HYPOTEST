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

public class GameModeBase : Singelton<GameModeBase> 
{

	PlayerGameCharacter playerCharacter;
	GameDifficultyLevel gameDifficultyLevel = GameDifficultyLevel.Normal;

	public PlayerGameCharacter PlayerGameCharacter 
	{ 
		get {
			if (playerCharacter == null) playerCharacter = FindObjectOfType<PlayerGameCharacter>();
			return playerCharacter; 
		} 
	}

	public GameDifficultyLevel GameDifficultyLevel { get { return gameDifficultyLevel; } set { gameDifficultyLevel = value; } }
	public virtual void Create()
	{

	}
}
