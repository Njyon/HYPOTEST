using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterBlockPluginState : AGameCharacterPluginState
{
	public GameCharacterBlockPluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base (gameCharacter, pluginStateMachine)
	{ }
	
	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.Block;
	}

    public override void Active()
	{
		base.Active();
	}

	public override void Deactive()
	{
		base.Deactive();
	}

	public override void AddState()
	{
	
	}
	
	public override void RemoveState()
	{
	
	}

	public override bool WantsToBeActive()
	{
		return true;
	}

	public override void ExecuteState(float deltaTime)
	{
	
	}
}