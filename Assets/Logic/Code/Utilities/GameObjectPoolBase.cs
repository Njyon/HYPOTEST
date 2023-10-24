using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameObjectPoolBase : PoolBase<GameObject>
{
	GameObject parent;
	public GameObject Parent { get { return parent; } }

	public GameObjectPoolBase(GameObject parent) : base()
	{
		this.parent = parent;	
	}

	public GameObjectPoolBase(GameObject parent, int minSize) : base(minSize) 
	{
		this.parent = parent;
	}
}

public abstract class MonoBehaviourPoolBase<T> : PoolBase<T> where T : MonoBehaviour
{
	GameObject parent;
	public GameObject Parent { get { return parent; } }

	public MonoBehaviourPoolBase(GameObject parent) : base()
	{
		this.parent = parent;
	}

	public MonoBehaviourPoolBase(GameObject parent, int minSize) : base(minSize)
	{
		this.parent = parent;
	}

	public override T GetValue()
	{
		if (!IsTStackInit) InitStack();

		if (NoMoreTInStack)
			SpawnValue();

		T go = stack.Pop();
		ActivateValue(go);
		return go;
	}

	protected override void ActivateValue(T value)
	{
		value.enabled = true;
	}

	protected override void DeactivateValue(T value)
	{
		value.enabled = false;
	}

	protected override void SpawnValue()
	{
		GameObject go = GameObject.Instantiate(new GameObject(parent.name + " PoolObject"), parent.transform);
		T value = go.AddComponent<T>();
		stack.Push(value);
		DeactivateValue(value);
	}
}

public abstract class ComponentPoolBase<T> : PoolBase<T> where T : Component
{
	GameObject parent;
	public GameObject Parent { get { return parent; } }

	public ComponentPoolBase(GameObject parent) : base()
	{
		this.parent = parent;
	}

	public ComponentPoolBase(GameObject parent, int minSize) : base(minSize)
	{
		this.parent = parent;
	}

	public override T GetValue()
	{
		if (!IsTStackInit) InitStack();

		if (NoMoreTInStack)
			SpawnValue();

		T go = stack.Pop();
		ActivateValue(go);
		return go;
	}

	protected override void ActivateValue(T value)
	{
		value.gameObject.SetActive(true);
	}

	protected override void DeactivateValue(T value)
	{
		value.gameObject.SetActive(false);
	}

	protected override void SpawnValue()
	{
		GameObject go = GameObject.Instantiate(new GameObject(parent.name + " PoolObject"), parent.transform);
		T value = go.AddComponent<T>();
		stack.Push(value);
		DeactivateValue(value);
	}
}
