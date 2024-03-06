using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterDetection : CharacterDetection<GameCharacter>
{
	protected override void OnTriggerEnterCall(GameCharacter gameCharacter)
	{
		base.OnTriggerEnterCall(gameCharacter);
		gameCharacter.onGameCharacterDied += OnPlayerDied;
	}

	protected override void OnTriggerExitCall(GameCharacter gameCharacter)
	{
		base.OnTriggerExitCall(gameCharacter);
		gameCharacter.onGameCharacterDied -= OnPlayerDied;
	}

	void OnPlayerDied(GameCharacter target)
	{
		if (target == null) return;
		if (onOverlapExit != null) onOverlapExit(target);
		DetectedGameCharacters.Remove(target);
		OnTriggerExitCall(target);
	}
}
