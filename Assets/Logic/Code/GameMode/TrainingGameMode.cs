using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingGameMode : GameModeBase
{
	public override bool AllowDamage()
	{
		return false;
	}

	public override void Create()
	{
		base.Create();
	}

	public TrainingModeSpecificData GetTrainingModeSpecificData()
	{
		return GameAssets.Instance.trainingModeSpecificData;
	}
}
