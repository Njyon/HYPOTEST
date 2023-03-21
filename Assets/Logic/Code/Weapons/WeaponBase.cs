using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase
{
    GameCharacter gameCharacter;
    ScriptableWeapon weaponData;

    public GameCharacter GameCharacter { get { return gameCharacter; } }
    public ScriptableWeapon WeaponData { get { return weaponData; } }

    public WeaponBase() { }
    public WeaponBase(GameCharacter gameCharacter, ScriptableWeapon weaponData)
    {
        this.gameCharacter = gameCharacter;
        this.weaponData = weaponData;
    }

    public virtual void EquipWeapon()
	{
		SetUpWeaponAnimationData();
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.WeaponReady);
	}

	public virtual void UnEquipWeapon()
	{
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.WeaponReady);
	}

    public abstract void UpdateWeapon(float deltaTime);

    public abstract void GroundAttack();
    public abstract void GroundUpAttack();
    public abstract void GroundDownAttack();
    public abstract void GroundDirectionAttack();

    public abstract void AirAttack();
    public abstract void AirUpAttack();
    public abstract void AirDownAttack();
    public abstract void AirDirectionAttack();

	void SetUpWeaponAnimationData()
	{
		gameCharacter.AnimController.SetUpperBodyAnimClip(weaponData.AnimationData.WeaponReadyPose);
		gameCharacter.AnimController.SetLowerBodyAnimClip(weaponData.AnimationData.WeaponReadyPose);
	}
}
