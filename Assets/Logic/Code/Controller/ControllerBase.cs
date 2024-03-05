using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HyppoliteTeam
{
	None,
	TeamPlayer,
	TeamEnemy,
}

public abstract class ControllerBase : MonoBehaviour
{
	protected GameObject pawn;
	protected ScriptableCharacter characterData;

	public virtual void BeginPosses(GameObject pawn, ScriptableCharacter characterData)
	{
		this.pawn = pawn;
		this.characterData = characterData;
	}
	public virtual void EndPosses()
	{
		pawn = null;
		characterData = null;
	}
	protected abstract void OnGameCharacterDied(GameCharacter gameCharacter);
	protected abstract void OnGameCharacterDestroyed(GameCharacter gameCharacter);
}
