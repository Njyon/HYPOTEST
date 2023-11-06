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
	public bool showFeedback = true;
	public bool canAttackInAir = false;

	bool attackDone = false;
	bool attackStartet = false;
	bool cantAttack = false;


	protected override void OnEnter(object options = null)
	{
		if (attackDone || attackStartet) return;
		base.OnEnter(options);

		if (!CanAttackBeExecuted())
		{
			cantAttack = true;
			return;
		}

		Vector3 dir = (TargetGameCharacter.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter).normalized;
		GameCharacter.HorizontalMovementInput(dir.x);
		GameCharacter.VerticalMovmentInput(dir.y);
		
		if (GameCharacter.MovementComponent.IsGrounded || (canAttackInAir && !GameCharacter.MovementComponent.IsGrounded))
		{
			GameCharacter.CombatComponent.Attack(AttackType);
			if (showFeedback)
				GameCharacter.ShowAttackFeedback();

			attackStartet = true;
		} else
		{
			cantAttack = true;
		}

		GameCharacter.StateMachine.onStateChanged += OnStateChanged;
	}

	protected override Status OnTick(BTNode from, object options = null)
	{
		if (attackDone) return Status.Succeeded;
		if (cantAttack && !attackStartet) return Status.Failed;
		if (GameCharacter == null || GameCharacter.IsGameCharacterDead) return Status.Failed;

		Vector3 dir = (TargetGameCharacter.MovementComponent.CharacterCenter  - GameCharacter.MovementComponent.CharacterCenter).normalized;
		GameCharacter.HorizontalMovementInput(dir.x);
		GameCharacter.VerticalMovmentInput(dir.y);

		if (attackStartet && !attackDone)
		{
			if (GameCharacter.StateMachine.CurrentState.GetStateType() != EGameCharacterState.Attack && GameCharacter.StateMachine.NewestStateChangeRequestState != EGameCharacterState.Attack)
				return Status.Failed;
		}
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
		if (oldState.GetStateType() == EGameCharacterState.Attack)
		{
			attackDone = true;
		}
	}


}
