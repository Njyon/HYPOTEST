using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PluginStates can just be added to the current application and modify behaviour of states or events
/// </summary>
/// <typeparam name="T"> T is the state identifier and should always be an enum  </typeparam>
public interface IPluginState<T> 
{
	/// <summary>
	/// Get this states enum Value
	/// </summary>
	/// <returns></returns>
	public T GetStateType();

	/// <summary>
	/// Called when this state gets activated
	/// </summary>
	public void Active();

	/// <summary>
	/// Get Called every frame if state is active
	/// </summary>
	/// <param name="deltaTime"> the Deltatime of this application </param>
	public void ExecuteState(float deltaTime);

	/// <summary>
	/// FixedUpdate Funktion for PluginStates
	/// </summary>
	/// <param name="fixedTime"></param>
	public void FixedExecuteState(float fixedTime);

	/// <summary>
	/// Called when state is supposed to be deactive
	/// </summary>
	public void Deactive();

	/// <summary>
	/// Checks if this state can be active in the moment
	/// <br> Get Calles every frame </br>
	/// </summary>
	/// <returns> True if the state should be active </returns>
	public bool WantsToBeActive();

	/// <summary>
	/// Add the state to the state machine
	/// <br> IMPORTANT does not mean this state is active, just that it wants to be active </br>
	/// </summary>
	public void AddState();

	/// <summary>
	/// Remove the State from the Statemachine
	/// </summary>
	public void RemoveState();

	/// <summary>
	/// Return if the state is active or not
	/// </summary>
	/// <returns> True if active </returns>
	public bool IsActive();
}
