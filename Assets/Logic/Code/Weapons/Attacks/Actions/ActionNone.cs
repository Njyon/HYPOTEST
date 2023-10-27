using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNone : ActionBase
{
	public override void StartAction()
	{

	}

	public override ActionBase CreateCopy()
	{
		return new ActionNone();
	}
}
