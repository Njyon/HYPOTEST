using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CombatComponent;

[Serializable]
public class TeleportDefensiveActionData : AttackData
{
	public AnimationClip defensiveAction;
	public float teleportCharacterDistance = 0.5f;
	public float teleportRange = 10f;
	public float teleportAimTrsehholdHalfAngle = 45f;

}
public class TeleportDefensiveAction : ActionBase
{
	public TeleportDefensiveActionData attackData;
	bool canTeleport = true;
	Vector3 defensiveMoveInput;
	Vector3 defensiveMovePostion;
	bool defensiveShouldMove = false;
	GameCharacter targetTeleportCharacter;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction initAction = null)
	{
		base.Init(gameCharacter, weapon, () => {
			gameCharacter.MovementComponent.onCharacterGroundReset += GroundReset;
			gameCharacter.CombatComponent.onWeaponChanged += OnWeaponChanged;
		});
	}

	public override void StartAction()
	{
		if (canTeleport)
		{
			GameCharacter.CombatComponent.DefensiveTimer.Start(attackData.defensiveAction.length);
			GameCharacter.AnimController.SetDefensiveAction(attackData.defensiveAction);
			Weapon.AttackAnimType = EAttackAnimType.Default;
			GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.DefensiveAction);

			//defensiveMovePostion = GameCharacter.MovementInput;
			canTeleport = false;

		}
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		base.PostAttackStateLogic(deltaTime);
		if (defensiveShouldMove)
		{
			RotateCharacterToTeleportLocationOrEnemy();

			//float deltaTimeScale = 1f / Time.deltaTime;
			Vector3 dir = defensiveMovePostion - GameCharacter.MovementComponent.CharacterCenter;
			Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, dir.normalized, dir.magnitude, Color.white, 10f, 100, DebugAreas.Combat);
			//dir *= deltaTimeScale;
			GameCharacter.MovementComponent.IgnoreGravity = true;
			GameCharacter.MovementComponent.IgnoreDeltaTime = true;

			GameCharacter.MovementComponent.MovementVelocity = dir;
			GameCharacter.MovementComponent.DeactiveStepup();

			defensiveShouldMove = false;
			DidMove();
		}
	}

	private void RotateCharacterToTeleportLocationOrEnemy()
	{
		// Rotate Character
		Vector3 targetLocation = GameCharacter.MovementComponent.CharacterCenter + defensiveMovePostion;
		Ultra.Utilities.DrawWireSphere(targetLocation, .1f, Color.black, 10f, 200, DebugAreas.Combat);

		Vector3 dir = Vector3.zero;
		if (targetTeleportCharacter != null)
			dir = targetTeleportCharacter.MovementComponent.CharacterCenter - targetLocation;
		else
			dir = targetLocation - GameCharacter.MovementComponent.CharacterCenter;

		Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, dir.normalized, dir.magnitude, Color.red, 10f, 200, DebugAreas.Combat);
		dir = new Vector3(dir.x, 0f, 0f);
		GameCharacter.RotateToDir(dir);
		Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, dir.normalized, dir.magnitude, Color.cyan, 10f, 200, DebugAreas.Combat);
	}

	public override void DefensiveActionStart()
	{
		targetTeleportCharacter = Ultra.HypoUttilies.FindCharactereNearestToDirectionTresholdWithRange(GameCharacter.MovementComponent.CharacterCenter, defensiveMovePostion.normalized.magnitude > 0 ? defensiveMovePostion.normalized : GameCharacter.transform.forward, attackData.teleportAimTrsehholdHalfAngle, Ultra.Utilities.IgnoreAxis(defensiveMovePostion, EAxis.YZ).normalized.magnitude > 0 ? Ultra.Utilities.IgnoreAxis(defensiveMovePostion, EAxis.YZ).normalized : GameCharacter.transform.forward, attackData.teleportRange, ref GameCharacter.CharacterDetection.TargetGameCharacters);
		Vector3 targetPosition = Vector3.zero;
		if (targetTeleportCharacter != null)
		{
			Vector3 dir = GameCharacter.MovementComponent.CharacterCenter - targetTeleportCharacter.MovementComponent.CharacterCenter;
			dir = new Vector3(dir.x, 0f, 0f).normalized;
			float lenght = targetTeleportCharacter.MovementComponent.CapsuleCollider.radius + GameCharacter.MovementComponent.CapsuleCollider.radius + attackData.teleportCharacterDistance;
			targetPosition = targetTeleportCharacter.MovementComponent.CharacterCenter + dir * lenght;
			Ultra.Utilities.DrawWireSphere(targetPosition, .5f, Color.red, 10f, 100, DebugAreas.Combat);
			targetTeleportCharacter.MovementComponent.RequestMovementOverride(0.5f);
		}
		else
		{
			targetPosition = GameCharacter.MovementComponent.CharacterCenter + (GameCharacter.MovementInput.normalized.magnitude > 0 ? new Vector3(GameCharacter.MovementInput.x, 0f, 0f).normalized : GameCharacter.transform.forward.normalized) * (attackData.teleportRange / 2);
		}
		defensiveMovePostion = targetPosition;
		defensiveShouldMove = true;

		Weapon.SpawnedWeapon?.SetActive(false);
		Weapon.SecondSpawnedWeapon?.SetActive(false);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = false;
		GameCharacter.MovementComponent.MoveThroughCharacterLayer();
	}

	public override void DefensiveActionEnd()
	{
		Weapon.SpawnedWeapon?.SetActive(true);
		Weapon.SecondSpawnedWeapon?.SetActive(true);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = true;
		GameCharacter.RequestBestCharacterState();
		GameCharacter.MovementComponent.IgnoreGravity = false;
		GameCharacter.MovementComponent.IgnoreDeltaTime = false;
	}

	async void DidMove()
	{
		await new WaitForEndOfFrame();
		await new WaitForEndOfFrame();

		GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
		GameCharacter.MovementComponent.ActivateStepup();
		GameCharacter.MovementComponent.SetLayerToDefault();

		if (targetTeleportCharacter != null)
			GameCharacter.RotateToDir(targetTeleportCharacter.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter);
	}

	public override void DefensiveActionStateEnd()
	{
		base.DefensiveActionStateEnd();

		// Dont go further if Object gets destroyed this frame
		if (GameCharacter.gameObject.activeSelf == false) return;

		Weapon?.SpawnedWeapon?.SetActive(true);
		Weapon?.SecondSpawnedWeapon?.SetActive(true);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = true;
		GameCharacter.MovementComponent.SetLayerToDefault();
		defensiveMovePostion = Vector3.zero;
		GameCharacter.MovementComponent.IgnoreGravity = false;
		GameCharacter.MovementComponent.IgnoreDeltaTime = false;

		if (defensiveShouldMove)
			Ultra.Utilities.Instance.DebugLogOnScreen("Character Left State without move!", 10f, StringColor.Red, 100, DebugAreas.Combat);
	}

	public override bool CanLeaveDefensiveState()
	{
		if (defensiveShouldMove) return false;
		return base.CanLeaveDefensiveState();
	}

	public override void GroundReset()
	{
		canTeleport = true;
	}

	void OnWeaponChanged(WeaponBase newWeapon, WeaponBase oldWeapon, GameCharacter gameCharacter)
	{
		if (oldWeapon == Weapon)
		{
			// Only Track when not Equiped for teleport in air reset
			GameCharacter.CombatComponent.onCharacterDamagedCharacter += OnCharacterDamagedCharacter;

		} else if (newWeapon == Weapon)
		{
			// Only Track when not Equiped for teleport in air reset
			GameCharacter.CombatComponent.onCharacterDamagedCharacter -= OnCharacterDamagedCharacter;

		}
	}

	void OnCharacterDamagedCharacter(GameCharacter attackingCharacter, GameCharacter damageCharacter, float damage)
	{
		if (damage > 0)
		{
			if (!canTeleport) canTeleport = true;
		}
	}

	public override void ActionInterupted()
	{
		base.ActionInterupted();
		DefensiveActionStateEnd();
	}

	public override float GetActionRanting()
	{
		return attackData.Rating;
	}

	public override float GetActionDischarge()
	{
		return attackData.Discharge;
	}

	public override ActionBase CreateCopy()
	{
		TeleportDefensiveAction copy = new TeleportDefensiveAction();
		copy.attackData = attackData;
		return copy;
	}
}
