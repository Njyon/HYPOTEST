using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem.LowLevel;

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
		GameCharacter.CombatComponent.CurrentWeapon.SetWeaponReadyPoseBasedOnStates();
		GameCharacter.AnimController.SetUpperBodyLayerWeight(1);
		//GameCharacter.AnimController.InterpChectCorrectionWeight(1, 10);

		OnGameCharacterStateChange(GameCharacter.StateMachine.CurrentState, null);

	}

	public override void Deactive()
	{
		base.Deactive();
		GameCharacter.AnimController.InAimBlendTree = false;
		GameCharacter.AnimController.InterpUpperBodyLayerWeight(0);
		//GameCharacter.AnimController.InterpChectCorrectionWeight(0);

		GameCharacter.CombatComponent.CurrentWeapon.SetWeaponReadyPoseBasedOnStates();
	}

	public override void AddState()
	{
		GameCharacter.StateMachine.onStateChanged += OnGameCharacterStateChange;
		GameCharacter.EventComponent.onCharacterEventTriggered += CharacterEvent;
		GameCharacter.MovementComponent.onCharacterFinishedJumping += OnFinishedJumping;
	}
	
	public override void RemoveState()
	{
		GameCharacter.StateMachine.onStateChanged -= OnGameCharacterStateChange;
		GameCharacter.EventComponent.onCharacterEventTriggered -= CharacterEvent;
		GameCharacter.MovementComponent.onCharacterFinishedJumping -= OnFinishedJumping;
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
			AimAtPosition(GameCharacter.MovementComponent.CharacterCenter + GameCharacter.transform.forward * 9999f);
		}
	}

	private void AimAtPosition(Vector3 position)
	{
		float angle = Vector3.Angle(Vector3.down, (position - GameCharacter.MovementComponent.CharacterCenter).normalized);
		float aimValue = Ultra.Utilities.Remap(angle, 0, 180, 1, -1);
		GameCharacter.AnimController.AimBlend = aimValue;
	}

	void OnGameCharacterStateChange(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		if (IsActive())
			WeaponLayerBasedOnState(newState.GetStateType());
	}

	private void WeaponLayerBasedOnState(EGameCharacterState newState)
	{
		if (GameCharacter.CombatComponent.CurrentWeapon.AnimationData == null) return;
		switch (newState)
		{
			case EGameCharacterState.Attack:
			case EGameCharacterState.AttackRecovery:
			case EGameCharacterState.DefensiveAction:
			case EGameCharacterState.Dodge:
				GameCharacter.AnimController.InterpSpineLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.AnimationData.WeaponReadyInterpSpeed * 2);
				GameCharacter.AnimController.InterpLegLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.AnimationData.WeaponReadyInterpSpeed * 2);
				GameCharacter.AnimController.InterpHeadLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.AnimationData.WeaponReadyInterpSpeed * 2);
				GameCharacter.AnimController.InterpArmRLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.AnimationData.WeaponReadyInterpSpeed * 2);
				GameCharacter.AnimController.InterpArmLLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.AnimationData.WeaponReadyInterpSpeed * 2);
				break;
			case EGameCharacterState.Moving:
			case EGameCharacterState.InAir:
			case EGameCharacterState.Sliding:
				GameCharacter.AnimController.SetLegLayerWeight(0);
				//GameCharacter.AnimController.SetUpperBodyLayerWeight(0);
				GameCharacter.AnimController.SetUpperBodyLayerWeight(1);
				GameCharacter.AnimController.SetSpineLayerWeight(GameCharacter.CombatComponent.CurrentWeapon.AnimationData.AimData.HeadSpineLayerMovingWeight);
				GameCharacter.AnimController.SetHeadLayerWeight(GameCharacter.CombatComponent.CurrentWeapon.AnimationData.AimData.HeadSpineLayerMovingWeight);
				GameCharacter.AnimController.SetArmRLayerWeight(GameCharacter.CombatComponent.CurrentWeapon.AnimationData.AimData.ArmRMovingWeight);
				GameCharacter.AnimController.SetArmLLayerWeight(GameCharacter.CombatComponent.CurrentWeapon.AnimationData.AimData.ArmLMovingWeight);
				break;
			default:
				SetDefaultWeaponUpperBodyLayer();
				SetDefaultWeaponLowerBodyLayer();
				GameCharacter.AnimController.InterpUpperBodyLayerWeight(1, 10);
				break;
		}
		GameCharacter.AnimController.InAimBlendTree = true;
	}

	void SetDefaultWeaponUpperBodyLayer()
	{
		ScriptableWeaponAnimationData weaponAnimationData = GameCharacter.CombatComponent.CurrentWeapon.AnimationData;
		GameCharacter.AnimController.InterpHeadSpineArmWeight(weaponAnimationData.WeaponReadyWeight, weaponAnimationData.WeaponReadyInterpSpeed);
	}

	void SetDefaultWeaponLowerBodyLayer()
	{
		ScriptableWeaponAnimationData weaponAnimationData = GameCharacter.CombatComponent.CurrentWeapon.AnimationData;
		GameCharacter.AnimController.InterpLegLayerWeight(weaponAnimationData.WeaponReadyWeight, weaponAnimationData.WeaponReadyInterpSpeed);
	}

	void SetMovingWeaponUpperBodyLayer()
	{
		ScriptableWeaponAnimationData weaponAnimationData = GameCharacter.CombatComponent.CurrentWeapon.AnimationData;
		GameCharacter.AnimController.InterpSpineLayerWeight(weaponAnimationData.AimData.HeadSpineLayerMovingWeight, weaponAnimationData.AimData.WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpHeadLayerWeight(weaponAnimationData.AimData.HeadSpineLayerMovingWeight, weaponAnimationData.AimData.WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpArmRLayerWeight(weaponAnimationData.AimData.ArmRMovingWeight, weaponAnimationData.AimData.WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpArmLLayerWeight(weaponAnimationData.AimData.ArmLMovingWeight, weaponAnimationData.AimData.WeaponReadyInterpSpeed);
	}

	void CharacterEvent(EGameCharacterEvent type)
	{
		switch (type)
		{
			case EGameCharacterEvent.Jump:
				GameCharacter.AnimController.SetHeadSpineArmWeight(0);
				GameCharacter.AnimController.SetLegLayerWeight(0);
				break;
			default: break;
		}
	}

	void OnFinishedJumping()
	{
		if (IsActive())
			WeaponLayerBasedOnState(GameCharacter.StateMachine.GetCurrentStateType());
	}
}