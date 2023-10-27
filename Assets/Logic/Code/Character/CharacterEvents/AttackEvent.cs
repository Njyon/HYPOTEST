using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEvent : CharacterEvent
{
	EAttackType attackType;
	float previousAttackTime;
	public AttackEvent(GameCharacter gameCharacter, EAttackType attackType, float previousAttackTime, float time = 0.2F) : base(gameCharacter, time)
	{
		this.attackType = attackType;
		this.previousAttackTime = previousAttackTime;
		//if (gameCharacter?.StateMachine?.GetCurrentStateType() == EGameCharacterState.Attack || gameCharacter?.StateMachine?.GetCurrentStateType() == EGameCharacterState.AttackRecovery) 
		//	this.time = 0.5f;

		//Ultra.Utilities.Instance.DebugLogOnScreen("AttackRequested!", 2f, StringColor.Orange, 100, DebugAreas.Combat);
	}

	public override bool CanBeExecuted()
	{
		if (gameCharacter.MovementComponent.IsInJump) return false;
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Attack) return false;
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Dodge) return false;
		return gameCharacter?.StateMachine?.CurrentState?.UpdateState(0, EGameCharacterState.Attack) == EGameCharacterState.Attack;
	}

	public override EGameCharacterEvent GetCharacterEvenetType()
	{
		return EGameCharacterEvent.Attack;
	}

	public override void StartEvent()
	{
		gameCharacter?.CombatComponent?.Attack(attackType, inputTime - previousAttackTime);
		//Ultra.Utilities.Instance.DebugLogOnScreen("AttackTriggered!", 2f, StringColor.Lightblue, 100, DebugAreas.Combat);
	}
}
