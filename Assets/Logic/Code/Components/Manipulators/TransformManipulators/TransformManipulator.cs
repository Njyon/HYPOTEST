using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformManipulator : MonoBehaviour
{
	public List<ClassInstance<ATransformManipulator>> transformModifiers = new List<ClassInstance<ATransformManipulator>>();

	public void Awake()
	{
		foreach (var mod in transformModifiers)
		{
			if (mod.instance.startOnAwake && !mod.instance.DidStart)
				mod.instance.DoOperation();
		}
	}

	public void Update()
	{
		foreach(var mod in transformModifiers)
		{
			if (mod.instance.DidStart && mod.instance.updateBehaviour)
				mod.instance.UpdateOperation();
		}
	}

	public void StartModification()
	{
		foreach (var mod in transformModifiers)
		{
			mod.instance.DoOperation();
		}
	}
}
