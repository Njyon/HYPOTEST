using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ATransformManipulator
{
	[SerializeField] protected List<Transform> transforms = new List<Transform>();
	public bool startOnAwake = false;
	public bool updateBehaviour = true;

	bool didStart = false;
	public bool DidStart { get { return didStart; } }

	public virtual void DoOperation()
	{
		didStart = true;
	}
	public virtual void UpdateOperation() { }
}
