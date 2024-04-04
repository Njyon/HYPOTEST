using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterDetection : TargetDetection<GameCharacter>
{
	protected override void OnTriggerEnterCall(GameCharacter gameCharacter)
	{
		gameCharacter.onGameCharacterDied += OnPlayerDiedDestroyed;
		gameCharacter.onGameCharacterDestroyed += OnPlayerDiedDestroyed;
	}

	protected override void OnTriggerExitCall(GameCharacter gameCharacter)
	{
		gameCharacter.onGameCharacterDied -= OnPlayerDiedDestroyed;
		gameCharacter.onGameCharacterDestroyed -= OnPlayerDiedDestroyed;
	}

	void OnPlayerDiedDestroyed(GameCharacter target)
	{
		if (target == null) return;
		OnTriggerExitCall(target);
	}
}
