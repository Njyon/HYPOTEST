using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterDetection : CharacterDetection<GameCharacter>
{
	protected override void OnTriggerEnterCall(GameCharacter player)
	{
		base.OnTriggerEnterCall(player);
		player.onGameCharacterDied += OnPlayerDied;
	}

	protected override void OnTriggerExitCall(GameCharacter player)
	{
		base.OnTriggerExitCall(player);
		player.onGameCharacterDied -= OnPlayerDied;
	}

	void OnPlayerDied(GameCharacter target)
	{
		if (target == null) return;
		if (onOverlapExit != null) onOverlapExit(target);
		DetectedGameCharacters.Remove(target);
		OnTriggerExitCall(target);
	}
}
