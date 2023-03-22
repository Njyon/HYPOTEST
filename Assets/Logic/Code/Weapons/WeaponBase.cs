using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase
{
    GameCharacter gameCharacter;
    ScriptableWeapon weaponData;
    GameObject spawnedWeapon;

    public GameCharacter GameCharacter { get { return gameCharacter; } }
    public ScriptableWeapon WeaponData { get { return weaponData; } }
    public GameObject SpawnedWeapon { get { return spawnedWeapon; } }

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
        switch(weaponData.AnimationData.HandType)
        {
            case EWeaponHandType.RightHand:
                if (weaponData.WeaponMeshData.WeaponMesh == null) break; 
                spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.GameCharacterData.HandROnjectPoint);
                spawnedWeapon.transform.Translate(weaponData.WeaponMeshData.WeaponOffset, Space.Self);
                spawnedWeapon.transform.rotation = Quaternion.Euler(spawnedWeapon.transform.rotation.eulerAngles + WeaponData.WeaponMeshData.WeaponRotationEuler);
                spawnedWeapon.transform.localScale = WeaponData.WeaponMeshData.WeaponScale;
                break;
			case EWeaponHandType.LeftHand:
				if (weaponData.WeaponMeshData.WeaponMesh == null) break; 
				spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.GameCharacterData.HandLOnjectPoint);
				spawnedWeapon.transform.Translate(weaponData.WeaponMeshData.WeaponOffset, Space.Self);
				spawnedWeapon.transform.rotation = Quaternion.Euler(spawnedWeapon.transform.rotation.eulerAngles + WeaponData.WeaponMeshData.WeaponRotationEuler);
				spawnedWeapon.transform.localScale = WeaponData.WeaponMeshData.WeaponScale;
				break;
			default:
                break;
        }
	}

	public virtual void UnEquipWeapon()
	{
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.WeaponReady);
        GameObject.Destroy(spawnedWeapon);
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
		gameCharacter.AnimController.SetBodyLayerAnimClip(weaponData.AnimationData.WeaponReadyPose);
	}
}
