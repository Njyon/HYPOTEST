using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAimBuff : ABuff
{
	public ForceAimBuff(GameCharacter gameCharacter, float duration) : base(gameCharacter, duration)
	{
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Aim);
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.LookAtAimTargetDirection);
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);

		// Check IsActive here becasue after Updating Parent class IsActive can be false
		if (IsActive)
		{
			if (!GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.Aim))
				GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Aim);
			if (!GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.LookAtAimTargetDirection))
				GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.LookAtAimTargetDirection);
		}
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
