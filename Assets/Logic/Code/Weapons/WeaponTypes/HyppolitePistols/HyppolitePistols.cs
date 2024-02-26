using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyppolitePistols : WeaponBase
{
	ParticleSystemPool shootParticlePool;
	ParticleSystemPool hitParticlePool;

	public HyppolitePistols() { }
	public HyppolitePistols(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base(gameCharacter, weaponData)
	{ }

	public override void InitWeapon()
	{
		base.InitWeapon();

		CreateWeaponVFXPools();
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

	async void CreateWeaponVFXPools()
	{
		await new WaitForEndOfFrame();
		await new WaitForEndOfFrame();

		shootParticlePool = new ParticleSystemPool(WeaponData.defaultAttackVFX, GameCharacter.CreateHolderChild("PistolFlashParticlePool"));
		hitParticlePool = new ParticleSystemPool(WeaponData.defaultHitVFX, GameCharacter.CreateHolderChild("PistolHitParticlePool"));
	}

	public override ParticleSystemPool GetRangeWeaponFlashParticlePool()
	{
		return shootParticlePool;
	}

	public override ParticleSystemPool GetRangeWeaponHitParticlePool()
	{
		return hitParticlePool;
	}

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new HyppolitePistols(gameCharacter, weapon);
	}
}