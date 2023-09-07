using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : UIBase
{

	PlayerGameCharacter gameCharacter;

	public void Init(PlayerGameCharacter playerCharacter)
	{
		gameCharacter = playerCharacter;
	}

	void Awake()
	{
		LoadedUI();	
	}

	public override void StartRemovingUI()
	{
		RemoveUI();
	}
}
