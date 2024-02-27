using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyppoliteShotgun : WeaponBase
{
	ParticleSystemPool shootParticlePool;
	ParticleSystemPool hitParticlePool;

	public HyppoliteShotgun() { }
	public HyppoliteShotgun(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

	public override void InitWeapon()
	{
		base.InitWeapon();

	}

	public override void EquipWeapon()
	{
		base.EquipWeapon();

		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.AimIKCorrection);
	}

	public override void UnEquipWeapon()
	{
		base.UnEquipWeapon();

		GameCharacter.BuffComponent.RemoveBuff(EBuff.ForceAim);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.AimIKCorrection);
	}

	protected override void CreateWeaponVFXPools()
	{
		shootParticlePool = new ParticleSystemPool(WeaponData.DefaultAttackVFX, GameCharacter.CreateHolderChild("ShotgunFlashParticlePool"));
		hitParticlePool = new ParticleSystemPool(WeaponData.DefaultHitVFX, GameCharacter.CreateHolderChild("ShotgunHitParticlePool"));
	}

	public override ParticleSystemPool GetRangeWeaponFlashParticlePool()
	{
		return shootParticlePool;
	}

	public override ParticleSystemPool GetRangeWeaponHitParticlePool()
	{
		return hitParticlePool;
	}

	public override bool SpawnRangedAttackShootPartilceAttached()
	{
		return false;
	}

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new HyppoliteShotgun(gameCharacter, weapon);
	}
}