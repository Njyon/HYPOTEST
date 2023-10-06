using System;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using Megumin.Reflection;
using Megumin.Serialization;
using UnityEngine;

[Category("Action")]
public sealed class TestActionNode : BTActionNode
{
	public bool Success = true;
	GameCharacter self = null;
	GameCharacter target = null;
	GameObject SelfObj = null;
	public RefVar_GameObject obj;
	protected override Status OnTick(BTNode from, object options = null)
	{
		return Success ? Status.Succeeded : Status.Failed;
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
	}

	protected override void OnEnter(object options = null)
	{
		base.OnEnter(options);
		if (Tree.TryGetRefValue<RefVar_GameObject>("SelfObj", out var refGameCharacter))
		{
			SelfObj = refGameCharacter.Value;
		}

		Tree.Variable.TrySetValue<string>("Test", SelfObj.name);
		if (Tree.Variable.TryGetParam<string>("Test", out var name))
		{
			Debug.Log("Self: " + name.Value /*+ "  & Target: " + target.name*/);
		}


	}
}
