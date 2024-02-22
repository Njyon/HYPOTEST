using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldInAirAfterStartFallingBuff : ABuff
{
	DefaultGameModeData gameModeData;
	public HoldInAirAfterStartFallingBuff(GameCharacter gameCharacter, float duration) : base(gameCharacter, duration)
	{
		gameModeData = Ultra.HypoUttilies.GameMode.GetDefaultGameModeData();
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);

		if (GameCharacter.MovementComponent.MovementVelocity.y <= 0)
		{
			GameCharacter.MovementComponent.IgnoreGravityForTime(gameModeData.ignoreGravityAfterHit);
			GameCharacter.BuffComponent.RemoveBuff(GetBuffType());
		}
	}

	public override void BuffEnds()
	{

	}

	public override EBuff GetBuffType()
	{
		return EBuff.HoldInAirAfterStartFalling;
	}
}
