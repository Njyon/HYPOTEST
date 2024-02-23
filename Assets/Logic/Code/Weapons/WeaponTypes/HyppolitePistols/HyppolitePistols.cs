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

	public override void EquipWeapon()
	{
		base.EquipWeapon();

		shootParticlePool = new ParticleSystemPool(GameAssets.Instance.hyppolitePistolShootEffect, GameCharacter.CreateHolderChild("PistolFlashParticlePool"));
		hitParticlePool = new ParticleSystemPool(GameAssets.Instance.hyppolitePistolHitEffect, GameCharacter.CreateHolderChild("PistolHitParticlePool"));

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