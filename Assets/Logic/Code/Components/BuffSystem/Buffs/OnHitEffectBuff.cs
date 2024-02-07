using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitEffectBuff : ABuff
{
	Vector3 moveHalfLenghtRange;
	float frequency;
	Vector3 currentShake;
	float speed = 5f;


	public OnHitEffectBuff(GameCharacter gameCharacter, float duration, Vector3 moveHalfLenghtRange, float frequency) : base(gameCharacter, duration)
	{ 
		this.moveHalfLenghtRange = moveHalfLenghtRange;
		this.frequency = frequency;
		if (!GameCharacter.IsGameCharacterDead)
			GameCharacter.Animator.enabled = false;
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);

		if (GameCharacter.IsGameCharacterDead)
		{
			GameCharacter.BuffComponent.RemoveBuff(EBuff.OnHitEffect);
			return;
		}
		Vector3 lastShake = currentShake;

		currentShake = ((Mathf.PerlinNoise(DurationTimer.CurrentTime * frequency, DurationTimer.CurrentTime * frequency) - 0.5f) * moveHalfLenghtRange.x * Vector3.right +
					(Mathf.PerlinNoise(DurationTimer.CurrentTime * frequency, DurationTimer.CurrentTime * frequency) - 0.5f) * moveHalfLenghtRange.y * Vector3.up);

		Vector3 shakeDelta = lastShake - currentShake;

		GameCharacter.RigDataComponent.Bones[GameCharacter.GameCharacterData.RootBoneName].Translate(shakeDelta, Space.Self);
	}

	public override void BuffEnds()
	{
		if (!GameCharacter.IsGameCharacterDead)
			GameCharacter.Animator.enabled = true;
		GameCharacter.RigDataComponent.Bones[GameCharacter.GameCharacterData.RootBoneName].localPosition = Vector3.zero;
	}

	public override EBuff GetBuffType()
	{
		return EBuff.OnHitEffect;
	}
}
