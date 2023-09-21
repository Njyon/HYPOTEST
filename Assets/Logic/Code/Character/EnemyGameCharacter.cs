using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGameCharacter : GameCharacter
{
	BehaviorTreeRunner btRunner;

	protected override void Awake()
	{
		base.Awake();
	}

	public override void CustomAwake()
	{
		btRunner = GetComponent<BehaviorTreeRunner>();
		base.CustomAwake();
		btRunner.onBehaviourTreeInit += OnBehaviourTreeInit;
	}

	new protected void Update()
	{
		if (!IsInitialized) return;
		base.Update();
	}

	new protected void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void OnCharacterDetectionOverlapEnter(GameCharacter other)
	{
		if (other != null && other.IsPlayerCharacter && btRunner != null && btRunner.BehaviourTree != null)
		{
			btRunner.BehaviourTree.Variable.TryGetParam<GameCharacter>("Target", out var gameCharacterRef);
			if (gameCharacterRef.Value == null)
				btRunner.BehaviourTree.Variable.TrySetValue<GameCharacter>("Target", other);
		}
	}

	void OnBehaviourTreeInit()
	{
		foreach (GameCharacter gc in CharacterDetection.OverlappingGameCharacter)
		{
			OnCharacterDetectionOverlapEnter(gc);
		}
	}
}