using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCameraModification : ACameraModification
{
	public override void DoOperation()
	{
		// Reset All Operations
		CameraController.RotationTarget = CameraController.DefaultRotationTarget;
		CameraController.AddativeOffsetTarget = Vector3.zero;
		CameraController.AddativeFOVTarget = 0;
	}
}
