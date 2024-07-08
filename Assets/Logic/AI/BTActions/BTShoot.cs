using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Category("Action")]
public class BTShoot : BTHyppoliteActionNodeBase
{
	[Header("ShootData")]
	public int shootAmount = 1;
	public float timeBetweenShots = 1f;
	public EAttackType AttackType;
	public bool showFeedback = true;
	public bool canAttackInAir = false;

	bool attackStartet = false;
	bool cantAttack = false;
	int currentShotAmount = 0;
	Ultra.Timer timerBetweenShots = new Ultra.Timer();

	protected override void OnEnter(object options = null)
	{
		if (attackStartet) return;
		base.OnEnter(options);

		if (!CanAttackBeExecuted())
		{
			cantAttack = true;
			return;
		}

		if (GameCharacter.MovementComponent.IsGrounded || (canAttackInAir && !GameCharacter.MovementComponent.IsGrounded))
		{
			attackStartet = true;
			currentShotAmount = 0;
		}
		else
		{
			cantAttack = true;
		}
	}

	protected override Status OnTick(BTNode from, object options = null)
	{
		if (cantAttack && !attackStartet) return Status.Failed;
		if (GameCharacter == null || GameCharacter.IsGameCharacterDead) return Status.Failed;

		timerBetweenShots.Update(Time.deltaTime);

		if (attackStartet)
		{
			if (currentShotAmount < shootAmount)
			{
				if (timerBetweenShots.IsRunning) return Status.Running;
				currentShotAmount++;

				GameCharacter.CombatComponent.Attack(AttackType);
				if (showFeedback)
					GameCharacter.ShowAttackFeedback();

				timerBetweenShots.Start(timeBetweenShots);
			}
			else
			{
				return Status.Succeeded;
			}
			if (!GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.Shoot))
				return Status.Succeeded;
		}
		return Status.Running;
	}

	protected override void OnExit(Status result, object options = null)
	{
		base.OnExit(result, options);

		switch (result)
		{
			case Status.Succeeded:
			case Status.Failed:
				attackStartet = false;
				cantAttack = false;
				break;
			default: break;
		}
	}
}
