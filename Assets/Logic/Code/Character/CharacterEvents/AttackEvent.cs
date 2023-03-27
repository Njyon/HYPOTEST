using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEvent : CharacterEvent
{
	EAttackType attackType;
	public AttackEvent(GameCharacter gameCharacter, EAttackType attackType, float time = 0.1F) : base(gameCharacter, time)
	{
		this.attackType = attackType;
	}

	public override bool CanBeExecuted()
	{
		return gameCharacter?.StateMachine?.CurrentState?.UpdateState(0, EGameCharacterState.Attack) == EGameCharacterState.Attack;
	}

	public override EGameCharacterEvent GetGameCharacterEvenetType()
	{
		return EGameCharacterEvent.Attack;
	}

	public override void StartEvent()
	{
		gameCharacter?.CombatComponent?.Attack(attackType);
	}
}
