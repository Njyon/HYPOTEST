using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetCameraModification : ACameraModification
{
	public Vector3 offset;

	public override void DoOperation()
	{
		CameraController.AddativeOffsetTarget = offset;
	}
}
