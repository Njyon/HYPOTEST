using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Action")]
public class BTAttack : BTHyppoliteActionNodeBase
{
	[Header("AttackData")]
	public EAttackType AttackType;

	bool attackDone = false;
	bool attackStartet = false;
	bool cantAttack = false;

	protected override void OnEnter(object options = null)
	{
		if (attackDone || attackStartet) return;
		base.OnEnter(options);

		if (GameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Attack)
		{
			cantAttack = true;
			return;
		}

		Vector3 dir = (TargetGameCharacter.transform.position - GameCharacter.transform.position).normalized;
		GameCharacter.HorizontalMovementInput(dir.x);
		GameCharacter.VerticalMovmentInput(dir.y);
		
		GameCharacter.CombatComponent.Attack(AttackType);
		attackStartet = true;
		GameCharacter.StateMachine.onStateChanged += OnStateChanged;
	}

	protected override Status OnTick(BTNode from, object options = null)
	{
		if (cantAttack && !attackStartet) return Status.Failed;
		if (attackStartet) 
			return  attackDone ? Status.Succeeded : Status.Running;
		return Status.Running;
	}

	protected override void OnExit(Status result, object options = null)
	{
		base.OnExit(result, options);

		GameCharacter.HorizontalMovementInput(0);
		GameCharacter.VerticalMovmentInput(0);

		switch (result) 
		{ 
			case Status.Succeeded:
			case Status.Failed:
				attackDone = false;
				attackStartet = false;
				cantAttack = false;

				GameCharacter.StateMachine.onStateChanged -= OnStateChanged;
				break;
			default: break;	
		}
	}

	void OnStateChanged(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		if (newState == null) return;
		if (newState.GetStateType() == EGameCharacterState.AttackRecovery)
		{
			attackDone = true;
		}
	}
}
