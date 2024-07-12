using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Action")]
public class BTSetTeleportForce : BTHeraklesActionNodeBase
{
	[Header("TeleportForceData")]
	public bool newValue = true;

	protected override void OnEnter(object options = null)
	{
		base.OnEnter(options);

		TeleportAttackForce = newValue;
	}
}
