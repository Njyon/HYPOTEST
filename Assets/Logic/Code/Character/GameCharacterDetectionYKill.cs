using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterDetectionYKill : GameCharacterDetection
{
	protected override void OnTriggerEnterCall(GameCharacter gameCharacter)
	{
		// Kill Character
		gameCharacter.Health.AddCurrentValue(-gameCharacter.Health.CurrentValue);
	}

	protected override void OnTriggerExitCall(GameCharacter gameCharacter)
	{
		
	}
}
