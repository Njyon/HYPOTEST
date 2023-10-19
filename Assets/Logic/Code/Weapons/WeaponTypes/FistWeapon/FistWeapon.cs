using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FistWeapon : WeaponBase
{
	float teleportCharacterDistance = 0.5f;
	Vector3 defensiveMoveVector;
	Vector3 defensiveMoveInput;
	bool defensiveShouldMove = false;
	bool canTeleport = true;

	GameCharacter targetTeleportCharacter;

	public FistWeapon() { }
	public FistWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{
	}

	public override void InitWeapon()
	{
		base.InitWeapon();

	}

	public override void EquipWeapon()
    {
        base.EquipWeapon();
		// Only Track when not Equiped for teleport in air reset
		GameCharacter.CombatComponent.onCharacterDamagedCharacter -= OnCharacterDamagedCharacter;

	}

    public override void UnEquipWeapon()
    {
        base.UnEquipWeapon();
		// Only Track when not Equiped for teleport in air reset
		GameCharacter.CombatComponent.onCharacterDamagedCharacter += OnCharacterDamagedCharacter;
		GameCharacter.GameCharacterData.MeshRenderer.enabled = true;
		GameCharacter.MovementComponent.ActivateStepup();
		GameCharacter.MovementComponent.SetLayerToDefault();
		UnHookAllHookedCharacerts();
	}

    public override void UpdateWeapon(float deltaTime)
    {
        base.UpdateWeapon(deltaTime);
    }

    public override AttackAnimationData GroundAttack(float attackDeltaTime)   
    {
        return base.GroundAttack(attackDeltaTime);
    }
    public override AttackAnimationData GroundUpAttack(float attackDeltaTime)    
    {
        return base.GroundUpAttack(attackDeltaTime);
    }
    public override AttackAnimationData GroundDownAttack(float attackDeltaTime)  
    {
        return base.GroundDownAttack(attackDeltaTime);
    }
    public override AttackAnimationData GroundDirectionAttack(float attackDeltaTime)   
    {
        return base.GroundDirectionAttack(attackDeltaTime);
    }

    public override AttackAnimationData AirAttack(float attackDeltaTime)  
    {
        return base.AirAttack(attackDeltaTime);
    }
    public override AttackAnimationData AirUpAttack(float attackDeltaTime)
    {
        return base.AirUpAttack(attackDeltaTime);
    }
    public override AttackAnimationData AirDownAttack(float attackDeltaTime)
	{
		return base.AirDownAttack(attackDeltaTime);
	}
    public override AttackAnimationData AirDirectionAttack(float attackDeltaTime) 
    {
        return base.AirDirectionAttack(attackDeltaTime);
    }

	public override AttackAnimationData DefensiveAction()
	{
		AttackAnimationData returnData = null; 
		if (canTeleport)
		{
			returnData = base.DefensiveAction();
			defensiveMoveVector = GameCharacter.MovementInput;
			canTeleport = false;
		}
		return returnData;
	}

	public override void AttackPhaseStart()
	{
		base.AttackPhaseStart();
	}

	public override void GroundAttackHit(GameObject hitObj)
	{
		base.GroundAttackHit(hitObj);
	}

	public override void GroundUpAttackHit(GameObject hitObj)
	{
		base.GroundUpAttackHit(hitObj);
	}

	public override void GroundDownAttackHit(GameObject hitObj)
	{
		base.GroundDownAttackHit(hitObj);
	}

	public override void GroundDirectionAttackHit(GameObject hitObj)
	{
		base.GroundDirectionAttackHit(hitObj);
	}

	public override void AirAttackHit(GameObject hitObj)
	{
		base.AirAttackHit(hitObj);
	}


	public override void AirUpAttackHit(GameObject hitObj)
	{
		base.AirUpAttackHit(hitObj);
	}

	public override void AirDownAttackHit(GameObject hitObj)
	{
		base.AirDownAttackHit(hitObj);
	}

	public override void AirDirectionAttackHit(GameObject hitObj)
	{
		base.AirDirectionAttackHit(hitObj);
	}

	public override void EndAttackStateLogic()
	{
		UnHookAllHookedCharacerts();
		GameCharacter.MovementComponent.SetLayerToDefault();

		base.EndAttackStateLogic();
	}

	public override void GroundReset()
	{
		base.GroundReset();
		canTeleport = true;
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		base.PostAttackStateLogic(deltaTime);

		if (CurrentAttackType == EExplicitAttackType.DefensiveAction)
		{
			if (defensiveShouldMove)
			{
				Ultra.Utilities.DrawArrow(GameCharacter.MovementComponent.CharacterCenter, defensiveMoveVector.normalized, defensiveMoveVector.magnitude, Color.white, 10f, 100, DebugAreas.Combat);

				RotateCharacterToTeleportLocationOrEnemy();

				float deltaTimeScale = 1f / Time.deltaTime;
				defensiveMoveVector *= deltaTimeScale;
				GameCharacter.MovementComponent.MovementVelocity = defensiveMoveVector;
				GameCharacter.MovementComponent.DeactiveStepup();

				defensiveShouldMove = false;
				DidMove();
			}
		}
	}

	private void RotateCharacterToTeleportLocationOrEnemy()
	{
		// Rotate Character
		Vector3 targetLocation = GameCharacter.MovementComponent.CharacterCenter + defensiveMoveVector;
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
		float angelTreshold = 5f;
		//targetTeleportCharacter = Ultra.HypoUttilies.FindCharactereNearestToDirectionTresholdWithRange(GameCharacter.MovementComponent.CharacterCenter, defensiveMoveVector.normalized.magnitude > 0 ? defensiveMoveVector.normalized : GameCharacter.transform.forward, angelTreshold, Ultra.Utilities.IgnoreAxis(defensiveMoveVector, EAxis.YZ).normalized.magnitude > 0 ? Ultra.Utilities.IgnoreAxis(defensiveMoveVector, EAxis.YZ).normalized : GameCharacter.transform.forward, CurrentAction.extraData.rangeValue, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
		Vector3 targetPosition = Vector3.zero;
		if (targetTeleportCharacter != null)
		{
			Vector3 dir =  GameCharacter.MovementComponent.CharacterCenter - targetTeleportCharacter.MovementComponent.CharacterCenter;
			dir = new Vector3(dir.x, 0f, 0f).normalized;
			float lenght = targetTeleportCharacter.MovementComponent.CapsuleCollider.radius + GameCharacter.MovementComponent.CapsuleCollider.radius + teleportCharacterDistance;
			targetPosition = targetTeleportCharacter.MovementComponent.CharacterCenter + dir * lenght;
			Ultra.Utilities.DrawWireSphere(targetPosition, .5f, Color.red, 10f, 100, DebugAreas.Combat);
			targetTeleportCharacter.MovementComponent.RequestMovementOverride(0.5f);
		}
		else
		{
			//targetPosition = GameCharacter.MovementComponent.CharacterCenter + (GameCharacter.MovementInput.normalized.magnitude > 0 ? new Vector3(GameCharacter.MovementInput.x, 0f, 0f) : GameCharacter.transform.forward) * (CurrentAction.extraData.rangeValue / 2);
		}
		defensiveMoveVector = targetPosition - GameCharacter.MovementComponent.CharacterCenter;
		defensiveShouldMove = true;

		SpawnedWeapon?.SetActive(false);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = false;
		GameCharacter.MovementComponent.MoveThroughCharacterLayer();
	}	

	public override void DefensiveActionEnd()
	{
		SpawnedWeapon?.SetActive(true);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = true;
		GameCharacter.RequestBestCharacterState();
	}

	async void DidMove()
	{
		await new WaitForEndOfFrame();
		await new WaitForEndOfFrame();

		GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
		GameCharacter.MovementComponent.ActivateStepup();
		GameCharacter.MovementComponent.SetLayerToDefault();
	}

	public override void DefensiveActionStateEnd()
	{
		base.DefensiveActionStateEnd();
		SpawnedWeapon?.SetActive(true);
		GameCharacter.GameCharacterData.MeshRenderer.enabled = true;
		GameCharacter.MovementComponent.SetLayerToDefault();
		defensiveMoveVector = Vector3.zero;

		if (defensiveShouldMove)
			Ultra.Utilities.Instance.DebugLogOnScreen("Character Left State without move!", 10f, StringColor.Red, 100, DebugAreas.Combat);
	}

	public override bool CanLeaveDefensiveState()
	{
		if (defensiveShouldMove) return false;
		return base.CanLeaveDefensiveState();
	}

	void OnCharacterDamagedCharacter(GameCharacter attackingCharacter, GameCharacter damageCharacter, float damage)
	{
		if (damage > 0)
		{
			if (!canTeleport) canTeleport = true;
		}
	}
}