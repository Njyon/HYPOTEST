using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameCharacterNoGravityPluginState : AGameCharacterPluginState
{
	public GameCharacterNoGravityPluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base (gameCharacter, pluginStateMachine)
	{ }
	
	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.IgnoreGravityRuleState;
	}

    public override void Active()
	{
		base.Active();

		GameCharacter.MovementComponent.DONTUSEChangeIgnoreGravity(true);
	}

	public override void Deactive()
	{
		base.Deactive();

		GameCharacter.MovementComponent.DONTUSEChangeIgnoreGravity(false);
	}

	public override void AddState()
	{
		// Maybe in Active but idk
		GameCharacter.MovementComponent.onCharacterIgnoreGravityChanged += OnCharacterGroundedChanged;

	}
	
	public override void RemoveState()
	{
		// Maybe in Deactive but idk
		GameCharacter.MovementComponent.onCharacterIgnoreGravityChanged -= OnCharacterGroundedChanged;

	}

	public override bool WantsToBeActive()
	{
		if (GameCharacter.BuffComponent.IsBuffActive(EBuff.NoGravity)) return true;
		if (GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.Shoot)) return true;
		switch (GameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Attack:
			case EGameCharacterState.AttackRecovery:
			case EGameCharacterState.Freez:
			case EGameCharacterState.Dodge:
			case EGameCharacterState.DefensiveAction:
			case EGameCharacterState.PullCharacterOnHorizontalLevel:
			case EGameCharacterState.HookedToCharacter:
			case EGameCharacterState.MoveToPosition:
				return true;
			default:
				return false;
		}
	}

	public override void ExecuteState(float deltaTime)
	{
	
	}

	void OnCharacterGroundedChanged(bool newState)
	{
		// Maybe need a negative Flag for forcing Gravity On Maybe via Buff or PluginState
		if (!newState)
			GameCharacter.MovementComponent.DONTUSEChangeIgnoreGravity(WantsToBeActive());
	}
}