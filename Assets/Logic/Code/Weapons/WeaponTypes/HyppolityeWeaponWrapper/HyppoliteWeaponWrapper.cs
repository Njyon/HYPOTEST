using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyppoliteWeaponWrapper : WeaponBase
{
	ScriptableWeaponWrapper scriptableWeaponWrapper;
	int pistolIndex = 0;
	int shotgunIndex = 1;
	WeaponBase currentUsedWeapon;
	WeaponBase CurrentUsedWeapon
	{
		get 
		{
			if (currentUsedWeapon == null)
			{
				currentUsedWeapon = scriptableWeaponWrapper.weapons[0].Weapon;
			}
			return currentUsedWeapon; 
		}
		set
		{
			if (value != currentUsedWeapon)
			{
				currentUsedWeapon?.UnEquipWeapon();
				currentUsedWeapon = value;
				currentUsedWeapon?.EquipWeapon();
			}
		}
	}
	public override ScriptableWeapon WeaponData => CurrentUsedWeapon?.WeaponData;
	public override GameObject SpawnedWeapon => CurrentUsedWeapon?.SpawnedWeapon;
	public override GameObject SecondSpawnedWeapon => CurrentUsedWeapon?.SecondSpawnedWeapon;
	public override GameObject SpawnedWeaponBones => CurrentUsedWeapon?.SpawnedWeaponBones;
	public override bool IsHitDetecting => CurrentUsedWeapon.IsHitDetecting;
	public override AttackAnimationData CurrentAction => CurrentUsedWeapon?.CurrentAction;
	public override EExplicitAttackType CurrentAttackType => CurrentUsedWeapon.CurrentAttackType;
	public override EExplicitAttackType LastAttackType => CurrentUsedWeapon.LastAttackType;
	public override EAttackAnimType AttackAnimType { get => CurrentUsedWeapon.AttackAnimType; set => CurrentUsedWeapon.AttackAnimType = value; }
	public override int ComboIndexInSameAttack => CurrentUsedWeapon.ComboIndexInSameAttack;
	public override List<GameObject> HitObjects => CurrentUsedWeapon?.HitObjects;
	public override int AttackIndex => CurrentUsedWeapon.AttackIndex;
	public override bool ShouldPlayHitSound { get => CurrentUsedWeapon.ShouldPlayHitSound; set => CurrentUsedWeapon.ShouldPlayHitSound = value; }
	//public override WeaponBase This => CurrentUsedWeapon?.This;
	public override ScriptableWeaponAnimationData AnimationData
	{
		get
		{
			if (CurrentUsedWeapon.AnimationData == null)
			{
				var data = ScriptableObject.CreateInstance<ScriptableWeaponAnimationData>();
				data.Copy(CurrentUsedWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name]);
				if (CurrentUsedWeapon.WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) CurrentUsedWeapon.AnimationData = data;
			}
			return CurrentUsedWeapon.AnimationData;
		}
	}

	public HyppoliteWeaponWrapper() { }
	public HyppoliteWeaponWrapper(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{
		scriptableWeaponWrapper = (ScriptableWeaponWrapper)weaponData;
		if (scriptableWeaponWrapper == null)
			Ultra.Utilities.Instance.DebugErrorString("HyppoliteWeaponWrapper", "HyppoliteWeaponWrapper Constructor", "scriptableWeaponWrapper was null after casting!");
	}

	public override void EquipWeapon()
	{
		CurrentUsedWeapon.EquipWeapon();
	}

	public override void UnEquipWeapon()
	{
		CurrentUsedWeapon?.UnEquipWeapon();
	}

	public override void UpdateWeapon(float deltaTime)
	{
		CurrentUsedWeapon?.UpdateWeapon(deltaTime);
	}

	public override AttackAnimationData GroundAttack(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[pistolIndex]?.Weapon;
		return CurrentUsedWeapon?.GroundAttack(attackDeltaTime);
	}

	public override AttackAnimationData GroundUpAttack(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[pistolIndex]?.Weapon;
		return CurrentUsedWeapon?.GroundUpAttack(attackDeltaTime);
	}

	public override AttackAnimationData GroundDownAttack(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[shotgunIndex]?.Weapon;
		return CurrentUsedWeapon?.GroundDownAttack(attackDeltaTime);
	}

	public override AttackAnimationData GroundDirectionAttack(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[shotgunIndex]?.Weapon;
		return CurrentUsedWeapon?.GroundDirectionAttack(attackDeltaTime);
	}

	public override AttackAnimationData AirAttack(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[pistolIndex]?.Weapon;
		return CurrentUsedWeapon?.AirAttack(attackDeltaTime);
	}

	public override AttackAnimationData AirUpAttack(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[pistolIndex]?.Weapon;
		return CurrentUsedWeapon?.AirUpAttack(attackDeltaTime);
	}

	public override AttackAnimationData AirDownAttack(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[pistolIndex]?.Weapon;
		return CurrentUsedWeapon?.AirDownAttack(attackDeltaTime);
	}

	public override AttackAnimationData AirDirectionAttack(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[shotgunIndex]?.Weapon;
		return CurrentUsedWeapon?.AirDirectionAttack(attackDeltaTime);
	}

	public override AttackAnimationData GapCloserAttack(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[pistolIndex]?.Weapon;
		return CurrentUsedWeapon?.GapCloserAttack(attackDeltaTime);
	}

	public override AttackAnimationData Ultimate(float attackDeltaTime)
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[pistolIndex]?.Weapon;
		return CurrentUsedWeapon?.Ultimate(attackDeltaTime);
	}

	public override AttackAnimationData DefensiveAction()
	{
		CurrentUsedWeapon = scriptableWeaponWrapper?.weapons[pistolIndex]?.Weapon;
		return CurrentUsedWeapon?.DefensiveAction();
	}


	public override void GroundAttackHit(GameObject hitObj)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.OnHit(hitObj);
	}

	public override void GroundUpAttackHit(GameObject hitObj)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.OnHit(hitObj);
	}

	public override void GroundDownAttackHit(GameObject hitObj)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.OnHit(hitObj);
	}

	public override void GroundDirectionAttackHit(GameObject hitObj)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.OnHit(hitObj);
	}

	public override void AirAttackHit(GameObject hitObj)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.OnHit(hitObj);
	}

	public override void AirUpAttackHit(GameObject hitObj)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.OnHit(hitObj);
	}

	public override void AirDownAttackHit(GameObject hitObj)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.OnHit(hitObj);
	}

	public override void AirDirectionAttackHit(GameObject hitObj)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.OnHit(hitObj);
	}

	public override void DefensiveActionHit(GameObject hitObj)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.OnHit(hitObj);
	}

	public override void SetWeaponReadyPoseBasedOnStates()
	{
		CurrentUsedWeapon?.SetWeaponReadyPoseBasedOnStates();
	}

	public override AttackAnimationData GetAttackAnimationData()
	{
		return CurrentUsedWeapon?.GetAttackAnimationData();
	}

	public override void HitDetectionStart()
	{
		CurrentUsedWeapon?.HitDetectionStart();
	}

	public override void HitDetectionEnd()
	{
		CurrentUsedWeapon?.HitDetectionEnd();
	}

	public override void WeaponColliderExit(Collider other)
	{
		CurrentUsedWeapon?.WeaponColliderExit(other);
	}

	public override void StartAttackStateLogic()
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.StartAttackStateLogic();
	}

	public override void PreAttackStateLogic(float deltaTime)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.PreAttackStateLogic(deltaTime);
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.PostAttackStateLogic(deltaTime);
	}

	public override void EndAttackStateLogic()
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.EndAttackStateLogic();
		CurrentUsedWeapon?.HitObjects.Clear();
	}

	public override void AttackPhaseStart()
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.AttackPhaseStart();
	}

	public override void AttackPhaseEnd()
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.AttackPhaseEnd();
	}

	public override void AttackRecoveryEnd()
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.AttackRecoveryEnd();
	}

	public override void DefensiveActionStateEnd()
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.DefensiveActionStateEnd();
	}

	public override void CharacterArrivedAtRequestedLocation(GameCharacter movedCharacter)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.CharacterArrivedAtRequestedLocation(movedCharacter);
	}

	public override void CharacterMoveToAbort(GameCharacter movedCharacter)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.CharacterMoveToAbort(movedCharacter);
	}

	public override void CharacterMoveToEnd(GameCharacter movedCharacter)
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.CharacterMoveToEnd(movedCharacter);
	}

	public override void DefensiveActionStart()
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.DefensiveActionStart();
	}

	public override void DefensiveActionEnd()
	{
		CurrentUsedWeapon?.CurrentAction?.Action?.DefensiveActionEnd();
	}

	public override bool CanLeaveDefensiveState()
	{
		if (CurrentUsedWeapon == null || CurrentUsedWeapon.CurrentAction == null || CurrentUsedWeapon.CurrentAction.Action == null) return true;
		return CurrentUsedWeapon.CurrentAction.Action.CanLeaveDefensiveState();
	}

	public override void StartParticelEffect(int index)
	{
		CurrentUsedWeapon?.StartParticelEffect(index);
	}

	public override float GetDamage(float damage)
	{
		if (CurrentUsedWeapon == null) return damage;
		return CurrentUsedWeapon.GetDamage(damage);
	}

	public override void PlayAttackSound(int index = -1)
	{
		if (CurrentUsedWeapon == null) return;
		CurrentUsedWeapon.PlayAttackSound(index);
	}

	public override void PlayHitSound()
	{
		if (CurrentUsedWeapon == null) return;
		CurrentUsedWeapon.PlayHitSound();
	}

	public override ParticleSystemPool GetRangeWeaponFlashParticlePool()
	{
		if (CurrentUsedWeapon == null) return null;
		return CurrentUsedWeapon.GetRangeWeaponFlashParticlePool();
	}

	public override ParticleSystemPool GetRangeWeaponHitParticlePool()
	{
		if (CurrentUsedWeapon == null) return null;
		return CurrentUsedWeapon.GetRangeWeaponHitParticlePool();
	}

	public override void InitWeapon()
	{
		for (int i = 0; i < scriptableWeaponWrapper.weapons.Count; i++)
		{
			scriptableWeaponWrapper.weapons[i]?.Weapon?.InitWeapon();
		}
	}

	public override WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon)
	{
		return new HyppoliteWeaponWrapper(gameCharacter, weapon);
	}
}