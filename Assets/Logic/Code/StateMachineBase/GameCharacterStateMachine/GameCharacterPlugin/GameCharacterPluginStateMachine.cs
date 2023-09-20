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
		var stateTypes = Assembly.GetExecutingAssembly().GetTypes()
			.Where(t => t.BaseType == typeof(AGameCharacterPluginState))
			.ToList();

		var stateClass = stateTypes.FirstOrDefault(t => t.Name == "GameCharacter" + stateType.ToString() + "PluginState");

		if (stateClass == null)
		{
			newPluginState = null;
			return false;
		}

		newPluginState = Activator.CreateInstance(stateClass, gameCharacter, this) as AGameCharacterPluginState;

		return newPluginState != null;
	}
}
