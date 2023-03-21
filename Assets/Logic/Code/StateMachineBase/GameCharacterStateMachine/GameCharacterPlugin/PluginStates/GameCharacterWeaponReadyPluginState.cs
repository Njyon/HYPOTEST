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
		ScriptableWeaponAnimationData weaponAnimationData = GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData;
		GameCharacter.AnimController.InterpUpperBodyLayerWeight(weaponAnimationData.WeaponReadyWeight, weaponAnimationData.WeaponReadyInterpSpeed);
	}

	public override void AddState()
	{

	}

	public override void Deactive()
	{
		base.Deactive();
		GameCharacter.AnimController.InterpUpperBodyLayerWeight(0, GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData.WeaponReadyInterpSpeed);
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

	}

	public override bool WantsToBeActive()
	{
		return true;
	}
}
