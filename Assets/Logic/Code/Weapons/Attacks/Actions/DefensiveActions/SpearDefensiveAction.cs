using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpearDefensiveActionData : AttackData
{
	public AimBlendAnimations spearDefensiveAction;
	public AimBlendAnimations spearDefensiveActionPull;
	public GameObject throwSpear;
}
public class SpearDefensiveAction : ActionBase
{
	public SpearDefensiveActionData attackData;
	OldWeaponProjectile defensiveSpear;
	bool canLeaveState = false;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon);
		canLeaveState = false;
	}

	public override void StartAction()
	{
		if (attackData.spearDefensiveAction == null)
		{
			Ultra.Utilities.Instance.DebugErrorString("SpearDefensiveAction", "StartAction", "AnimationData was null!");
		}

		GameCharacter targetEnemy = Ultra.HypoUttilies.FindCharactereNearestToDirection(GameCharacter.MovementComponent.CharacterCenter, (GameCharacter.MovementInput.magnitude <= 0) ? GameCharacter.transform.forward : GameCharacter.MovementInput, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
		if (targetEnemy == null) return;

		GameCharacter.CombatComponent.DefensiveTimer.Start(attackData.spearDefensiveAction.midAnimation.length);
		GameCharacter.AnimController.ApplyBlendTree(attackData.spearDefensiveAction);
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Aim);
		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.DefensiveAction);
		Weapon.AttackAnimType = EAttackAnimType.AimBlendSpace;

		Weapon.SpawnedWeapon.SetActive(false);

		GameObject throwSpear = GameObject.Instantiate(attackData.throwSpear);
		Vector3 spearDir = targetEnemy.MovementComponent.CharacterCenter - throwSpear.transform.position;
		throwSpear.transform.position = new Vector3(Weapon.SpawnedWeaponBones.transform.position.x, Weapon.SpawnedWeaponBones.transform.position.y, 0);
		throwSpear.transform.rotation = Quaternion.LookRotation(spearDir.normalized, Vector3.up);
		throwSpear.transform.eulerAngles = new Vector3(throwSpear.transform.eulerAngles.x, throwSpear.transform.eulerAngles.y, 90f);

		GameCharacter.CombatComponent.AimCharacter = targetEnemy;
		Weapon.HookCharacterToCharacter(targetEnemy);
		GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;


		defensiveSpear = throwSpear.GetComponent<OldWeaponProjectile>();
		defensiveSpear.onProjectileHit += DefensiveActionHit;
		defensiveSpear.Initialize(GameCharacter, throwSpear.transform.position, targetEnemy);
	}
	void DefensiveActionHit(GameObject hitObj)
	{
		GameCharacter hitgameCharacter = hitObj.GetComponent<GameCharacter>();
		if (hitgameCharacter == null)
		{
			IDamage damageInterface = hitObj.GetComponent<IDamage>();
			if (damageInterface != null)
			{
				if (damageInterface.GetTeam() == GameCharacter.GetTeam()) return;
				damageInterface.DoDamage(GameCharacter, 0);
			}
			return;
		}

		if (hitgameCharacter.CombatComponent.CanRequestMoveTo())
		{
			GameCharacter.CombatComponent.AimCharacter = hitgameCharacter;
			Weapon.HookCharacterToCharacter(hitgameCharacter);
			GameCharacter.CombatComponent.HookedCharacters.Add(hitgameCharacter);
			hitgameCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			hitgameCharacter.CombatComponent.MoveToPosition = GameCharacter.transform.position + GameCharacter.transform.forward * 1f;

			hitgameCharacter.StateMachine.RequestStateChange(EGameCharacterState.MoveToPosition, true);

			GameCharacter.AnimController.ApplyBlendTree(attackData.spearDefensiveActionPull);
			GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Aim);
			GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.DefensiveAction);
			Weapon.AttackAnimType = EAttackAnimType.AimBlendSpace;
		}
		else
		{
			AfterDefensiveActionCleanUp();
		}
	}

	public override void CharacterArrivedAtRequestedLocation(GameCharacter movedCharacter)
	{
		AfterDefensiveActionCleanUp();
	}

	public override void CharacterMoveToAbort(GameCharacter movedCharacter)
	{
		AfterDefensiveActionCleanUp();
	}

	public override void CharacterMoveToEnd(GameCharacter movedCharacter)
	{
		AfterDefensiveActionCleanUp();
	}

	public override void DefensiveActionEnd()
	{
		base.DefensiveActionEnd();
		AfterDefensiveActionCleanUp();
	}

	private void AfterDefensiveActionCleanUp()
	{
		canLeaveState = true;
		GameCharacter.AnimController.InAimBlendTree = false;
		Weapon.UnHookAllHookedCharacerts();
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
		GameCharacter.RequestBestCharacterState();
		Weapon.SpawnedWeapon.SetActive(true);
		if (defensiveSpear != null)
			GameObject.Destroy(defensiveSpear.gameObject);
	}

	public override bool CanLeaveDefensiveState()
	{
		return canLeaveState;
	}

	public override void ActionInterupted()
	{
		base.ActionInterupted();
		AfterDefensiveActionCleanUp();
	}

	public override ActionBase CreateCopy()
	{
		SpearDefensiveAction copy = new SpearDefensiveAction();
		copy.attackData = attackData;
		return copy;
	}
}
