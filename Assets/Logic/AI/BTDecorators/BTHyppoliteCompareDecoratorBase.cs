using Megumin.GameFramework.AI.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTHyppoliteCompareDecoratorBase<T> : CompareDecorator<T> where T : IComparable<T>
{
	GameCharacter gameCharacter;
	GameCharacter targetGameCharacter;

	public GameCharacter GameCharacter
	{
		get
		{
			if (gameCharacter == null)
			{
				if (Tree.RootTree.Variable.TryGetParam<GameCharacter>("Self", out var gc))
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

	public GameCharacter TargetGameCharacter
	{
		get
		{
			if (targetGameCharacter == null)
			{
				if (Tree.RootTree.Variable.TryGetParam<GameCharacter>("Target", out var gc))
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
