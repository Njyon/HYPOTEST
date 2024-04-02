using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVCameraModification : ACameraModification
{
	[Range(-1f,1f)]
	public float addativeFOV = 0;

	public override void DoOperation()
	{
		CameraController.AddativeFOVTarget = addativeFOV;
	}
}
