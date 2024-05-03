using Megumin.GameFramework.AI.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeleportAttackData : AttackData
{ 
	public AnimationClip attackAnim;
}

public class TeleportAttack : AttackBase
{
	public TeleportAttackData attackData;

	public override void StartAction()
	{
		StartAttack(attackData.attackAnim);
	}

	public override void TriggerAnimationEvent()
	{ 
		foreach (GameCharacter gc in GameCharacter.CharacterDetection.TargetGameCharacters)
		{
			if (GameCharacter.CheckForSameTeam(gc.Team)) continue;
			Vector3 dir = gc.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter;
			dir = dir.IgnoreAxis(EAxis.YZ);
			Vector3 pos = gc.MovementComponent.CharacterCenter + dir.normalized * GameCharacter.GameCharacterData.MinCharacterDistance;
			GameCharacter.MovementComponent.MovementVelocity = GameCharacter.MovementComponent.CharacterCenter - pos;
			GameCharacter.MovementComponent.IgnoreDeltaTime = true;

			GameCharacter.RotateToDir((pos - gc.MovementComponent.CharacterCenter).normalized);
			break;
		}		
	}

	public override ActionBase CreateCopy()
	{
		TeleportAttack copy = new TeleportAttack();
		copy.attackData = attackData;
		return copy;
	}
}
