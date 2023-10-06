using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDetection<T> : MonoBehaviour where T : GameCharacter
{
	public delegate void OnOverlapEnter(T other);
	public OnOverlapEnter onOverlapEnter;
	public delegate void OnOverlapExit(T other);
	public OnOverlapExit onOverlapExit;

	[SerializeField] Collider collider;
	public Collider Collider { get { return collider; } }

	public List<T> OverlappingGameCharacter = new List<T>();

	public void Awake()
	{
		if (collider == null) collider.GetComponent<Collider>();
		if (collider == null)
		{
			Debug.LogError(Ultra.Utilities.Instance.DebugErrorString("CharacterDetection", "Awake", "Collider on CharacterDectection was null!"));
			return;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform == this.transform.parent) return;

		T template = other.gameObject.GetComponent<T>();
		if (template != null && !OverlappingGameCharacter.Contains(template))
		{
			OverlappingGameCharacter.Add(template);
			OnTriggerEnterCall(template);
			if (onOverlapEnter != null) onOverlapEnter(template);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform == this.transform.parent)
		{
			return;
		}

		T template = other.gameObject.GetComponent<T>();
		if (template != null)
		{
			OverlappingGameCharacter.Remove(template);
			OnTriggerExitCall(template);
			if (onOverlapExit != null) onOverlapExit(template);
		}
	}

	protected virtual void OnTriggerEnterCall(T collider)
	{

	}

	protected virtual void OnTriggerExitCall(T collider)
	{

	}
}
