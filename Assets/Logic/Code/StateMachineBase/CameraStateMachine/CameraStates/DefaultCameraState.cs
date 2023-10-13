using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
		if (!GameCharacter.IsInitialized) return;

		if (CameraController.Targets.Count <= 0) return;
		Vector3 target = CameraController.Targets[0].MovementComponent.CharacterCenter;

		// Get the target's velocity and direction
		Vector3 targetDirection = CameraController.Targets[0].MovementComponent.Velocity * (CameraController.LookAhead * CameraController.Speed);
		//Ultra.Utilities.DrawArrow(target, targetDirection, 1f, Color.blue);

		Vector3 newTargetLocation = new Vector3(target.x, target.y, CameraController.FinalCameraPosition.z);

		// Move the camera towards the target position and direction
		Vector3 targetPosition = newTargetLocation + targetDirection;   // Vector3.SmoothDamp(CameraController.FinalCameraPosition, newTargetLocation + targetDirection, ref camerVel, CameraController.Damping, Mathf.Infinity, Time.deltaTime);
		Ultra.Utilities.DrawWireSphere(targetPosition, 1, Color.red, 0);

		// Keep the camera within the bounds of the scene
		float xMin = target.x + CameraController.ClampX.x;
		float xMax = target.x + CameraController.ClampX.y;
		float x = Mathf.Clamp(targetPosition.x, xMin, xMax);
		float yMin = (GameCharacter.MovementComponent.PossibleGround != null) ? GameCharacter.MovementComponent.PossibleGround.hit.point.y + CameraController.Offset.y : target.y + CameraController.ClampY.x;
		float yMax = target.y + CameraController.ClampX.y;
		float y = Mathf.Clamp(targetPosition.y, yMin, yMax);

		CameraController.CameraTargetPosition = new Vector3(x, y, CameraController.CameraTargetPosition.z);

		Vector3 xPos = Vector3.SmoothDamp(CameraController.FinalCameraPosition, Vector3.ProjectOnPlane(CameraController.CameraTargetPosition, Vector3.up), ref CameraController.velocityVelx, 1 / CameraController.MoveSpeedx, Mathf.Infinity, Time.deltaTime);
		Vector3 yPos = Vector3.SmoothDamp(CameraController.FinalCameraPosition, Vector3.ProjectOnPlane(CameraController.CameraTargetPosition, Vector3.right), ref CameraController.velocityVely, 1 / CameraController.MoveSpeedy, Mathf.Infinity, Time.deltaTime);

		Ultra.Utilities.DrawWireSphere(xPos, 1, Color.cyan, 0);
		Ultra.Utilities.DrawWireSphere(yPos, 1, Color.magenta, 0);

		x = Mathf.Clamp(xPos.x, xMin, xMax);
		y = Mathf.Clamp(yPos.y, yMin, yMax);

		CameraController.FinalCameraPosition = new Vector3(x, y, CameraController.CameraTargetPosition.z);
		Ultra.Utilities.DrawWireSphere(CameraController.FinalCameraPosition, 1, Color.green, 0);
	}

	public override void EndState(ECameraStates newState)
	{

	}
}
