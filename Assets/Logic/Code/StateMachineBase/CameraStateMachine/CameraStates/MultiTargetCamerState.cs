using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargetCamerState : ACameraState
{
	public MultiTargetCamerState(CameraStateMachine stateMachine, CameraController cameraController) : base(stateMachine, cameraController)
	{ }

	public override void StartState(CameraStates oldState)
	{

	}

	public override CameraStates GetStateType()
	{
		return CameraStates.MultipleTargets;
	}

	public override CameraStates UpdateState(float deltaTime, CameraStates newStateRequest)
	{
		if (CameraController.Targets.Count > 1) return CameraStates.Default;

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
		if (CameraController.Targets.Count == 0) return;

		// Ermittlung der Bounding Box, die alle Targets umschlieﬂt
		Bounds bounds = new Bounds(CameraController.Targets[0].position, Vector3.zero);
		for (int i = 0; i < CameraController.Targets.Count; i++)
		{
			bounds.Encapsulate(CameraController.Targets[i].position);
		}

		// Berechnung der neuen Kamera-Position
		Vector3 targetPos = bounds.center + CameraController.Offset;
		float distance = Mathf.Max(bounds.size.x, bounds.size.y) / 2f / Mathf.Tan(CameraController.Camera.fieldOfView / 2f * Mathf.Deg2Rad);

		// Anpassung der Kamera-Zoomstufe, um die mindestens Entfernung und minimale Zoomstufe zu ber¸cksichtigen
		float zoomVel = CameraController.ZoomVelocity;
		float zoomLevel = Mathf.SmoothDamp(CameraController.Camera.orthographicSize, distance, ref zoomVel, CameraController.SmoothTime);
		CameraController.ZoomVelocity = zoomVel;

		zoomLevel = Mathf.Clamp(zoomLevel, CameraController.MinZoom, CameraController.MaxZoom);
		distance = Mathf.Clamp(distance, CameraController.MinDistance, Mathf.Infinity);

		// Berechnung der Kamera-Position auf der Z-Achse
		targetPos += Vector3.back * distance;

		// Interpolation der Kamera-Position
		Vector3 vel = CameraController.Velocity;
		CameraController.CameraTargetPosition = Vector3.SmoothDamp(CameraController.CameraTargetPosition, targetPos, ref vel, CameraController.SmoothTime);
		CameraController.Velocity = vel;

		// Ausrichtung der Kamera
		CameraController.transform.LookAt(bounds.center);
	}

	public override void EndState(CameraStates newState)
	{

	}
}
