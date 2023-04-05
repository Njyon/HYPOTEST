using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHitScript : MonoBehaviour
{
	public delegate void OnOverlapEnter(Collider other);
	public OnOverlapEnter onOverlapEnter;
	public delegate void OnOverlapExit(Collider other);
	public OnOverlapExit onOverlapExit;

	List<Collider> overlappingColliders = new List<Collider>();
	public List<Collider> OverlappingColliders { get { return overlappingColliders; } }


	private void OnTriggerEnter(Collider other)
	{
		overlappingColliders.Add(other);
		if (onOverlapEnter != null) onOverlapEnter(other);
	}

	private void OnTriggerExit(Collider other)
	{
		overlappingColliders.Remove(other);
		if (onOverlapExit != null) onOverlapExit(other);
	}
}
