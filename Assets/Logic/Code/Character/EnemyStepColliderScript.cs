using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStepColliderScript : MonoBehaviour
{
	List<GameCharacter> validEnemyStepGameCharacters = new List<GameCharacter>();
	List<GameCharacter> removableCharacters = new List<GameCharacter>();
	public bool CanEnemyStep
	{
		get
		{
			foreach (var character in validEnemyStepGameCharacters)
			{
				if (character == null)
				{
					removableCharacters.Add(character);
					continue;
				}
				if (character.IsGameCharacterDead)
				{
					removableCharacters.Add(character);
					continue;
				}
				UpdateList();
				return true;
			}
			UpdateList();
			return false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other == null) return;
		if (other.tag == "EnemyStep")
		{
			Transform parent = other.transform.parent;
			bool findLastParent = false;
			while (!findLastParent)
			{
				if (parent.parent != null)
					parent = parent.parent;
				else
					findLastParent = true;
			}
			GameCharacter overlapGameCharacter = parent.gameObject.GetComponent<GameCharacter>();
			if (overlapGameCharacter != null)
			{
				validEnemyStepGameCharacters.Add(overlapGameCharacter);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other == null) return;
		if (other.tag == "EnemyStep")
		{
			Transform parent = other.transform.parent;
			bool findLastParent = false;
			while (!findLastParent)
			{
				if (parent.parent != null)
					parent = parent.parent;
				else
					findLastParent = true;
			}
			GameCharacter overlapGameCharacter = parent.gameObject.GetComponent<GameCharacter>();
			if (overlapGameCharacter != null && validEnemyStepGameCharacters.Contains(overlapGameCharacter)) 
			{
				validEnemyStepGameCharacters.Remove(overlapGameCharacter);
			}
		}
	}

	void UpdateList()
	{
		foreach(var character in removableCharacters)
		{
			validEnemyStepGameCharacters.Remove(character);
		}
	}
}
