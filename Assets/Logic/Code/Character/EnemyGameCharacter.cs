using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Megumin.GameFramework.AI.BehaviorTree.BehaviorTreeRunner;

public class EnemyGameCharacter : GameCharacter
{
	BehaviorTreeRunner btRunner;
	public BehaviorTreeRunner BTRunner { get { return btRunner; } set { btRunner = value; } }

	public ParticleSystem attackFeedback;

	protected override void Awake()
	{
		base.Awake();
	}

	public override void CustomAwake()
	{
		base.CustomAwake();

		if (btRunner != null)
		{
			if (btRunner.BehaviourTree != null)
				OnBehaviourTreeInit();
			else
				btRunner.onBehaviourTreeInit += OnBehaviourTreeInit;
		}
	}

	new protected void Update()
	{
		if (!IsInitialized) return;
		base.Update();
	}

	new protected void OnDestroy()
	{
		if (btRunner != null) 
			btRunner.onBehaviourTreeInit -= OnBehaviourTreeInit;
		base.OnDestroy();
	}

	protected override void OnCharacterDetectionOverlapEnter(GameCharacter other)
	{
		if (other != null && other.IsPlayerCharacter && btRunner != null && btRunner.BehaviourTree != null)
		{
			btRunner.BehaviourTree.Variable.TryGetParam<GameCharacter>("Target", out var gameCharacterRef);
			if (gameCharacterRef != null && gameCharacterRef.Value == null)
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

	public override void ShowAttackFeedback()
	{
		if (attackFeedback == null)
		{
			if (!RigDataComponent.Bones.ContainsKey(GameCharacterData.HeadBoneName))
			{
				Ultra.Utilities.Instance.DebugErrorString("EnemyGameCharacter", "ShowAttackFeedback", "Head Bonename not Valid!");
				return;
			}
			GameObject go = Instantiate(GameAssets.Instance.DefaultAttackFeedback, RigDataComponent.Bones[GameCharacterData.HeadBoneName]);
			go.transform.Translate(GameCharacterData.AttackFeedbackOffset, Space.World);

			attackFeedback = go.GetComponent<ParticleSystem>();

			if (attackFeedback == null)
				Ultra.Utilities.Instance.DebugErrorString("EnemyGameCharacter", "ShowAttackFeedback", "Particle System is null!");
		}
		attackFeedback.Play();
	}
}