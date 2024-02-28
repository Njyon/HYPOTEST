using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmazonAxe : WeaponBase
{
	List<ParticleSystemPool> weaponVFXPools = new List<ParticleSystemPool>();

	public AmazonAxe() { }
	public AmazonAxe(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

	protected override void CreateWeaponVFXPools()
	{
		// Clean up if Create gets called multiple times
		foreach (var pool in weaponVFXPools)
		{
			GameObject.Destroy(pool.Parent);
		}
		weaponVFXPools.Clear();

		GameObject parent = GameCharacter.CreateHolderChild("AxeVFX_Pools");

		weaponVFXPools.Add(new ParticleSystemPool(WeaponData.UltimateParticleSystems[0], GameCharacter.CreateHolderChild("AxeUltimateSpearAttackParticlePool", parent)));
		weaponVFXPools.Add(new ParticleSystemPool(WeaponData.UltimateParticleSystems[1], GameCharacter.CreateHolderChild("AxeUltimateLivingBombParticlePool", parent)));
		weaponVFXPools.Add(new ParticleSystemPool(WeaponData.UltimateParticleSystems[2], GameCharacter.CreateHolderChild("AxeUltimateLivingBombExplosionParticlePool", parent)));
	}

	public override ParticleSystemPool GetWeaponVFXPool(int index)
	{
		if (weaponVFXPools.Count <= index) return null;
		return weaponVFXPools[index];
	}

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new AmazonAxe(gameCharacter, weapon);
	}
}