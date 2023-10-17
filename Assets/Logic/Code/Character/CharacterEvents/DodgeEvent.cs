using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeEvent : CharacterEvent
{
	public DodgeEvent(GameCharacter gameCharacter, float time = 0.2F) : base(gameCharacter, time)
	{ }

	public override bool CanBeExecuted()
	{
		if (gameCharacter.MovementComponent.IsInJump) return false;
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Attack) return false;
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.DefensiveAction) return false;
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Dodge) return false;
		return gameCharacter?.StateMachine?.CurrentState?.UpdateState(0, EGameCharacterState.Dodge) == EGameCharacterState.Dodge;
	}

	public override EGameCharacterEvent GetCharacterEvenetType()
	{
		return EGameCharacterEvent.Dodge;
	}

	public override void StartEvent()
	{
		gameCharacter.CombatComponent.Dodge();
	}
}
