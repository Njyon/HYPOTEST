using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Condition")]
public class BTIsCombatReady : BTHyppoliteConditionDecoratorBase
{
	protected override bool OnCheckCondition(object options = null)
	{
		switch (GameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Freez:
			case EGameCharacterState.FlyAway:
			case EGameCharacterState.MoveToPosition:
			case EGameCharacterState.HookedToCharacter:
			case EGameCharacterState.PullCharacterOnHorizontalLevel:
			case EGameCharacterState.Sliding:
				return false;
			default:
				break;
		}
		return true;
	}
}
