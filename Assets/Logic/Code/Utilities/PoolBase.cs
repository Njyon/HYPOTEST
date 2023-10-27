using Megumin.GameFramework.AI.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolBase<T>
{
	public PoolBase()
	{
		minStackSize = 5;
	}

	public PoolBase(int minStackSize)
	{
		this.minStackSize = minStackSize;
	}

	protected Stack<T> stack;
	protected bool NoMoreTInStack => stack.Count <= 0;
	protected bool IsTStackInit => stack != null;
	int minStackSize;

	public abstract T GetValue();
	protected abstract void SpawnValue();
	protected abstract void ActivateValue(T value);
	protected abstract void DeactivateValue(T value);

	public void ReturnValue(T value)
	{
		if (value == null) return;
		DeactivateValue(value);
		if (IsTStackInit)
			stack.Push(value);
	}

	protected void InitStack()
	{
		stack = new Stack<T>();
		for (int i = 0; i < minStackSize; i++)
		{
			SpawnValue();
		}
	}
}
