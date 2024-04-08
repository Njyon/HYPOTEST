using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargetCamerState : ACameraState
{
	public MultiTargetCamerState(CameraStateMachine stateMachine, CameraController cameraController, GameCharacter gameCharacter) : base(stateMachine, cameraController, gameCharacter)
	{ }

	public override void StartState(ECameraStates oldState)
	{

	}

	public override ECameraStates GetStateType()
	{
		return ECameraStates.MultipleTarget;
	}

	public override ECameraStates UpdateState(float deltaTime, ECameraStates newStateRequest)
	{
		if (CameraController.Targets.Count <= 1) return ECameraStates.Default;

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{

	}

	public override void FixedExecuteState(float deltaTime)
	{

	}

	public override void LateExecuteState(float deltaTime)
	{
		if (CameraController.Targets.Count <= 1 || deltaTime == 0) return;

		// Berechne den Mittelpunkt der Ziele
		Bounds characterBounds = GetCharacterBounds();
		float longerBoundsSide = Mathf.Max(characterBounds.size.x, characterBounds.size.y);

		float fovTarget = Mathf.Lerp(CameraController.MinFOV, CameraController.MaxFoV, longerBoundsSide / CameraController.ZoomLimiter);
		CameraController.FinalFoV = Mathf.Lerp(CameraController.FinalFoV, fovTarget, Time.deltaTime * CameraController.MultiZoomSpeed);
		CameraController.FinalCameraPosition = Vector3.Lerp(CameraController.FinalCameraPosition, characterBounds.center + CameraController.MultiOffset + CameraController.Offset, Time.deltaTime * CameraController.MultiInterpSpeed);
	}

	Bounds GetCharacterBounds()
	{
		// Berechne den Mittelpunkt der Ziele
		if (CameraController.Targets.Count == 1)
		{
			return new Bounds(CameraController.Targets[0].MovementComponent.CharacterCenter, Vector3.zero);
		}

		Bounds bounds = new Bounds(CameraController.Targets[0].MovementComponent.CharacterCenter, Vector3.zero);
		foreach (GameCharacter target in CameraController.Targets)
		{
			bounds.Encapsulate(target.MovementComponent.CharacterCenter);
		}

		return bounds;
	}

	public override void EndState(ECameraStates newState)
	{

	}
}
