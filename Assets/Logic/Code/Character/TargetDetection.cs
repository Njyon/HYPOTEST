using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetection<T> : MonoBehaviour
{
	public delegate void OnOverlapEnter(T other);
	public OnOverlapEnter onOverlapEnter;
	public delegate void OnOverlapExit(T other);
	public OnOverlapExit onOverlapExit;

	public UltEvents.UltEvent<T> onOverlapEnterEvent;
	public UltEvents.UltEvent<T> onOverlapExitEvent;

	[SerializeField] Collider collider;
	public Collider Collider { 
		get 
		{ 
			if (collider == null)
			{
				collider = GetComponent<Collider>();
				if (collider == null)
				{
					Ultra.Utilities.Instance.DebugErrorString("TargetDetection", "Collider", "Collider was not attached to a TargetDetection Object!");
				}
			}
			return collider; 
		} 
	}

	public List<T> DetectedTargets = new List<T>();

	[Header("Gizmo")]
	[SerializeField] bool drawGizmo = true;
	[ConditionalField("drawGizmo")] [SerializeField] Color gizmoColor;

	public void Awake()
	{
		if (collider == null) collider.GetComponent<Collider>();
		if (collider == null)
		{
			Debug.LogError(Ultra.Utilities.Instance.DebugErrorString("TargetDetection", "Awake", "Collider on CharacterDectection was null!"));
			return;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform == this.transform.parent) return;

		T template = other.gameObject.GetComponent<T>();
		if (template != null && !DetectedTargets.Contains(template))
		{
			OnTriggerEnterCall(template);
			if (onOverlapEnter != null) onOverlapEnter(template);
			if (onOverlapEnterEvent != null) onOverlapEnterEvent.Invoke(template);
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
			OnTriggerExitCall(template);
			if (onOverlapExit != null) onOverlapExit(template);
			if (onOverlapExitEvent != null) onOverlapExitEvent.Invoke(template);
		}
	}

	private void OnDrawGizmos()
	{
		if (!drawGizmo) return;

		Gizmos.color = gizmoColor;
		Gizmos.DrawCube(transform.position, collider.bounds.size);
		
	}

	protected virtual void OnTriggerEnterCall(T collider)
	{
		DetectedTargets.Add(collider);
	}

	protected virtual void OnTriggerExitCall(T collider)
	{
		DetectedTargets.Remove(collider);
	}


}
