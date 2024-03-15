using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum EConditionCheck
{
	Or,
	And,
}

[Category("Condition")]
public class BTTargetDistanceCondition : BTHyppoliteConditionDecoratorBase
{
	public EConditionCheck conditonCheck = EConditionCheck.And;
	public List<ClassInstance<ConditionOperation>> conditions = new List<ClassInstance<ConditionOperation>>();

	protected override bool OnCheckCondition(object options = null)
	{
		bool result = false;
		foreach (ClassInstance<ConditionOperation> condition in conditions)
		{
			condition.instance.Init(GameCharacter, TargetGameCharacter);
			result = condition.instance.DoOperation();

			switch (conditonCheck)
			{
				case EConditionCheck.Or:
					if (result)
						return true;
					break;
				case EConditionCheck.And:
					if (!result) 
						return false;
					break;
				default: break;
			}

		}
		return result;
	}
}
