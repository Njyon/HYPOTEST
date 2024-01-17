using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAimBuff : ABuff
{
	public ForceAimBuff(GameCharacter gameCharacter, float duration) : base(gameCharacter, duration)
	{
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Aim);
	}

	public override void BuffEnds()
	{
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
	}

	public override EBuff GetBuffType()
	{
		return EBuff.ForceAim;
	}
}
