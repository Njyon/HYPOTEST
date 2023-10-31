using Megumin.GameFramework.AI.BehaviorTree;
using Megumin.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;

[Icon("d_UnityEditor.GameView@2x")]
public class AIControllerBase : ControllerBase
{
	EnemyGameCharacter gameCharacter;
	BehaviorTreeRunner btRunner;

	EnemyInfo enemyInfo;

	public override void BeginPosses(GameObject pawn, ScriptableCharacter characterData)
	{
		base.BeginPosses(pawn, characterData);

		SetupGameCharacter(pawn);
	}

	async private void SetupGameCharacter(GameObject pawn)
	{
		Profiler.BeginSample("Init new GameCharacter");
		gameCharacter = pawn.AddComponent<EnemyGameCharacter>();
		LoadingChecker.Instance.Tasks.Add(Task.Run(async () => {
			while (!gameCharacter.IsInitialized)
			{
				await Task.Yield();
			}
		}));
		gameCharacter.CharacterData = characterData;
		if (characterData != null && characterData.behaviourTree != null)
		{
			btRunner = AIManager.Instance.GetBehaviorTreeRunner(gameCharacter, characterData, OnBehaviourTreeInit);

			//btRunner = gameCharacter.AddComponent<BehaviorTreeRunner>();
			//btRunner.onBehaviourTreeInit += OnBehaviourTreeInit;
			//InitBehaviourTreeValues();
		}
		gameCharacter.BTRunner = btRunner;
		gameCharacter.CustomAwake();
		GameCharacterMovementComponent movementComponent = pawn.GetComponent<GameCharacterMovementComponent>();
		if (movementComponent != null) movementComponent.SetupGameCharacter(gameCharacter);
		
		//if (btRunner != null) btRunner.EnableTree();

		gameCharacter.onGameCharacterDied += OnGameCharacterDied;
		gameCharacter.Team = HyppoliteTeam.TeamEnemy;

		Profiler.EndSample();

		await new WaitUntil(() => UIManager.Instance.Canvas != null);
		enemyInfo = UIManager.Instance.GetEnemyInfo(gameCharacter);
	}

	protected void OnDestroy()
	{
		if (btRunner != null) btRunner.onBehaviourTreeInit -= OnBehaviourTreeInit;
		if (gameCharacter != null) gameCharacter.onGameCharacterDied -= OnGameCharacterDied;

	}

	protected override void OnGameCharacterDied(GameCharacter gameCharacter)
	{
		if (btRunner != null)
		{
			AIManager.Instance.ReturnBehaviorTreeRunner(btRunner);
		}

		UIManager.Instance.ReturnEnemyInfo(enemyInfo);
	}

	protected virtual void InitBehaviourTreeValues()
	{
		btRunner.RunOption = new Megumin.GameFramework.AI.RunOption();
		btRunner.InitOption = new Megumin.GameFramework.AI.InitOption();
		btRunner.BehaviorTreeAsset = characterData.behaviourTree;
	}

	protected virtual void OnBehaviourTreeInit(BehaviorTreeRunner btr)
	{
		InitCustomBehaviourTreeValues();
		AIManager.Instance.AddManagableAI(new HyppoliteManagableAI(gameCharacter, btRunner));
		btr.ReStart();
	}

	protected virtual void InitCustomBehaviourTreeValues()
	{
		if (!btRunner.BehaviourTree.Variable.Contains("Self"))
		{
			RefVar_GameCharacter selfVar = new RefVar_GameCharacter();
			selfVar.RefName = "Self";
			selfVar.Value = null;

			btRunner.BehaviourTree.InitAddVariable(selfVar);
		}

		btRunner.BehaviourTree.Variable.TrySetValue<GameCharacter>("Self", gameCharacter);

		if (!btRunner.BehaviourTree.Variable.Contains("Target"))
		{
			RefVar_GameCharacter targetRef = new RefVar_GameCharacter();
			targetRef.RefName = "Target";
			targetRef.Value = null;

			btRunner.BehaviourTree.InitAddVariable(targetRef);
		}
	}
}
