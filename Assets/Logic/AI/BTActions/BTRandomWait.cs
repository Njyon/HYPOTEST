using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Action")]
[Icon("d_unityeditor.animationwindow@2x")]
public class BTRandomWait : BTHyppoliteActionNodeBase, IDetailable
{
	public float maxWaitTime = 5f;
	public float minWaitTime = 0f;

	float waitTime;
	float enterTime;
	float left;

	protected override void OnEnter(object options = null)
	{
		waitTime = Random.Range(minWaitTime, maxWaitTime);
		enterTime = Time.time;
		left = waitTime;
	}

	protected override Status OnTick(BTNode from, object options = null)
	{
		left = waitTime - (Time.time - enterTime);
		if (left <= 0) 
		{
			return Status.Succeeded;
		}
		return Status.Running;

	}

	public string GetDetail()
	{
		if (State == Status.Running)
		{
			return $"Wait: {(float)waitTime:0.000}  Left:{left:0.000}";
		}
		else
		{
			return $"Wait: {(float)waitTime:0.000}";
		}
	}
}
