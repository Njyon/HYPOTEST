using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCameraState : ACameraState
{
	Vector3 camerVel;
	public DefaultCameraState(CameraStateMachine stateMachine, CameraController cameraController, GameCharacter gameCharacter) : base(stateMachine, cameraController, gameCharacter)
	{ }

	public override void StartState(ECameraStates oldState)
	{
		camerVel = Vector3.zero;
	}

	public override ECameraStates GetStateType()
	{
		return ECameraStates.Default;
	}

	public override ECameraStates UpdateState(float deltaTime, ECameraStates newStateRequest)
	{
		if (CameraController.Targets.Count > 1) return ECameraStates.MultipleTarget;

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
		Vector3 targetPosition =  Vector3.SmoothDamp(CameraController.Camera.transform.position, target.position + targetDirection, ref camerVel, CameraController.Damping) ;
		targetPosition.z = CameraController.CameraTargetPosition.z;

		// Keep the camera within the bounds of the scene
		float xMin = target.position.x + CameraController.ClampX.x;
		float xMax = target.position.x + CameraController.ClampX.y;
		float x = Mathf.Clamp(targetPosition.x, xMin, xMax);
		float yMin = (GameCharacter.MovementComponent.PossibleGround != null) ? GameCharacter.MovementComponent.PossibleGround.hit.point.y + CameraController.Offset.y : target.position.y + CameraController.ClampY.x;
		float yMax = target.position.y + CameraController.ClampX.y;
		float y = Mathf.Clamp(targetPosition.y, yMin, yMax);

		Ultra.Utilities.DrawWireSphere(new Vector3(CameraController.transform.position.x, yMin, CameraController.transform.position.z), 0.2f, Color.magenta, 0, 100, DebugAreas.Camera);
		Ultra.Utilities.DrawWireSphere(new Vector3(CameraController.transform.position.x, y, CameraController.transform.position.z), 0.2f, Color.magenta, 0, 100, DebugAreas.Camera);

		CameraController.CameraTargetPosition = new Vector3(x, y, CameraController.CameraTargetPosition.z);
	}

	public override void EndState(ECameraStates newState)
	{

	}
}
