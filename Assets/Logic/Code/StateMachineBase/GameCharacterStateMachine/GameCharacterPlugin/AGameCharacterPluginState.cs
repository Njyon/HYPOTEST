using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGameCharacterPluginState : IPluginState<EPluginCharacterState>
{
	GameCharacter gameCharacter;
	GameCharacterPluginStateMachine pluginStateMachine;
	public GameCharacter GameCharacter { get { return gameCharacter; } }
	public GameCharacterPluginStateMachine PluginStateMachine { get { return pluginStateMachine; } }
	bool isActive = false;

	public AGameCharacterPluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine)
	{
		this.gameCharacter = gameCharacter;
		this.pluginStateMachine = pluginStateMachine;
	}

	public virtual void Active()
	{
		isActive = true;
	}
	public virtual void Deactive()
	{
		isActive = false;
	}
	public virtual bool IsActive()
	{
		return isActive;
	}

	public abstract void AddState();
	public abstract void ExecuteState(float deltaTime);
	public abstract EPluginCharacterState GetStateType();
	public abstract void RemoveState();
	public abstract bool WantsToBeActive();
}
