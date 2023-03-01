using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCameraState : ACameraState
{
	private Vector3 velocityVel = Vector3.zero;
	public DefaultCameraState(CameraStateMachine stateMachine, CameraController cameraController, GameCharacter gameCharacter) : base(stateMachine, cameraController, gameCharacter)
	{ }

	public override void StartState(CameraStates oldState)
	{

	}

	public override CameraStates GetStateType()
	{
		return CameraStates.Default;
	}

	public override CameraStates UpdateState(float deltaTime, CameraStates newStateRequest)
	{
		if (CameraController.Targets.Count > 1) return CameraStates.MultipleTargets;

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
		if (CameraController.Targets.Count <= 0) return;
		Transform target = CameraController.Targets[0];
		if (!target) return;

		// Get the target's velocity and direction
		Vector3 targetDirection = CameraController.MainTargetVelocity * (CameraController.LookAhead * CameraController.Speed);

		// Move the camera towards the target position and direction
		Vector3 targetPosition = target.position + targetDirection;
		Vector3 test = Vector3.SmoothDamp(CameraController.CameraTargetPosition, targetPosition, ref velocityVel, 1 / CameraController.MoveSpeed);
		test.z = CameraController.CameraTargetPosition.z;

		// Keep the camera within the bounds of the scene
		float xMin = target.position.x + CameraController.ClampX.x;
		float xMax = target.position.x + CameraController.ClampX.y;
		float x = Mathf.Clamp(test.x, xMin, xMax);
		float yMin = (GameCharacter.PossibleGround != null) ? GameCharacter.PossibleGround.hit.point.y + CameraController.Offset.y : target.position.y + CameraController.ClampY.x;
		float yMax = target.position.y + CameraController.ClampX.y;
		float y = Mathf.Clamp(test.y, yMin, yMax);

		Ultra.Utilities.DrawWireSphere(new Vector3(CameraController.transform.position.x, yMin, CameraController.transform.position.z), 0.2f, Color.magenta, 0, 100, DebugAreas.Camera);
		Ultra.Utilities.DrawWireSphere(new Vector3(CameraController.transform.position.x, y, CameraController.transform.position.z), 0.2f, Color.magenta, 0, 100, DebugAreas.Camera);

		CameraController.CameraTargetPosition = new Vector3(x, y, CameraController.CameraTargetPosition.z);
	}

	public override void EndState(CameraStates newState)
	{

	}
}
