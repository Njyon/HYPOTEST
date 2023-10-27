using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : GameObjectPoolBase
{
	GameObject instance;

	public GameObjectPool(GameObject instance, GameObject parent) : base(instance)
	{
		this.instance = instance;
	}

	public GameObjectPool(GameObject instance, GameObject parent, int stackMinsize) : base(parent, stackMinsize)
	{
		this.instance = instance;
	}

	public override GameObject GetValue()
	{
		if (!IsTStackInit) InitStack();

		if (NoMoreTInStack)
			SpawnValue();

		GameObject go = stack.Pop();
		ActivateValue(go);
		return go;
	}

	protected override void ActivateValue(GameObject value)
	{
		value.SetActive(true);
	}

	protected override void DeactivateValue(GameObject value)
	{
		value.SetActive(false);
	}

	protected override void SpawnValue()
	{
		GameObject go = GameObject.Instantiate(instance, Parent.transform);
		stack.Push(go);
		DeactivateValue(go);
	}
}
