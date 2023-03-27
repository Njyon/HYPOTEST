using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EExplicitAttackType
{
    Unknown,
    GroundedDefaultAttack,
    GroundedDirectionalAttack,
    GroundedUpAttack,
    GroundedDownAttack,
    AirDefaultAttack,
    AirDirectionalAttack,
    AirUpAttack,
    AirDownAttack,
}

public abstract class WeaponBase
{
    GameCharacter gameCharacter;
    ScriptableWeapon weaponData;
    GameObject spawnedWeapon;
	EExplicitAttackType lastAttackType;
    int attackIndex;

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

    public virtual void GroundAttack()
	{
		BaseAttackLogic(EExplicitAttackType.GroundedDefaultAttack, ref WeaponData.AnimationData.GroundAttacks);
	}
	public virtual void GroundUpAttack()
	{
		BaseAttackLogic(EExplicitAttackType.GroundedUpAttack, ref WeaponData.AnimationData.GroundUpAttacks);
	}
    public virtual void GroundDownAttack()
    {
		BaseAttackLogic(EExplicitAttackType.GroundedDownAttack, ref WeaponData.AnimationData.GroundDownAttacks);
	}
    public virtual void GroundDirectionAttack()
    {
		BaseAttackLogic(EExplicitAttackType.GroundedDirectionalAttack, ref WeaponData.AnimationData.GroundDirectionAttacks);
	}

    public virtual void AirAttack()
	{
		BaseAttackLogic(EExplicitAttackType.AirDefaultAttack, ref WeaponData.AnimationData.AirAttacks);
	}
    public virtual void AirUpAttack()
	{
		BaseAttackLogic(EExplicitAttackType.AirUpAttack, ref WeaponData.AnimationData.AirUpAttacks);
	}
    public virtual void AirDownAttack()
	{
		BaseAttackLogic(EExplicitAttackType.AirDownAttack, ref WeaponData.AnimationData.AirDownAttacks);
	}
    public virtual void AirDirectionAttack()
	{
		BaseAttackLogic(EExplicitAttackType.AirDirectionalAttack, ref WeaponData.AnimationData.AirDirectionAttacks);
	}

	void SetUpWeaponAnimationData()
	{
		gameCharacter.AnimController.SetBodyLayerAnimClip(weaponData.AnimationData.WeaponReadyPose);
	}

	private void BaseAttackLogic(EExplicitAttackType explicitAttackType, ref List<AnimationClip> attackList)
	{
		if (lastAttackType != explicitAttackType)
		{
			attackIndex = 0;
			lastAttackType = explicitAttackType;
		}
		else
		{
			attackIndex++;
		}
		if (attackList == null || attackList.Count <= 0) return;
		attackIndex = attackIndex % attackList.Count;
		gameCharacter.AnimController.Attack(attackList[attackIndex]);
		gameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Attack);
		gameCharacter.CombatComponent.AttackTimer.Start(attackList[attackIndex].length);
	}
}
