using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PluginStateMachineBase<T> : MonoBehaviour
{
	Dictionary<T, IPluginState<T>> dictionaryOfPluginStates;
	public Dictionary<T, IPluginState<T>> DictionaryOfPluginStates { get { return dictionaryOfPluginStates; } }

	public OnPluginStateAdded onPluginStateAdded;
	public OnPluginStateRemoved onPluginStateRemoved;
	public OnPluginStateActivated onPluginStateActivated;
	public OnPluginStateDeactivated onPluginStateDeactivated;
	public delegate void OnPluginStateAdded(T addedStateType);
	public delegate void OnPluginStateRemoved(T removedStateType);
	public delegate void OnPluginStateActivated(T activatedStateType);
	public delegate void OnPluginStateDeactivated(T deactivatedStateType);

	private void Awake()
	{
		dictionaryOfPluginStates = new Dictionary<T, IPluginState<T>>();
	}

	private void Update()
	{
		// In this foreach will every state, containt by "DictionaryOfPluginStates" be updated
		foreach (var item in DictionaryOfPluginStates)
		{
			// First we check if the state wants to be active with the current states and event of the application
			bool wantsToBeActive = item.Value.WantsToBeActive();
			// If the state doesnt want to be deactive & is active right now, we deactivate it and call the onDeactivated event
			if (item.Value.IsActive() && !wantsToBeActive)
			{
				item.Value.Deactive();
				if (onPluginStateDeactivated != null) onPluginStateDeactivated(item.Value.GetStateType());
			}
			// If the state does want to be active & is deactive, we activate it and call the onActivation event
			else if (!item.Value.IsActive() && wantsToBeActive)
			{
				item.Value.Active();
				if (onPluginStateActivated != null) onPluginStateActivated(item.Value.GetStateType());
			}

			// After that we Execute all active PluginStates
			if (item.Value.IsActive())
				item.Value.ExecuteState(Time.deltaTime);
		}
	}

	/// <summary>
	/// Add PluginState to Statemachine if state is not yet added
	/// </summary>
	/// <param name="stateType"> Type of PluginState </param>
	/// <returns> True if the State got successfully added </returns>
	public bool AddPluginState(T stateType)
	{
		if (DictionaryOfPluginStates == null || DictionaryOfPluginStates.ContainsKey(stateType))
			return false;

		IPluginState<T> newState;
		bool wasCreated = CreatePluginState(stateType, out newState);
		if (wasCreated)
		{
			DictionaryOfPluginStates.Add(stateType, newState);
			newState?.AddState();
			if (onPluginStateAdded != null) onPluginStateAdded(stateType);
		}
		else 
			return false;
		return true;
	}

	/// <summary>
	/// Try to remove Plugin State of type T
	/// </summary>
	/// <param name="stateType"> Type of Plugin State </param>
	/// <returns> Return True if Plugin State got successfully removed </returns>
	public bool RemovePluginState(T stateType)
	{
		if (!DictionaryOfPluginStates.ContainsKey(stateType))
			return false;

		IPluginState<T> removeState;
		DictionaryOfPluginStates.TryGetValue(stateType, out removeState);
		removeState?.Deactive();
		removeState?.RemoveState();
		bool removed = DictionaryOfPluginStates.Remove(stateType);
		if (removed && onPluginStateRemoved != null) onPluginStateRemoved(stateType);
		return removed;
	}

	/// <summary>
	/// Create the Plugin State
	/// </summary>
	/// <param name="stateType"> Type of the Plugin State </param>
	/// <param name="newPluginState"></param>
	/// <returns></returns>
	protected abstract bool CreatePluginState(T stateType, out IPluginState<T> newPluginState);

	/// <summary>
	/// Check if the State is pluged in and if its active or not
	/// </summary>
	/// <param name="stateType"> The state type of the state that should be checkt </param>
	/// <param name="CheckOnlyIfActive"> if False, return true even when state is not active but pluged in </param>
	/// <returns> Return if the state is pluged in and active if "CheckOnlyIfActive" is true </returns>
	public bool IsPluginStatePlugedIn(T stateType, bool CheckOnlyIfActive = true)
	{
		bool stateIsPluged = DictionaryOfPluginStates.ContainsKey(stateType);
		if (!stateIsPluged) return false;
		IPluginState<T> pluginState;
		bool foundState = DictionaryOfPluginStates.TryGetValue(stateType, out pluginState);
		if (!foundState) return false;
		bool isActive = pluginState.IsActive();

		if (CheckOnlyIfActive)
			return isActive;
		else
			return true;
	}

	/// <summary>
	/// A fast check if the State exists in the Dictionary
	/// </summary>
	/// <param name="stateType"> The state type of the state that should be checkt </param>
	/// <returns></returns>
	public bool ContainsPluginState(T stateType)
	{
		return DictionaryOfPluginStates.ContainsKey(stateType);
	}
}
