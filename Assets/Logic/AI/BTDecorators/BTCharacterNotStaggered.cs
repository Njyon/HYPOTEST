using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Condition")]
public class BTCharacterNotStaggered : BTHyppoliteConditionDecoratorBase
{
	protected override bool OnCheckCondition(object options = null)
	{
		switch (GameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Freez:
			case EGameCharacterState.MoveToPosition:
			case EGameCharacterState.HookedToCharacter:
			case EGameCharacterState.FlyAway:
			case EGameCharacterState.PullCharacterOnHorizontalLevel:
				return false;
			default: break;
		}
		return true;
	}
}
