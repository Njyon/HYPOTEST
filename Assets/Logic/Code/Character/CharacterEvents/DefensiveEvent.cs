using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveEvent : CharacterEvent
{
	public DefensiveEvent(GameCharacter gameCharacter, float time = 0.2f) : base(gameCharacter, time)
	{

	}

	public override bool CanBeExecuted()
	{
		if (gameCharacter.MovementComponent.IsInJump) return false;
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Attack) return false;
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.DefensiveAction) return false;
		return gameCharacter?.StateMachine?.CurrentState?.UpdateState(0, EGameCharacterState.DefensiveAction) == EGameCharacterState.DefensiveAction;
	}

	public override EGameCharacterEvent GetGameCharacterEvenetType()
	{
		return EGameCharacterEvent.DefensiveAction;
	}

	public override void StartEvent()
	{
		gameCharacter?.CombatComponent?.DefensiveAction();
	}
}
