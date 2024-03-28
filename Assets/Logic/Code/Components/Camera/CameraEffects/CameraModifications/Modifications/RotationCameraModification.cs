using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCameraModification : ACameraModification
{
	[SerializeField] Vector3 rotation;

	public override void DoOperation()
	{
		CameraController.RotationTarget = Quaternion.Euler(rotation);
	}
}
