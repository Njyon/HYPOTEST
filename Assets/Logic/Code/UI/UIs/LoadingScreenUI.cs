using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenUI : UIBase
{
	void Awake()
    {
        LoadedUI();

        UIManager.Instance.UIStack.Clear();
    }

	public override void StartRemovingUI()
	{
		RemoveUI();
	}
}
