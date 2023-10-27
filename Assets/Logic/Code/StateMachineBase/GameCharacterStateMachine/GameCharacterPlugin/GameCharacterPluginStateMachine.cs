using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public enum EPluginCharacterState
{
	WeaponReady,
	Aim,
	MovementOverride,
	LookInVelocityDirection,
	DefensiveActionHold,
	Parry,
	Block,
	IFrame,
}

public class GameCharacterPluginStateMachine : PluginStateMachineBase<EPluginCharacterState>
{
	GameCharacter gameCharacter;
	public GameCharacter GameCharacter { get { return gameCharacter; } }

	public void Init(GameCharacter gameCharacter)
	{
		this.gameCharacter = gameCharacter;
	}

	protected override bool CreatePluginState(EPluginCharacterState stateType, out IPluginState<EPluginCharacterState> newPluginState)
	{
		newPluginState = null;
		switch (stateType)
		{
			case EPluginCharacterState.WeaponReady: newPluginState = new GameCharacterWeaponReadyPluginState(this.gameCharacter, this); break;
			case EPluginCharacterState.Aim: newPluginState = new GameCharacterAimPluginState(this.gameCharacter, this); break;
			case EPluginCharacterState.MovementOverride: newPluginState = new GameCharacterMovementOverridePluginState(this.gameCharacter, this); break;
			case EPluginCharacterState.LookInVelocityDirection: newPluginState = new GameCharacterLookInVelocityDirectionPluginState(this.gameCharacter, this); break;
			case EPluginCharacterState.DefensiveActionHold: newPluginState = new GameCharacterDefensiveActionHoldPluginState(this.gameCharacter, this); break;
			case EPluginCharacterState.Parry: newPluginState = new GameCharacterParryPluginState(this.gameCharacter, this); break;
			case EPluginCharacterState.Block: newPluginState = new GameCharacterBlockPluginState(this.gameCharacter, this); break;
			case EPluginCharacterState.IFrame: newPluginState = new GameCharacterIFramePluginState(this.gameCharacter, this); break;
			default:
				Ultra.Utilities.Instance.DebugErrorString("GameCharacterPluginStateMaschine", "CreatePluginState", "PluginState has no Implementation!");
				break;
		}
		//var stateTypes = Assembly.GetExecutingAssembly().GetTypes()
		//	.Where(t => t.BaseType == typeof(AGameCharacterPluginState))
		//	.ToList();
		//
		//var stateClass = stateTypes.FirstOrDefault(t => t.Name == "GameCharacter" + stateType.ToString() + "PluginState");
		//
		//if (stateClass == null)
		//{
		//	newPluginState = null;
		//	return false;
		//}
		//
		//newPluginState = Activator.CreateInstance(stateClass, gameCharacter, this) as AGameCharacterPluginState;

		return newPluginState != null;
	}
}
