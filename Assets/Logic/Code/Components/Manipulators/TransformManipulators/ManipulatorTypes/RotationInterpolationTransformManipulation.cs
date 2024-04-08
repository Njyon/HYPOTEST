using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RotationInterpolationData
{
	public bool useAxes = false;
	[ConditionalField("useAxes")] public float roationInterpolationSpeed = 1;
	[ConditionalField("useAxes")] public bool rotateRight = false;
}

public class RotationInterpolationTransformManipulation : ATransformManipulator
{
	public float delay = 0f;

	public RotationInterpolationData xData;
	public RotationInterpolationData yData;
	public RotationInterpolationData zData;

	public async override void DoOperation()
	{
		await new WaitForSeconds(delay);
		base.DoOperation();

	}

	public override void UpdateOperation()
	{
		if (!DidStart) return;

		base.UpdateOperation();

		float deltaTime = Time.deltaTime;

		for (int i = 0; i < transforms.Count; i++)
		{
			Vector3 rotation = transforms[i].rotation.eulerAngles;

			UpdateAxes(xData, ref rotation.x, deltaTime);
			UpdateAxes(yData, ref rotation.y, deltaTime);
			UpdateAxes(zData, ref rotation.z, deltaTime);

			transforms[i].rotation = Quaternion.Euler(rotation);
		}
	}

	void UpdateAxes(RotationInterpolationData axesData, ref float currentValue, float deltaTime)
	{
		if (axesData.useAxes)
		{
			currentValue = Mathf.Lerp(currentValue, axesData.rotateRight ? currentValue + 179 : currentValue - 179, deltaTime * axesData.roationInterpolationSpeed);
		}
	}
}
