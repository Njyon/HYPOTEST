using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDetection : MonoBehaviour
{
	public delegate void OnOverlapEnter(GameCharacter other);
	public OnOverlapEnter onOverlapEnter;
	public delegate void OnOverlapExit(GameCharacter other);
	public OnOverlapExit onOverlapExit;

	[SerializeField] SphereCollider collider;
	public SphereCollider Collider { get { return collider; } }

	public List<GameCharacter> OverlappingGameCharacter = new List<GameCharacter>();

	public void Awake()
	{
		if (collider == null) collider.GetComponent<SphereCollider>();
		if (collider == null)
		{
			Debug.LogError(Ultra.Utilities.Instance.DebugErrorString("CharacterDetection", "Awake", "Collider on CharacterDectection was null!"));
			return;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform == this.transform.parent) return;

		GameCharacter gameCharacter = other.gameObject.GetComponent<GameCharacter>();
		if (gameCharacter != null && !OverlappingGameCharacter.Contains(gameCharacter))
		{
			OverlappingGameCharacter.Add(gameCharacter);
			if (onOverlapEnter != null) onOverlapEnter(gameCharacter);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform == this.transform.parent)
		{
			return;
		}

		GameCharacter gameCharacter = other.gameObject.GetComponent<GameCharacter>();
		if (gameCharacter != null)
		{
			OverlappingGameCharacter.Remove(gameCharacter);
			if (onOverlapExit != null) onOverlapExit(gameCharacter);
		}
	}
}
