using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingBombDebuff : ABuff
{
	ParticleSystem particleSystem;
	ParticleSystemPool livingBombTriggerPool;
	float explosionStunTime;
	float kickAwayStrengh;

	public LivingBombDebuff(GameCharacter gameCharacter, float duration, ParticleSystem partilceSystem, ParticleSystemPool livingBombTriggerPool, float explosionStunTime, float kickAwayStrengh) : base(gameCharacter, duration)
	{
		this.particleSystem = partilceSystem;
		this.particleSystem.transform.position = gameCharacter.MovementComponent.CharacterCenter;
		this.particleSystem.transform.parent = gameCharacter.transform;
		this.livingBombTriggerPool = livingBombTriggerPool;
		this.explosionStunTime = explosionStunTime;
		this.kickAwayStrengh = kickAwayStrengh;

		GameCharacter.MovementComponent.onCharacterGroundedChanged += OnCharacterGroundedChanged;
	}

	void OnCharacterGroundedChanged(bool newState)
	{
		if (!newState) return;
		GameCharacter.MovementComponent.onCharacterGroundedChanged -= OnCharacterGroundedChanged;
		var ps = livingBombTriggerPool.GetValue();
		ps.transform.position = GameCharacter.MovementComponent.CharacterCenter;
		GameCharacter.CombatComponent.KickAway(GameCharacter, explosionStunTime, Vector3.up, kickAwayStrengh, true, false);
		GameCharacter.BuffComponent.AddBuff(new HoldInAirAfterStartFallingBuff(GameCharacter, -1f));
		particleSystem.Stop();
		GameCharacter.BuffComponent.RemoveBuff(GetBuffType());
	}

	public override void BuffEnds()
	{
		GameCharacter.MovementComponent.onCharacterGroundedChanged -= OnCharacterGroundedChanged;
	}

	public override EBuff GetBuffType()
	{
		return EBuff.LivingBomb;
	}
}
