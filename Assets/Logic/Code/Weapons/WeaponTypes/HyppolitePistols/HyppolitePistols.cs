using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyppolitePistols : WeaponBase
{
    public HyppolitePistols() { }
	public HyppolitePistols(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base(gameCharacter, weaponData)
	{ }

	~HyppolitePistols()
	{
		if (GameCharacter != null)
		{
			GameCharacter.StateMachine.onStateChanged -= OnGameCharacterStateChange;
			GameCharacter.PluginStateMachine.onPluginStateActivated -= OnPluginStateActivated;
			GameCharacter.PluginStateMachine.onPluginStateDeactivated -= OnPluginStateDeactivated;
			GameCharacter.MovementComponent.onCharacterFinishedJumping -= OnFinishedJumping;
			GameCharacter.EventComponent.onCharacterEventTriggered -= OnCharacterEvent;
		}
	}

	public override void EquipWeapon()
	{
		base.EquipWeapon();

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

	public override void UnEquipWeapon()
	{
		base.UnEquipWeapon();

		GameCharacter.StateMachine.onStateChanged -= OnGameCharacterStateChange;
		GameCharacter.PluginStateMachine.onPluginStateActivated -= OnPluginStateActivated;
		GameCharacter.PluginStateMachine.onPluginStateDeactivated -= OnPluginStateDeactivated;
		GameCharacter.MovementComponent.onCharacterFinishedJumping -= OnFinishedJumping;
		GameCharacter.EventComponent.onCharacterEventTriggered -= OnCharacterEvent;

		GameCharacter.BuffComponent.RemoveBuff(EBuff.ForceAim);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);

		GameCharacter.AnimController.SetChestCorrectionWeight(0);
		GameCharacter.AnimController.InterpFPFootIKLWeight(0, 10);
		GameCharacter.AnimController.InterpFPFootIKRWeight(0, 10);
	}

	void OnFinishedJumping()
	{
		SetIKBasedOnCurrentState(GameCharacter.StateMachine.GetCurrentStateType());
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

	void OnGameCharacterStateChange(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		SetIKBasedOnCurrentState(newState.GetStateType());
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

	void OnCharacterEvent(EGameCharacterEvent type)
	{
		switch(type)
		{
			case EGameCharacterEvent.Jump:
				GameCharacter.AnimController.SetFPFootIKLWeight(0);
				GameCharacter.AnimController.SetFPFootIKRWeight(0);
				break;
			default:
				break;
		}
	}

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new HyppolitePistols(gameCharacter, weapon);
	}
}