using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterIFramePluginState : AGameCharacterPluginState
{
	Ultra.Timer backUpTimer;
	public GameCharacterIFramePluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base (gameCharacter, pluginStateMachine)
	{
		backUpTimer = new Ultra.Timer();
		backUpTimer.onTimerFinished += OnBackupTimerFinished;
	}
	
	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.IFrame;
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
		backUpTimer.Start(5f);
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
		backUpTimer.Update(deltaTime);
	}

	async void OnBackupTimerFinished()
	{
		await new WaitForEndOfFrame();
		if (GameCharacter != null && GameCharacter.PluginStateMachine != null)
			GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.IFrame);
	}
}