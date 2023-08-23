using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterMovementOverridePluginState : AGameCharacterPluginState
{
	Ultra.Timer timer;
	public GameCharacterMovementOverridePluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base (gameCharacter, pluginStateMachine)
	{ }
	
	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.MovementOverride;
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
		timer = new Ultra.Timer();
		timer.Start(GameCharacter.MovementComponent.MovementOverrideTime);
		timer.onTimerFinished += OnTimerFinished;
		GameCharacter.MovementComponent.UseGravity = false;
	}
	
	public override void RemoveState()
	{
		timer.onTimerFinished -= OnTimerFinished;
		GameCharacter.MovementComponent.UseGravity = true;
	}

	public override bool WantsToBeActive()
	{
		return true;
	}

	public override void ExecuteState(float deltaTime)
	{
		timer.Update(deltaTime);
		GameCharacter.MovementComponent.MovementOverride = Vector3.zero;
	}

	async void OnTimerFinished()
	{
		await new WaitForEndOfFrame();
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.MovementOverride);
	}
}