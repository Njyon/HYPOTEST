using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterWeaponReadyPluginState : AGameCharacterPluginState
{
	public GameCharacterWeaponReadyPluginState(GameCharacter gameCharacter, GameCharacterPluginStateMachine pluginStateMachine) : base(gameCharacter, pluginStateMachine)
	{ }

	public override void Active()
	{
		base.Active();
		WeaponLayerBasedOnState(GameCharacter.StateMachine.GetCurrentStateType());
	}

	public override void AddState()
	{
		GameCharacter.StateMachine.onStateChanged += OnGameCharacterStateChange;
		GameCharacter.EventComponent.onCharacterEventTriggered += CharacterEvent;
	}

	public override void Deactive()
	{
		base.Deactive();
		if (!GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		GameCharacter.AnimController.InterpSpineLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpLegLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpHeadLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpArmRLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpArmLLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
	}

	public override void ExecuteState(float deltaTime)
	{

	}

	public override EPluginCharacterState GetStateType()
	{
		return EPluginCharacterState.WeaponReady;
	}

	public override void RemoveState()
	{
		GameCharacter.StateMachine.onStateChanged -= OnGameCharacterStateChange;
		GameCharacter.EventComponent.onCharacterEventTriggered -= CharacterEvent;
	}

	public override bool WantsToBeActive()
	{
		switch(GameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Freez:
			case EGameCharacterState.HookedToCharacter:
			case EGameCharacterState.PullCharacterOnHorizontalLevel:
			case EGameCharacterState.MoveToPosition:
			case EGameCharacterState.FlyAway: 
				return false;
			default:
				break;
		}
		return true;
	}

	void OnGameCharacterStateChange(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		if (IsActive())
			WeaponLayerBasedOnState(newState.GetStateType());
	}

	private void WeaponLayerBasedOnState(EGameCharacterState newState)
	{
		if (!GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		switch (newState)
		{
			case EGameCharacterState.Attack:
			case EGameCharacterState.AttackRecovery:
			case EGameCharacterState.DefensiveAction:
				GameCharacter.AnimController.InterpSpineLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed * 2);
				GameCharacter.AnimController.InterpLegLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed * 2);
				GameCharacter.AnimController.InterpHeadLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed * 2);
				GameCharacter.AnimController.InterpArmRLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed * 2);
				GameCharacter.AnimController.InterpArmLLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed * 2);
				break;
			case EGameCharacterState.InAir:
				GameCharacter.AnimController.InterpSpineLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
				GameCharacter.AnimController.InterpLegLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
				GameCharacter.AnimController.InterpHeadLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
				GameCharacter.AnimController.InterpArmRLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
				GameCharacter.AnimController.InterpArmLLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyInterpSpeed);
				break;
			case EGameCharacterState.Moving:
			case EGameCharacterState.Sliding:
				GameCharacter.AnimController.SetLegLayerWeight(0);
				SetMovingWeaponUpperBodyLayer();
				break;
			default:
				SetDefaultWeaponUpperBodyLayer();
				SetDefaultWeaponLowerBodyLayer(); 
				break;
		}
	}

	void SetDefaultWeaponUpperBodyLayer()
	{
		ScriptableWeaponAnimationData weaponAnimationData = GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name];
		GameCharacter.AnimController.InterpHeadSpineArmWeight(weaponAnimationData.WeaponReadyWeight, weaponAnimationData.WeaponReadyInterpSpeed);
	}

	void SetDefaultWeaponLowerBodyLayer()
	{
		ScriptableWeaponAnimationData weaponAnimationData = GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name];
		GameCharacter.AnimController.InterpLegLayerWeight(weaponAnimationData.WeaponReadyWeight, weaponAnimationData.WeaponReadyInterpSpeed);
	}

	void SetMovingWeaponUpperBodyLayer()
	{
		ScriptableWeaponAnimationData weaponAnimationData = GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name];
		GameCharacter.AnimController.InterpSpineLayerWeight(weaponAnimationData.HeadSpineLayerMovingWeight, weaponAnimationData.WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpHeadLayerWeight(weaponAnimationData.HeadSpineLayerMovingWeight, weaponAnimationData.WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpArmRLayerWeight(weaponAnimationData.ArmRMovingWeight, weaponAnimationData.WeaponReadyInterpSpeed);
		GameCharacter.AnimController.InterpArmLLayerWeight(weaponAnimationData.ArmLMovingWeight, weaponAnimationData.WeaponReadyInterpSpeed);
	}

	void CharacterEvent(EGameCharacterEvent type)
	{
		switch(type)
		{
			case EGameCharacterEvent.Jump: 
				GameCharacter.AnimController.SetHeadSpineArmWeight(0);
				GameCharacter.AnimController.SetLegLayerWeight(0);
				break;
			default: break;
		}
	}
}
