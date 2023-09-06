using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
	public delegate void OnRemoveUI();
	public OnRemoveUI onRemoveUI;
	protected bool canBeRemoved = false;

	/// <summary>
	/// Call this when the UI got Removed (After Animations end)
	/// </summary>
	public void RemoveUI()
	{
		canBeRemoved = true;
		if (onRemoveUI != null ) onRemoveUI();
	}

	public bool CanBeRemoved()
	{
		return canBeRemoved;
	}

	/// <summary>
	/// Override this in class and let it start the remove Animation
	/// </summary>
	public abstract void StartRemovingUI();

	/// <summary>
	/// IMPORTANT Call in Awake 
	/// </summary>
	protected void LoadedUI()
	{
		string name = UIManager.Instance.UIStack.Pop().name;
		UIManager.Instance.UIStack.Push(new UIStackELement(name, this));
	}
}
