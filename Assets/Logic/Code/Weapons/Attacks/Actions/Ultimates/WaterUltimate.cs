using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaterUltimateData : AttackData
{
	public AnimationClip attackAnimation;
	public WaterUltimateHelperScript wavePrefab;
	public float waveSpawnDistanceFromCharacter = 2f;
	public float waveKnockupRange = 5f;
	public float waveSpeed = 5f;
	public float waveLifeTime = 5f;
	public WaterDrop waterDropPrefab;
	public float WaterUltBuffDuration = 10f;
	public float WaterUltTimeBetweenBursts = 0.2f;
	public int BurstAmount = 3;
	public float WaterDropSpeed = 5f;
}

public class WaterUltimate : AttackBase
{
	public WaterUltimateData attackData;
	WaterUltimateHelperScript rightWave;
	WaterUltimateHelperScript leftWave;
	WaterDropPool waterDropPool;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		if (!IsActionInit)
		{
			waterDropPool = new WaterDropPool(attackData.waterDropPrefab, gameCharacter.CreateHolderChild("WaterDropPool"), 10);
		}
		base.Init(gameCharacter, weapon, action);

	}

	public override void StartAction()
	{
		if (rightWave != null) rightWave.gameCharacterDetection.onOverlapEnter -= OnOverlapEnter;
		if (leftWave != null) leftWave.gameCharacterDetection.onOverlapEnter -= OnOverlapEnter;

		Vector3 characterWorldPos = GameCharacter.MovementComponent.CharacterCenter + Vector3.up * attackData.waveSpawnDistanceFromCharacter;

		Vector3 rightWavePos = characterWorldPos + Vector3.right * attackData.waveSpawnDistanceFromCharacter;
		Vector3 leftWavePos = characterWorldPos + Vector3.left * attackData.waveSpawnDistanceFromCharacter;

		rightWavePos = GetWaveSpawnPoint(rightWavePos);
		leftWavePos = GetWaveSpawnPoint(leftWavePos);

		rightWave = SpawnWave(rightWavePos, Vector3.right);
		leftWave = SpawnWave(leftWavePos, Vector3.left);

		GameCharacter.BuffComponent.AddBuff(new WaterUltimateBuff(GameCharacter, attackData.WaterUltBuffDuration, waterDropPool, attackData.WaterUltTimeBetweenBursts, attackData.BurstAmount, attackData.WaterDropSpeed));

	}

	Vector3 GetWaveSpawnPoint(Vector3 worldPos)
	{
		Vector3 result = worldPos;
		RaycastHit hit;
		if (Physics.Raycast(worldPos, Vector3.down, out hit, 9999f, -5, QueryTriggerInteraction.Ignore))
		{
			result = new Vector3(hit.point.x, hit.point.y + attackData.wavePrefab.characterController.height / 2, hit.point.z);
		}

		return result;
	}

	WaterUltimateHelperScript SpawnWave(Vector3 spawnPosition, Vector3 dir)
	{
		WaterUltimateHelperScript wave = GameObject.Instantiate(attackData.wavePrefab, GameCharacter.DataWorldHolder.transform, true);
		wave.name = ">> " + wave.name;
		wave.transform.rotation = Quaternion.LookRotation(dir);
		wave.transform.position = spawnPosition;
		wave.gameCharacterDetection.onOverlapEnter += OnOverlapEnter;
		wave.Init(GameCharacter, attackData.waveSpeed, attackData.waveLifeTime);

		return wave;
	}

	void OnOverlapEnter(IDamage other)
	{
		if (other.IsGameCharacter())
			if (other.GetGameCharacter() == GameCharacter || other.GetGameCharacter().Team == GameCharacter.Team) return;

		other.DoDamage(GameCharacter, attackData.Damage, false);

		if (other.IsGameCharacter())
		{
			GameCharacter gc = other.GetGameCharacter();
			gc.CombatComponent.RequestMoveTo(GameCharacter, gc.MovementComponent.CharacterCenter + Vector3.up * attackData.waveKnockupRange);
		}
	}

	public override void ActionInterupted()
	{
		
	}

	public override ActionBase CreateCopy()
	{
		WaterUltimate copy = new WaterUltimate();
		copy.attackData = attackData;
		return copy;
	}
}
