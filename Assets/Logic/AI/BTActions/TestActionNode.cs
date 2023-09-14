using System;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Category("Action")]
public sealed class TestActionNode : BTActionNode
{
	public bool Success = true;
	protected override Status OnTick(BTNode from, object options = null)
	{
		GameObject self = null;
		GameCharacter gameCharacter = null;
		Tree.Variable.TrySetValue<GameCharacter>("Self", gameCharacter);
		RefVar_GameCharacter selfVar = new RefVar_GameCharacter();
		selfVar.RefName = "Self";
		selfVar.value = gameCharacter;

		Tree.Variable.Table.Add(selfVar);

		if (Tree.Variable.TryGetParam<GameObject>("OwnGameObject", out  var variable))
		{
			self = variable.Value;
		}
		return Success ? Status.Succeeded : Status.Failed;
	}
}
