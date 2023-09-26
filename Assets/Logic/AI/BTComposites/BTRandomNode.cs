using Megumin.Binding;
using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BTRandomNode : BTHyppoliteCompositeNodeBase
{
	bool start = true;

	public List<float> weightList = new List<float>();
	int childToExecute = 0;

	protected override void OnEnter(object options = null)
	{
		base.OnEnter(options);

		if (start)
		{
			start = false;
			childToExecute = 0;
			AdjustListLenght();
			GetRandomChildToExecute();
		}
	}

	private void GetRandomChildToExecute()
	{
		float totalWeight = weightList.Sum(e => e);
		float randomValue = Random.Range(0, totalWeight);
		float cumulativeWeight = 0;
		for (int i = 0; i < Children.Count; i++)
		{
			cumulativeWeight += weightList[i];
			if (randomValue <= cumulativeWeight)
			{
				childToExecute = i;
				break;
			}
		}
	}

	protected override Status OnTick(BTNode from, object options = null)
	{
		BTNode target = Children[childToExecute];
		if (target == null) return Status.Failed;

		return target.Tick(this, options);
	}

	protected override void OnExit(Status result, object options = null)
	{
		base.OnExit(result, options);

		switch (result)
		{
			case Status.Failed:
			case Status.Succeeded:
				start = true;
				break;
			default: break;
		}
	}

	void AdjustListLenght()
	{
		int maxLenght = Children.Count;
		if (weightList.Count < maxLenght)
		{
			while (weightList.Count < maxLenght)
				weightList.Add(1f);
		}
		else if (weightList.Count > maxLenght)
		{
			while (weightList.Count > maxLenght)
				weightList.RemoveAt(weightList.Count - 1);
		}
	}
}
