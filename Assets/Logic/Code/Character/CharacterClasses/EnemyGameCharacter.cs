using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Megumin.GameFramework.AI.BehaviorTree.BehaviorTreeRunner;

public class EnemyGameCharacter : GameCharacter
{
	BehaviorTreeRunner btRunner;
	public BehaviorTreeRunner BTRunner { get { return btRunner; } set { btRunner = value; } }

	public ParticleSystemPool attackFeedbackPool;
	Ultra.Timer desolveTimer;
	int desolveID;

	protected override void Awake()
	{
		base.Awake();

		desolveID = Shader.PropertyToID("_DissolveBlend");

		SetDesolveBlendValue(0);
		desolveTimer = new Ultra.Timer(Ultra.HypoUttilies.GameMode.GetDefaultGameModeData().desolveLenght, false);
	}

	private void SetDesolveBlendValue(float value)
	{
		if (GameCharacterData.MeshRenderer == null)
		{
			Ultra.Utilities.Instance.DebugErrorString("EnemyGameCharacter", "SetDesolveBlendValue", "MeshRenderer is not set in GameCharacterData!");
			return;
		}

		foreach (Material mat in GameCharacterData.MeshRenderer.materials)
		{
			if (mat != null)
			{
				mat.SetFloat(desolveID, value);
			}
		}
	}

	public override void CustomAwake()
	{
		base.CustomAwake();
		attackFeedbackPool = new ParticleSystemPool(GameAssets.Instance.DefaultAttackFeedback, CreateHolderChild("AttackFeedbackHolder"), 1);

		if (btRunner != null)
		{
			if (btRunner.BehaviourTree != null)
				OnBehaviourTreeInit(btRunner);
			else
				btRunner.onBehaviourTreeInit += OnBehaviourTreeInit;
		}
	}

	new protected void Update()
	{
		if (!IsInitialized) return;
		base.Update();

		if (desolveTimer.IsRunning)
		{
			desolveTimer.Update(Time.deltaTime);
			if (IsGameCharacterDead)
			{
				SetDesolveBlendValue(desolveTimer.GetProgressRevered());
			}
			else
			{
				SetDesolveBlendValue(desolveTimer.GetProgress());
			}
		}
	}

	new protected void OnDestroy()
	{
		if (btRunner != null)
			btRunner.onBehaviourTreeInit -= OnBehaviourTreeInit;
		base.OnDestroy();
	}

	protected override void OnTargetDetectionOverlapEnter(IDamage other)
	{
		if (other.IsGameCharacter())
		{
			GameCharacter gc = other.GetGameCharacter();
			if (other != null && gc.IsPlayerCharacter && btRunner != null && btRunner.BehaviourTree != null)
			{
				btRunner.BehaviourTree.Variable.TryGetParam<GameCharacter>("Target", out var gameCharacterRef);
				if (gameCharacterRef != null && gameCharacterRef.Value == null)
				{
					btRunner.BehaviourTree.Variable.TrySetValue<GameCharacter>("Target", gc);
					gc.AddCharacterToAggroedCharacters(this);
				}
			}
		}
	}

	void OnBehaviourTreeInit(BehaviorTreeRunner btr)
	{
		if (IsGameCharacterDead) return;
		foreach (GameCharacter gc in CharacterDetection.DetectedTarget)
		{
			OnTargetDetectionOverlapEnter(gc);
		}
	}

	public override void ShowAttackFeedback()
	{
		
		if (!RigDataComponent.Bones.ContainsKey(GameCharacterData.HeadBoneName))
		{
			Ultra.Utilities.Instance.DebugErrorString("EnemyGameCharacter", "ShowAttackFeedback", "Head Bonename not Valid!");
			return;
		}
		var ps = attackFeedbackPool.GetValue();
		ps.transform.position = Vector3.zero;
		ps.transform.localPosition = Vector3.zero;
		ps.transform.parent = RigDataComponent.Bones[GameCharacterData.HeadBoneName];
		ps.transform.Translate(GameCharacterData.AttackFeedbackOffset, Space.World);
		//Instantiate(GameAssets.Instance.DefaultAttackFeedback, RigDataComponent.Bones[GameCharacterData.HeadBoneName]);
		//attackFeedback.transform.Translate(GameCharacterData.AttackFeedbackOffset, Space.World);

			if (ps == null)
				Ultra.Utilities.Instance.DebugErrorString("EnemyGameCharacter", "ShowAttackFeedback", "Particle System is null!");
		ps.Play();
	}

	protected override async void GameCharacterDied()
	{
		base.GameCharacterDied();
		await new WaitForSeconds(1f);
		desolveTimer.Start();
	}
}