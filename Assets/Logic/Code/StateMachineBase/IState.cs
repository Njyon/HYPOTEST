using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Interface, should be used with StateMachineBase and all States for that state machine
/// </summary>
/// <typeparam name="T"> T is the state identifier and should always be an enum </typeparam>
public interface IState<T>
{
	/// <summary>
	/// Get this states enum value
	/// </summary>
	/// <returns></returns>
	public T GetStateType();

	/// <summary>
	/// This should always get called when the StateMachine trys to change the current state
	/// And this is the function that allows which new state can be entered
	/// </summary>
	/// <param name="deltaTime"> deltatime of the application </param>
	/// <param name="newStateRequest"> newest Requested State Change </param>
	/// <returns> returns the new desired active state </returns>
	public T UpdateState(float deltaTime, T newStateRequest);

	/// <summary>
	/// This gets called when this state becomes the new current active state
	/// Subcribing to events or safing default data should happen here
	/// </summary>
	public void StartState(T oldState);

	/// <summary>
	/// This is the update part of the state where logic that needs to be called everyframe needs to go
	/// </summary>
	/// <param name="deltaTime"> deltatime of the application </param>
	public void ExecuteState(float deltaTime);

	/// <summary>
	/// Lateupdate of this state
	/// </summary>
	/// <param name="deltaTime"></param>
	public void LateExecuteState(float deltaTime);

	/// <summary>
	/// This is the update part of the state where logic that needs to be called everyframe needs to go
	/// </summary>
	/// <param name="deltaTime"> deltatime of the application </param>
	public void FixedExecuteState(float deltaTime);

	/// <summary>
	/// This gets called when a new state will become the new active state and this one will end
	/// Unsubcribing to events or clean up data here
	/// </summary>
	public void EndState(T newState);
}
