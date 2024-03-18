using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HyppoliteAxeUltimateData : AttackData
{
	public AnimationClip attackAnimation;
	[Tooltip("Use this Value to Sync VFX and the Damage")]
	public float damageDelay = 1f;
	public float kickUpStrenght = 10f;
	public float stunLenght = 1f;
	[Tooltip("This Value is used to Divid the distance from the user to the target, to get a time value so the spears dont spawn at the same time")]
	public float spearSpawnDivisor = 10f;
	[Header("LivingBombData")]
	public float livingBombKickUpStrenght = 10;
	public float livingBombStunLenght = 1f;
}

public class HyppoliteAxeUltimate : AttackBase
{
	public HyppoliteAxeUltimateData attackData;
	ParticleSystemPool ultimateVFXPool;
	ParticleSystemPool livingBombVFXPool;
	ParticleSystemPool livingBombExplosionVFXPool;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, () => {
			ultimateVFXPool = Weapon.GetWeaponVFXPool(0);
			livingBombVFXPool = Weapon.GetWeaponVFXPool(1);
			livingBombExplosionVFXPool = Weapon.GetWeaponVFXPool(2);
		});

	}

	public override void StartAction()
	{
		StartAttack(attackData.attackAnimation);
	}

	public override void TriggerAnimationEvent()
	{
		var gcArray = GameCharacter.CharacterDetection.TargetGameCharacters.ToArray();
		Array.Sort(gcArray, CompareDistanceToReference);


		for (int i = 0; i < GameCharacter.CharacterDetection.TargetGameCharacters.Count; i++)
		{
			GameCharacter gc = GameCharacter.CharacterDetection.TargetGameCharacters[i];
			if (gc.CheckForSameTeam(GameCharacter.GetTeam()) || gc.IsGameCharacterDead) continue;
			SpawnSpearUnderGameCharacterAfterTimeBasedOnDistance(gc);
		}
	}

	async void SpawnSpearUnderGameCharacterAfterTimeBasedOnDistance(GameCharacter gc)
	{
		float t = Vector3.Distance(gc.transform.position, GameCharacter.transform.position);
		t = t / attackData.spearSpawnDivisor;
		await new WaitForSeconds(t);

		ParticleSystem ps = ultimateVFXPool.GetValue();
		ps.transform.position = gc.transform.position;

		KickEnemy(gc);
	}

	async void KickEnemy(GameCharacter gc)
	{
		await new WaitForSeconds(attackData.damageDelay);

		gc.DoDamage(GameCharacter, attackData.Damage, true, true, false);
		GameCharacter.CombatComponent.KickAway(gc, attackData.stunLenght, Vector3.up, attackData.kickUpStrenght, true, false);
		gc.BuffComponent.AddBuff(new HoldInAirAfterStartFallingBuff(gc, -1f));
		gc.BuffComponent.AddBuff(new LivingBombDebuff(gc, -1, livingBombVFXPool.GetValue(), livingBombExplosionVFXPool, attackData.livingBombStunLenght, attackData.livingBombKickUpStrenght));
	}


	int CompareDistanceToReference(GameCharacter a, GameCharacter b)
	{
		float distanceToA = Vector3.Distance(GameCharacter.transform.position, a.transform.position);
		float distanceToB = Vector3.Distance(GameCharacter.transform.position, b.transform.position);

		if (distanceToA < distanceToB)
			return -1;
		else if (distanceToA > distanceToB)
			return 1;
		else
			return 0;
	}

	public override ActionBase CreateCopy()
	{
		HyppoliteAxeUltimate copy = new HyppoliteAxeUltimate();
		copy.attackData = attackData;
		return copy;
	}
}
