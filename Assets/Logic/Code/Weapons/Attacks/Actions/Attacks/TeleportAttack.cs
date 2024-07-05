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
		GameCharacter gc = GameCharacter.CharacterDetection.TargetGameCharacters[0];

		if (gc == null || GameCharacter.CheckForSameTeam(gc.Team)) return;

		Vector3 dir = gc.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter;
		dir = dir.IgnoreAxis(EAxis.YZ);
		Vector3 pos = new Vector3(gc.MovementComponent.CharacterCenter.x, GameCharacter.MovementComponent.CharacterCenter.y, GameCharacter.MovementComponent.CharacterCenter.z) + dir.normalized * (GameCharacter.GameCharacterData.MinCharacterDistance * 2);
		GameCharacter.MovementComponent.MovementVelocity = pos - GameCharacter.MovementComponent.CharacterCenter;
		GameCharacter.MovementComponent.IgnoreDeltaTime = true;

		//Ultra.Utilities.DrawArrow(gc.MovementComponent.CharacterCenter, dir.normalized, 5f, Color.cyan, 5f);
		//Ultra.Utilities.DrawWireSphere(pos, 1, Color.red, 5f);

		Vector3 rotDir = (gc.MovementComponent.CharacterCenter - pos).IgnoreAxis(EAxis.YZ).normalized;
		GameCharacter.RotateToDir(rotDir);
		GameCharacter.RotationTarget = GameCharacter.transform.rotation;
		//Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, rotDir, 3f, Color.red, 5f);
	}

	public override ActionBase CreateCopy()
	{
		TeleportAttack copy = new TeleportAttack();
		copy.attackData = attackData;
		return copy;
	}
}
