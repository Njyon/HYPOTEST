using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
	public delegate void OnRemoveUI();
	public OnRemoveUI onRemoveUI;
	protected bool canBeRemoved = false;

	public void RemoveUI()
	{
		canBeRemoved = true;
		if (onRemoveUI != null ) onRemoveUI();
	}

	public bool CanBeRemoved()
	{
		return canBeRemoved;
	}

	public abstract void StartRemovingUI();
	protected void LoadedUI()
	{
		string name = UIManager.Instance.UIStack.Pop().name;
		UIManager.Instance.UIStack.Push(new UIs(name, this));
	}
}
