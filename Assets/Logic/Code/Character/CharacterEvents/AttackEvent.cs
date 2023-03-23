using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEvent : CharacterEvent
{
	public AttackEvent(GameCharacter gameCharacter, float time = 0.2F) : base(gameCharacter, time)
	{ }

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
		gameCharacter?.StateMachine?.RequestStateChange(EGameCharacterState.Attack);
	}
}
