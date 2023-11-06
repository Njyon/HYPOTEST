using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

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
		GameCharacter.AnimController.InAimBlendTree = false;
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
		if (GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.BlockAim)) return false;

		return true;
	}

	public override void ExecuteState(float deltaTime)
	{
		if (GameCharacter.CombatComponent.AimPositionCheck.Value == true)
		{
			AimAtPosition(GameCharacter.CombatComponent.AimPositionCheck.Position);
		}
		else if (GameCharacter.CombatComponent.AimCharacter != null)
		{
			AimAtPosition(GameCharacter.CombatComponent.AimCharacter.MovementComponent.CharacterCenter);
		}else
		{
			GameCharacter.AnimController.AimBlend = 0;
		}
	}

	private void AimAtPosition(Vector3 position)
	{
		float angle = Vector3.Angle(Vector3.down, (position - GameCharacter.MovementComponent.CharacterCenter).normalized);
		float aimValue = Ultra.Utilities.Remap(angle, 0, 180, 1, -1);
		GameCharacter.AnimController.AimBlend = aimValue;
	}
}