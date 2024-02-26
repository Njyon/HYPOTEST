using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterAimIKCorrectionPluginState : AGameCharacterPluginState
{
	public GameCharacterAimIKCorrectionPluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base(gameCharacter, pluginStateMachine)
	{
	}

	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.AimIKCorrection;
	}

	public override void AddState()
	{
		GameCharacter.StateMachine.onStateChanged += OnGameCharacterStateChange;
		GameCharacter.PluginStateMachine.onPluginStateActivated += OnPluginStateActivated;
		GameCharacter.PluginStateMachine.onPluginStateDeactivated += OnPluginStateDeactivated;
		GameCharacter.MovementComponent.onCharacterFinishedJumping += OnFinishedJumping;
		GameCharacter.EventComponent.onCharacterEventTriggered += OnCharacterEvent;

		GameCharacter.AnimController.InterpChectCorrectionWeight(1);
		GameCharacter.AnimController.InterpFPFootIKLWeight(1);
		GameCharacter.AnimController.InterpFPFootIKRWeight(1);

		SetIKBasedOnCurrentState(GameCharacter.StateMachine.GetCurrentStateType());
	}

	public override void RemoveState()
	{
		GameCharacter.StateMachine.onStateChanged -= OnGameCharacterStateChange;
		GameCharacter.PluginStateMachine.onPluginStateActivated -= OnPluginStateActivated;
		GameCharacter.PluginStateMachine.onPluginStateDeactivated -= OnPluginStateDeactivated;
		GameCharacter.MovementComponent.onCharacterFinishedJumping -= OnFinishedJumping;
		GameCharacter.EventComponent.onCharacterEventTriggered -= OnCharacterEvent;

		GameCharacter.AnimController.SetChestCorrectionWeight(0);
		GameCharacter.AnimController.InterpFPFootIKLWeight(0, 10);
		GameCharacter.AnimController.InterpFPFootIKRWeight(0, 10);
	}

	public override bool WantsToBeActive()
	{
		return true;
	}

	public override void ExecuteState(float deltaTime)
	{
		
	}

	void OnGameCharacterStateChange(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		SetIKBasedOnCurrentState(newState.GetStateType());
	}

	void OnPluginStateActivated(EPluginCharacterState activatedStateType)
	{
		switch (activatedStateType)
		{

			case EPluginCharacterState.Shoot:
				GameCharacter.AnimController.SetChestCorrectionWeight(1);
				GameCharacter.AnimController.SetSpineLayerWeight(GameCharacter.CombatComponent.CurrentWeapon.AnimationData.AimData.HeadSpineLayerMovingWeight);
				GameCharacter.AnimController.SetHeadLayerWeight(GameCharacter.CombatComponent.CurrentWeapon.AnimationData.AimData.HeadSpineLayerMovingWeight);
				GameCharacter.AnimController.SetArmRLayerWeight(GameCharacter.CombatComponent.CurrentWeapon.AnimationData.AimData.ArmRMovingWeight);
				GameCharacter.AnimController.SetArmLLayerWeight(GameCharacter.CombatComponent.CurrentWeapon.AnimationData.AimData.ArmLMovingWeight);
				GameCharacter.AnimController.SetUpperBodyLayerWeight(1);
				break;
			default: break;
		}
	}

	void SetIKBasedOnCurrentState(EGameCharacterState currentState)
	{
		if (GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.Aim))
		{
			switch (currentState)
			{
				case EGameCharacterState.Attack:
				case EGameCharacterState.AttackRecovery:
				case EGameCharacterState.DefensiveAction:
				case EGameCharacterState.Dodge:
					GameCharacter.AnimController.SetChestCorrectionWeight(0);
					GameCharacter.AnimController.SetFPFootIKLWeight(0);
					GameCharacter.AnimController.SetFPFootIKRWeight(0);
					break;
				case EGameCharacterState.Sliding:
				case EGameCharacterState.Moving:
				case EGameCharacterState.InAir:
					GameCharacter.AnimController.InterpChectCorrectionWeight(1, 10);
					GameCharacter.AnimController.SetFPFootIKLWeight(0);
					GameCharacter.AnimController.SetFPFootIKRWeight(0);
					break;
				default:
					GameCharacter.AnimController.InterpChectCorrectionWeight(1);
					GameCharacter.AnimController.InterpFPFootIKLWeight(1, 10);
					GameCharacter.AnimController.InterpFPFootIKRWeight(1, 10);
					break;
			}
		}
		else
		{
			switch (currentState)
			{
				case EGameCharacterState.Attack:
				case EGameCharacterState.AttackRecovery:
				case EGameCharacterState.DefensiveAction:
				case EGameCharacterState.Dodge:
				case EGameCharacterState.Moving:
				case EGameCharacterState.InAir:
				case EGameCharacterState.Sliding:
					GameCharacter.AnimController.SetChestCorrectionWeight(0);
					GameCharacter.AnimController.SetFPFootIKLWeight(0);
					GameCharacter.AnimController.SetFPFootIKRWeight(0);
					break;
				default:
					GameCharacter.AnimController.InterpChectCorrectionWeight(1);
					GameCharacter.AnimController.InterpFPFootIKLWeight(1, 10);
					GameCharacter.AnimController.InterpFPFootIKRWeight(1, 10);
					break;
			}
		}
	}


	void OnPluginStateDeactivated(EPluginCharacterState deactivatedStateType)
	{
		switch (deactivatedStateType)
		{
			case EPluginCharacterState.Aim:
				SetIKBasedOnCurrentState(GameCharacter.StateMachine.GetCurrentStateType());
				break;
			case EPluginCharacterState.Shoot:
				break;
			default: break;
		}
	}

	void OnFinishedJumping()
	{
		SetIKBasedOnCurrentState(GameCharacter.StateMachine.GetCurrentStateType());
	}

	void OnCharacterEvent(EGameCharacterEvent type)
	{
		switch (type)
		{
			case EGameCharacterEvent.Jump:
				GameCharacter.AnimController.SetFPFootIKLWeight(0);
				GameCharacter.AnimController.SetFPFootIKRWeight(0);
				break;
			default:
				break;
		}
	}
}
