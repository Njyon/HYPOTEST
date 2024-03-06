using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterDetection : CharacterDetection<GameCharacter>
{
	protected override void OnTriggerEnterCall(GameCharacter gameCharacter)
	{
		base.OnTriggerEnterCall(gameCharacter);
	}

	protected override void OnTriggerExitCall(GameCharacter gameCharacter)
	{
		base.OnTriggerExitCall(gameCharacter);
	}
}
