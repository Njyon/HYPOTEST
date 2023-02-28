using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A State MachineBase that can be used for all kinds of statemachines in projects
/// </summary>
/// <typeparam name="T"> T should be the state ENUM </typeparam>
public abstract class AStateMachineBase<T> : MonoBehaviour 
{
	/// <summary>
	/// <para> Triggers when the current State changes </para> 
	/// <br> Parameter "newState" is the new currentState </br>
	/// <br> Parameter "oldState" is the old currentState </br>
	/// </summary>
	public OnStateChanged onStateChanged;
	public delegate void OnStateChanged(IState<T> newState, IState<T> oldState);

	/// <summary>
	/// The lenght of the privousState List, 
	///  <br> if 0, behaviour is deactivated </br>
	/// </summary>
	[SerializeField] int previousStateListLenght = 5;
	public int PreviousStateListLenght { get { return previousStateListLenght; } }
	private IState<T>[] previousStates;
	public IState<T>[] PreviousStates { get { return previousStates; } }

	private List<T> lazyStateList;
	public List<T> LazyStateList { get { return lazyStateList; } }

	/// <summary>
	/// if true restart the current state
	/// </summary>
	bool forceNextState = false;

	/// <summary>
	/// Current Active State
	/// </summary>
	public IState<T> CurrentState { 
		get { return currentState; } 
		set
		{
			IState<T> oldState = currentState;
			currentState?.EndState(value.GetStateType());
			currentState = value;
			if (previousStateListLenght > 0) ReorderPrivousStateList(oldState);
			currentState?.StartState(oldState != null ? oldState.GetStateType() : GetUnknownT());
			if (onStateChanged != null) onStateChanged(currentState, oldState);
		}
	}
	private IState<T> currentState;

	/// <summary>
	/// The newset State that got requested
	/// <br> Trys to be changed is late updated </br>
	/// </summary>
	public T NewestStateChangeRequestState { get; set; }

	protected virtual void Awake()
	{
		// States might need default initialization
		previousStates = new IState<T>[previousStateListLenght];
		InitPreviousStatesWithUnknown();
		lazyStateList = new List<T>();
		currentState = CreateUnknownStartState();

	}

	protected virtual void Start()
	{
		CurrentState = CreateDefaultState();
		ResetNewestRequestedState();
	}

	protected virtual void Update()
	{
		CurrentState?.ExecuteState(Time.deltaTime);
	}

	protected virtual void FixedUpdate()
	{
		CurrentState?.FixedExecuteState(Time.deltaTime);
	}

	protected virtual void LateUpdate()
	{
		// Lazy State Override if Newest stat is current state or NewestState is UNKNOWN
		// This is because Requested states have higher priority as LazyState ... i think (needs testing)
		if (LazyStateList.Count > 0)
		{
			if (CompareStateTypes(NewestStateChangeRequestState, CurrentState.GetStateType()) || IsStateTypeUnknown(NewestStateChangeRequestState))
			{
				NewestStateChangeRequestState = LazyStateList[LazyStateList.Count - 1];
			}
		}

		T newStateType = CurrentState.UpdateState(Time.deltaTime, NewestStateChangeRequestState); 
		if ((!CompareStateTypes(newStateType, CurrentState.GetStateType()) || forceNextState) && !IsStateTypeUnknown(newStateType))
		{
			forceNextState = false;
			IState<T> newState;
			bool found = CreateState(newStateType, out newState);
			if (found) CurrentState = newState;
		}
		ResetNewestRequestedState();

		CurrentState?.LateExecuteState(Time.deltaTime);
	}

	protected virtual void OnDestroy()
	{
		CurrentState?.EndState(currentState.GetStateType());
	}

	private void ReorderPrivousStateList(IState<T> oldState)
	{
		for(int i = PreviousStates.Length - 1; i > 1; i--)
		{
			if (PreviousStates.Length > i) PreviousStates[i] = PreviousStates[i - 1];
		}
		PreviousStates[0] = oldState;
	}

	public T GetCurrentStateType()
	{
		return CurrentState.GetStateType();
	}

	/// <summary>
	/// Request a new State based on T
	/// <br> Requests will get consumed by newer Request </br>
	/// <br> Request will get processed on LateUpdate </br>
	/// </summary>
	/// <param name="newState"> The new State based on Type T </param>
	/// <param name="forceState"> if True force the next state change, if current state and T are the same, restart T</param>
	public void RequestStateChange(T newState, bool forceState = false)
	{
		forceNextState = forceState;
		NewestStateChangeRequestState = newState;
	}

	/// <summary>
	/// Adds a Lazy State to the lazy state list
	/// <br> A Lazy State is a state that trys to become the active state until it got removed or there is a newer lazyState above </br>
	/// <br> This implementation acts like a Stack, the newest (on top) will get executed </br>
	/// <para> The State cant be added if he already is in the list, no need to check it before calling this function </para>
	/// </summary>
	/// <param name="stateType"> The new lazy State of type T </param>
	public void AddLazyState(T stateType)
	{
		if (LazyStateList.Contains(stateType))
			return;
		LazyStateList.Add(stateType);
	}

	/// <summary>
	/// Remove the lazy state of type T from the lazy state list
	/// <br> No need to check if state Type is in list befor this funktion, we check it here </br>
	/// </summary>
	/// <param name="stateType"> State Type that should be removed </param>
	/// <returns> Return True if the state could be removed </returns>
	public bool RemoveLazyState(T stateType)
	{
		if (!LazyStateList.Contains(stateType))
			return false;
		return LazyStateList.Remove(stateType);
	}

	/// <summary>
	/// <para> Returns the previous State Type </para>
	/// <br> IMPORTANT returns state UNKNOWN if "previousStateListLenght" 0 </br>
	/// </summary>
	/// <returns> The Previous State Type </returns>
	public T PreviousState()
	{
		return PreviousState(0);
	}

	/// <summary>
	/// <para> Returns the previous State Type at index position </para>
	/// <br> IMPORTANT returns state UNKNOWN if "previousStateListLenght" 0 </br>
	/// </summary>
	/// <param name="index"> position of the previous state type, 0 is previous state </param>
	/// <returns> The Previous State Type at index position </returns>
	public T PreviousState(int index)
	{
		if (PreviousStates.Length > index && PreviousStates[index] != null)
			return PreviousStates[index].GetStateType();
		else
			return GetUnknownT();
	}

	/// <summary>
	/// Set "NewestStateChangeRequestState" to ENUM State UNKNOWN aka 0;
	/// </summary>
	public void ResetNewestRequestedState()
	{
		NewestStateChangeRequestState = GetUnknownT();
	}

	/// <summary>
	/// Normal State change Request will change state in Late Update, if state change needed instand Force the State
	/// <br> SHOULD BE USED NORMALY </br>
	/// </summary>
	/// <param name="newState"> Desired new State </param>
	/// <param name="forceIlligal"> FORCE state even if trasition is not allowed! Only use if you know what u are doing </param>
	/// <returns></returns>
	public bool ForceStateChange(T newState, bool forceIlligal = false)
	{
		IState<T> state;
		bool couldCreate = CreateState(newState, out state);
		T transition = CurrentState.UpdateState(0, newState);
		if (!forceIlligal)
		{
			if (couldCreate && CompareStateTypes(transition, newState))
			{
				CurrentState = state;
				ResetNewestRequestedState();
			}
		}
		else
		{
			if (couldCreate)
			{
				CurrentState = state;
				ResetNewestRequestedState();
			}
		}

		return couldCreate;
	}

	/// <summary>
	/// Compare State Type A with State Type B
	/// <br> NEEDS to be an extra function because ==Operator is not allowed on T </br>
	/// </summary>
	/// <param name="A"></param>
	/// <param name="B"></param>
	/// <returns> Return True if A and B are the same </returns>
	public abstract bool CompareStateTypes(T A, T B);

	/// <summary>
	/// Create a new State of type T
	/// </summary>
	/// <param name="stateType"> The Type of the state that should be created </param>
	/// <param name="newState"> The created state </param>
	/// <returns> Returns true if the state got created </returns>
	protected abstract bool CreateState(T stateType, out IState<T> newState);

	/// <summary>
	/// Create the Default Starting State
	/// </summary>
	/// <returns></returns>
	protected abstract IState<T> CreateDefaultState();

	/// <summary>
	/// Check if state type is UNKNOWN
	/// </summary>
	/// <param name="stateType"></param>
	/// <returns> Return True if stateType is UNKNOWN aka 0 </returns>
	public abstract bool IsStateTypeUnknown(T stateType);

	/// <summary>
	/// Needed so the array operates ob correct values
	/// </summary>
	public abstract void InitPreviousStatesWithUnknown();

	public abstract IState<T> CreateUnknownStartState();

	public abstract T GetUnknownT();
}
