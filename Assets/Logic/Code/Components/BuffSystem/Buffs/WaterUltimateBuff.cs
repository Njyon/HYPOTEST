using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class WaterUltimateBuff : ABuff
{
	int waterParticlesToShoot;
	int totalWaterParticlesShoot;
	Ultra.Timer burstTimer;
	int burstAmount;
	GameObject lol;
	WaterDropPool waterDropPool;
	float waterDropSpeed;

	public WaterUltimateBuff(GameCharacter gameCharacter, float duration, WaterDropPool waterDropPool, float timeBetweenBursts, int burstAmount, float waterDropSpeed) : base(gameCharacter, duration)
	{
		if (GameCharacter != null) GameCharacter.CombatComponent.onAttack += OnAttack;
		waterParticlesToShoot = 0;
		totalWaterParticlesShoot = 0;
		this.burstAmount = burstAmount; 
		this.waterDropPool = waterDropPool;	
		this.waterDropSpeed = waterDropSpeed;
		burstTimer = new Ultra.Timer(timeBetweenBursts);
		burstTimer.onTimerFinished += OnBurstTimerFinished;
		burstTimer.Start();
	}

	void OnAttack(ref ShelfList<AttackAnimationData> lastAttacks)
	{
		waterParticlesToShoot += GameCharacter.CharacterDetection.DetectedGameCharacters.Count;
		if (burstTimer.IsFinished || burstTimer.IsPaused) burstTimer.Start();
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (burstTimer != null) burstTimer.Update(deltaTime);

	}

	void OnBurstTimerFinished()
	{
		BurstParticle();
		if (waterParticlesToShoot > 0) burstTimer.Start();
	}

	void BurstParticle()
	{
		int delta = (waterParticlesToShoot - burstAmount < 0) ? waterParticlesToShoot : burstAmount;
		waterParticlesToShoot -= delta;

		for (int i = 0; i < delta; i++) 
		{
			WaterDrop drop = waterDropPool.GetValue();
			drop.TrailRenderer.Clear();
			drop.TrailRenderer.enabled = false;

			drop.transform.position = GameCharacter.MovementComponent.CharacterCenter + new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(-1, 1));
			drop.Init(GameCharacter, GameCharacter.CharacterDetection.DetectedGameCharacters[i % GameCharacter.CharacterDetection.DetectedGameCharacters.Count], waterDropPool, waterDropSpeed);
			
			drop.TrailRenderer.enabled = true;
		}

	}

	public override void BuffEnds()
	{
		if (GameCharacter != null) GameCharacter.CombatComponent.onAttack -= OnAttack;
		if (burstTimer != null)
		{
			burstTimer.Stop();
			burstTimer.onTimerFinished -= OnBurstTimerFinished;
		}
	}

	public override EBuff GetBuffType()
	{
		return EBuff.WaterUltimate;
	}
}
