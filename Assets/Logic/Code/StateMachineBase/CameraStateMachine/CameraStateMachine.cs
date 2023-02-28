using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraStates
{
	Unknown,
	Default,
	MultipleTargets,
}

public class CameraStateMachine : AStateMachineBase<CameraStates>
{
	CameraController comController;
	protected override void Start()
	{
		comController = GetComponent<CameraController>();
		base.Start();
	}

	public override bool CompareStateTypes(CameraStates A, CameraStates B)
	{
		return A == B;
	}

	public override IState<CameraStates> CreateUnknownStartState()
	{
		IState<CameraStates> unknownState = null;
		CreateState(CameraStates.Unknown, out unknownState);
		return unknownState;
	}

	public override CameraStates GetUnknownT()
	{
		return CameraStates.Unknown;
	}

	public override void InitPreviousStatesWithUnknown()
	{
		for (int i = 0; i < PreviousStateListLenght; i++)
		{
			PreviousStates[i] = CreateUnknownStartState();
		}
	}

	public override bool IsStateTypeUnknown(CameraStates stateType)
	{
		return stateType == CameraStates.Unknown;
	}

	protected override IState<CameraStates> CreateDefaultState()
	{
		IState<CameraStates> newState = null;
		CreateState(CameraStates.Default, out newState);
		return newState;
	}

	protected override bool CreateState(CameraStates stateType, out IState<CameraStates> newState)
	{
		switch (stateType)
		{
			case CameraStates.Default: newState = new DefaultCameraState(this, comController); break;
			case CameraStates.MultipleTargets: newState = new MultiTargetCamerState(this, comController); break;
			default:
				newState = null;
				return false;
		}
		return true; 
	}
}
