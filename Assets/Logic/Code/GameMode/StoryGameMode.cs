using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryGameMode : GameModeBase
{
	public override bool AllowDamage()
	{
		return true;	
	}

	public override void Create()
	{
		base.Create();
	}

	public StoryModeSpecificData GetStoryModeData()
	{
		return GameAssets.Instance.storyModeSpecificData;
	}
}
