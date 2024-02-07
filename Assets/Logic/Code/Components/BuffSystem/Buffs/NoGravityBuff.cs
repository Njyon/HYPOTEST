using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGravityBuff : ABuff
{
	public NoGravityBuff(GameCharacter gameCharacter, float duration) : base(gameCharacter, duration)
	{
		Vector3 vel = GameCharacter.MovementComponent.MovementVelocity;
		GameCharacter.MovementComponent.UseGravity = false;
		GameCharacter.MovementComponent.MovementVelocity = new Vector3(vel.x, 0, vel.z);
	}

	public override void BuffEnds()
	{
		GameCharacter.MovementComponent.UseGravity = true;
	}

	public override EBuff GetBuffType()
	{
		return EBuff.NoGravity;
	}
}
