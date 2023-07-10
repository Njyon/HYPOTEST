using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerBase : ControllerBase
{
	GameCharacter gameCharacter;

	public override void BeginPosses(GameObject pawn, ScriptableCharacter characterData)
	{
		base.BeginPosses(pawn, characterData);

		SetupGameCharacter(pawn);
	}

	private void SetupGameCharacter(GameObject pawn)
	{
		gameCharacter = pawn.AddComponent<GameCharacter>();
		gameCharacter.CharacterData = characterData;
		gameCharacter.CustomAwake();
		GameCharacterMovementComponent movementComponent = pawn.GetComponent<GameCharacterMovementComponent>();
		if (movementComponent != null) movementComponent.SetupGameCharacter(gameCharacter);
	}
}
