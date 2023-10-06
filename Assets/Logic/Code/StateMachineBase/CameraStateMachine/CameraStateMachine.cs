using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public enum ECameraStates
{
	Unknown,
	Default,
	MultipleTarget,
}

public class CameraStateMachine : AStateMachineBase<ECameraStates>
{
	CameraController camController;
	protected override void Start()
	{
		camController = GetComponent<CameraController>();
		base.Start();
	}

	public override bool CompareStateTypes(ECameraStates A, ECameraStates B)
	{
		return A == B;
	}

	public override IState<ECameraStates> CreateUnknownStartState()
	{
		IState<ECameraStates> unknownState = null;
		CreateState(ECameraStates.Unknown, out unknownState);
		return unknownState;
	}

	public override ECameraStates GetUnknownT()
	{
		return ECameraStates.Unknown;
	}

	public override void InitPreviousStatesWithUnknown()
	{
		for (int i = 0; i < PreviousStateListLenght; i++)
		{
			PreviousStates[i] = CreateUnknownStartState();
		}
	}

	public override bool IsStateTypeUnknown(ECameraStates stateType)
	{
		return stateType == ECameraStates.Unknown;
	}

	protected override IState<ECameraStates> CreateDefaultState()
	{
		IState<ECameraStates> newState = null;
		CreateState(ECameraStates.Default, out newState);
		return newState;
	}

	protected override bool CreateState(ECameraStates stateType, out IState<ECameraStates> newState)
	{
		newState = null;
		switch (stateType)
		{
			case ECameraStates.Unknown: break;
				case ECameraStates.Default: newState = new DefaultCameraState(this, camController, camController.GameCharacter); break;
				case ECameraStates.MultipleTarget: newState = new MultiTargetCamerState(this, camController, camController.GameCharacter); break;
			default:
				Ultra.Utilities.Instance.DebugErrorString("CamerStateMaschine", "CreateState", "Camera State has no valid Implementation!");
				break;
		}
		//var stateTypes = Assembly.GetExecutingAssembly().GetTypes()
		//	.Where(t => t.BaseType == typeof(ACameraState))
		//	.ToList();
		//
		//var stateClass = stateTypes.FirstOrDefault(t => t.Name == stateType.ToString() + "CameraState");
		//
		//if (stateClass == null)
		//{
		//	newState = null;
		//	return false;
		//}
		//
		//newState = Activator.CreateInstance(stateClass, this, camController, camController.GameCharacter) as ACameraState;

		return newState != null;
	}
}
