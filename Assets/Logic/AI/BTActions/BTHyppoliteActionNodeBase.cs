using Megumin.GameFramework.AI.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTHyppoliteActionNodeBase : BTActionNode
{
	GameCharacter gameCharacter;
	GameCharacter targetGameCharacter;

	public GameCharacter GameCharacter { 
		get 
		{ 
			if (gameCharacter == null)
			{
				if (Tree.Variable.TryGetParam<GameCharacter>("Self", out var gc))
				{
					gameCharacter = gc.Value;
				}
				if (gameCharacter == null)
				{
					Debug.Log("Self and SelfObj was null in Behaviour Tree, but still used BTHyppoliteActionNodeBase... something is fucked!");
				}
			
			}
			return gameCharacter; 
		} 
	}

	public GameCharacter TargetGameCharacter { 
		get
		{
			if (targetGameCharacter == null)
			{
				if (Tree.Variable.TryGetParam<GameCharacter>("Target", out var gc))
				{
					targetGameCharacter = gc.Value;
				}
				//if (targetGameCharacter == null)
				//{
				//	targetGameCharacter = Ultra.HypoUttilies.GetGameMode().PlayerGameCharacter;
				//	Tree.Variable.TrySetValue<GameCharacter>("Target", targetGameCharacter);
				//	
				//	if (targetGameCharacter == null)
				//	{
				//		Debug.Log("Target was null in Behaviour Tree, but still used BTHyppoliteActionNodeBase... something is fucked!");
				//	}
				//}

			}
			return targetGameCharacter; 
		} 
	}

	protected bool CanAttackBeExecuted()
	{
		if (GameCharacter.MovementComponent.IsInJump) return false;
		if (GameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Attack) return false;
		if (GameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Dodge) return false;
		if (GameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Freez) return false;
		if (GameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.MoveToPosition) return false;
		if (GameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.HookedToCharacter) return false;
		if (GameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.PullCharacterOnHorizontalLevel) return false;
		return GameCharacter?.StateMachine?.CurrentState?.UpdateState(0, EGameCharacterState.Attack) == EGameCharacterState.Attack;
	}
}
