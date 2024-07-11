using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRainDebuff : ABuff
{
	ProjectilePool projectilePool;
	Ultra.Timer waveTimer;
	Ultra.Timer initialDelayTimer;
	int waveSize;
	float bulletToBulletDistance;
	float spawnHightOverCharacter;

	public BulletRainDebuff(GameCharacter gameCharacter, float duration, ProjectilePool projectilePool, float timeBetweenWaves, int wavesSize, float initialDelay, float bulletToBulletDistance, float spawnHightOverCharacter) : base(gameCharacter, duration)
	{
		this.projectilePool = projectilePool;
		waveTimer = new Ultra.Timer(timeBetweenWaves);
		initialDelayTimer = new Ultra.Timer(initialDelay, false);
		waveSize = wavesSize;
		this.bulletToBulletDistance = bulletToBulletDistance;
		this.spawnHightOverCharacter = spawnHightOverCharacter;

		initialDelayTimer.onTimerFinished += OnInitialDelayTimerFinished;
		waveTimer.onTimerFinished += OnWaveTimerFinished;
		waveTimer.onTimerStarted += OnWaveTimerStarted;
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);

		if (waveTimer.IsRunning) waveTimer.Update(Time.deltaTime);
		if (initialDelayTimer.IsRunning) initialDelayTimer.Update(Time.deltaTime);
	}

	public override void BuffEnds()
	{
		initialDelayTimer.Stop();
		waveTimer.Stop();

		initialDelayTimer.onTimerFinished -= OnInitialDelayTimerFinished;
		waveTimer.onTimerFinished -= OnInitialDelayTimerFinished;
		waveTimer.onTimerStarted -= OnWaveTimerStarted;

	}

	void OnWaveTimerStarted(float lenght)
	{
		Vector3 characterPos = GameCharacter.MovementComponent.CharacterCenter;		// Character Pos
		Vector3 startPos = characterPos + Vector3.up * spawnHightOverCharacter;		// add hight
		float xDistance = ((waveSize / 2) * bulletToBulletDistance);				// get half range
		startPos.x -= xDistance;													// add half range on Middle

		for (int i = 0; i < waveSize; i++)
		{
			WeaponProjectile projectile = projectilePool.GetValue();
			projectile.transform.position = startPos;
			startPos.x += bulletToBulletDistance;									// add range on position
		}
	}

	void OnWaveTimerFinished()
	{
		waveTimer.Start();
	}

	void OnInitialDelayTimerFinished()
	{
		waveTimer.Start();
	}

	public override EBuff GetBuffType()
	{
		return EBuff.BulletRain;
	}
}
