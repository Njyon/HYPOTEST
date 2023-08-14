using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterAimPluginState : AGameCharacterPluginState
{
	public GameCharacterAimPluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base (gameCharacter, pluginStateMachine)
	{ }
	
	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.Aim;
	}

    public override void Active()
	{
		base.Active();
		GameCharacter.AnimController.InAimBlendTree = true;
		GameCharacter.AnimController.SetUpperBodyLayerWeight(1);

	}

	public override void Deactive()
	{
		base.Deactive();
		GameCharacter.AnimController.SetUpperBodyLayerWeight(0);

	}

	public override void AddState()
	{
	
	}
	
	public override void RemoveState()
	{
	
	}

	public override bool WantsToBeActive()
	{
		switch (GameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Freez:
				return false;
			default: break;
		}
		return true;
	}

	public override void ExecuteState(float deltaTime)
	{
		if (GameCharacter.CombatComponent.AimCharacter != null)
			GameCharacter.AnimController.AimBlend = Vector3.Dot(Vector3.down, (GameCharacter.CombatComponent.AimCharacter.transform.position - GameCharacter.transform.position).normalized);

	}
}