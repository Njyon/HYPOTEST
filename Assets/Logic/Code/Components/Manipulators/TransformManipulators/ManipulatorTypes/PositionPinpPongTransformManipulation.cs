using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PositionPingPongData
{
	public bool useAxes = false;
	[ConditionalField("useAxes")] public float relativeMaxValue = 1;
	[ConditionalField("useAxes")] public AnimationCurve animationCurve;
	[ConditionalField("useAxes")] public float pingPongTime = 1;
}

public class PositionPinpPongTransformManipulation : ATransformManipulator
{
	public float offset = 0f;
	public PositionPingPongData xAxes = new PositionPingPongData(); 
	public PositionPingPongData yAxes = new PositionPingPongData(); 
	public PositionPingPongData zAxes = new PositionPingPongData();

	List<Vector3> startPostions = new List<Vector3>();

	public override void DoOperation()
	{
		base.DoOperation();

		startPostions.Clear();
		foreach (Transform t in transforms)
		{
			startPostions.Add(t.position);
		}
	}

	public override void UpdateOperation()
	{
		base.UpdateOperation();

		float time = Time.time - offset;

		for (int i = 0; i < transforms.Count; i++)
		{
			Vector3 pos = transforms[i].position;
			Vector3 startPos = startPostions[i];

			UpdateAxes(xAxes, startPos.x, time, ref pos.x);
			UpdateAxes(yAxes, startPos.y, time, ref pos.y);
			UpdateAxes(zAxes, startPos.z, time, ref pos.z);

			transforms[i].position = pos;
		}
	}

	void UpdateAxes(PositionPingPongData axesData, float startPosAxes, float time, ref float newPos)
	{
		if (axesData.useAxes)
		{
			float pingPong = Mathf.PingPong(time, axesData.pingPongTime);
			float remap = Unity.Mathematics.math.remap(0, axesData.pingPongTime, 0, 1, pingPong);
			float animCurve = axesData.animationCurve.Evaluate(remap);
			newPos = Mathf.Lerp(startPosAxes, startPosAxes + axesData.relativeMaxValue, animCurve);
		}
	}
}
