using Megumin.GameFramework.AI.BehaviorTree;
using Megumin.Reflection;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIControllerBase : ControllerBase
{
	GameCharacter gameCharacter;
	BehaviorTreeRunner btRunner;

	public override void BeginPosses(GameObject pawn, ScriptableCharacter characterData)
	{
		base.BeginPosses(pawn, characterData);

		SetupGameCharacter(pawn);
	}

	private void SetupGameCharacter(GameObject pawn)
	{
		gameCharacter = pawn.AddComponent<GameCharacter>();
		gameCharacter.CharacterData = characterData;
		gameCharacter.CustomAwake();
		GameCharacterMovementComponent movementComponent = pawn.GetComponent<GameCharacterMovementComponent>();
		if (movementComponent != null) movementComponent.SetupGameCharacter(gameCharacter);
		if (characterData != null && characterData.behaviourTree != null)
		{
			btRunner = gameCharacter.AddComponent<BehaviorTreeRunner>();
			InitBehaviourTreeValues();
			btRunner.onBehaviourTreeInit += InitCustomBehaviourTreeValues;
			btRunner.EnableTree();
		}
	}

	protected virtual void InitBehaviourTreeValues()
	{
		btRunner.RunOption = new Megumin.GameFramework.AI.RunOption();
		btRunner.InitOption = new Megumin.GameFramework.AI.InitOption();
		btRunner.BehaviorTreeAsset = characterData.behaviourTree;
	}

	protected virtual void InitCustomBehaviourTreeValues()
	{
		RefVar_GameCharacter selfVar = new RefVar_GameCharacter();
		selfVar.RefName = "Self";
		selfVar.Value = null;

		RefVar_GameCharacter targetRef = new RefVar_GameCharacter();
		targetRef.RefName = "Target";
		targetRef.Value = Ultra.HypoUttilies.GetGameMode().PlayerGameCharacter;

		if (!btRunner.BehaviourTree.Variable.Contains("Self"))
			btRunner.BehaviourTree.InitAddVariable(selfVar);
		if (!btRunner.BehaviourTree.Variable.Contains("Target"))
			btRunner.BehaviourTree.InitAddVariable(targetRef);

		//btRunner.BehaviourTree.ParseAllBindable(btRunner.BehaviourTree.Agent);
	}
}
