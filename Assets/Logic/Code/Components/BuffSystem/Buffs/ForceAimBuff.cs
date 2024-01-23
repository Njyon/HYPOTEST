using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAimBuff : ABuff
{
	public ForceAimBuff(GameCharacter gameCharacter, float duration) : base(gameCharacter, duration)
	{
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Aim);
		if (GameCharacter.CombatComponent.AimCharacter != null || GameCharacter.CombatComponent.AimPositionCheck != null) GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.LookAtAimTargetDirection);
	}

	public override void BuffEnds()
	{
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.LookAtAimTargetDirection);
	}

	public override EBuff GetBuffType()
	{
		return EBuff.ForceAim;
	}
}
