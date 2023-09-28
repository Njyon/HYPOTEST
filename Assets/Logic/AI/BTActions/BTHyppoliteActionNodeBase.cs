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
					if (Tree.Variable.TryGetParam<GameObject>("SelfObj", out var go))
					{
						if (go != null && go.Value != null)
						{
							gameCharacter = go.Value.GetComponent<GameCharacter>();
							Tree.Variable.TrySetValue<GameCharacter>("Self", gameCharacter);
						}else
						{
							Debug.Log("Self and SelfObj was null in Behaviour Tree, but still used BTHyppoliteActionNodeBase... something is fucked!");
						}
					}
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
}
