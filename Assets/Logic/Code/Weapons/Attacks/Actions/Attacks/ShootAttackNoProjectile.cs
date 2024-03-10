using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

[Serializable]
public class ShootAttackNoProjectileData : AttackData
{
	public AnimationClip shootAddativeAnimation;
	public bool leftHand;
	public float forceAimDuration = 5f;
	public bool cameraShake = true;
	[ConditionalField("cameraShake")] 
	public int shakeIndex = 0;
}

public class ShootAttackNoProjectile : AttackBase
{
	public ShootAttackNoProjectileData attackData;
	WeaponObjData weaponObjData;
	ParticleSystemPool shootFlashParticlePool;
	ParticleSystemPool hitParticlePool;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, action);

		weaponObjData = attackData.leftHand ? GameCharacter.CombatComponent.CurrentWeapon.SpawnedWeapon.GetComponent<WeaponObjData>() : GameCharacter.CombatComponent.CurrentWeapon.SecondSpawnedWeapon.GetComponent<WeaponObjData>();
		shootFlashParticlePool = weapon.GetRangeWeaponFlashParticlePool();
		hitParticlePool = weapon.GetRangeWeaponHitParticlePool();
	}

	public override void StartAction()
	{
		GameCharacter.AnimController.ApplyUpperBodyAddativeAnimationToState(attackData.shootAddativeAnimation);
		GameCharacter.AnimController.InUpperBodyAddativeState = true;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += OnTimerFinished;
		GameCharacter.CombatComponent.AttackTimer.Start(attackData.shootAddativeAnimation.length);
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Shoot);
		GameCharacter.AnimController.ApplyBlendTree(GameCharacter.CombatComponent.CurrentWeapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].AimAnimations);
		Weapon.PlayAttackSound(0);
		Weapon.SpawnWeaponFlash(weaponObjData);

		GameCharacter target = Ultra.HypoUttilies.FindCharactereNearestToDirection(GameCharacter.MovementComponent.CharacterCenter, GameCharacter.MovementInput.magnitude > 0 ? GameCharacter.MovementInput : GameCharacter.transform.forward, ref GameCharacter.CharacterDetection.DetectedGameCharacters);
		GameCharacter.CombatComponent.AimCharacter = target;

		if (target != null)
			GameCharacter.RotateToDir((target.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter).IgnoreAxis(EAxis.YZ));

		Vector3 weaponTipPos = weaponObjData.weaponTip.transform.position;
		Vector3 bulletDirection = target != null ? target.MovementComponent.CharacterCenter - weaponObjData.weaponTip.transform.position : GameCharacter.transform.forward * 9999f;
		
		Ultra.Utilities.DrawArrow(weaponTipPos, bulletDirection, bulletDirection.magnitude, Color.magenta, 10f, 200, DebugAreas.Combat);
		RaycastHit[] hits = Physics.RaycastAll(weaponTipPos, bulletDirection, 9999f, -5, QueryTriggerInteraction.Ignore);
		foreach (RaycastHit hit in hits)
		{ 
			if (hit.collider.gameObject.layer == GameCharacter.CharacterLayer)
			{
				Transform parent = hit.collider.transform.parent;
				while (parent.parent != null)
				{
					parent = parent.parent;
				}

				if (parent == GameCharacter.transform) continue;
			}
			Vector3 hitDir = hit.point - weaponTipPos;
			Ultra.Utilities.DrawArrow(weaponTipPos, hitDir, hitDir.magnitude, Color.red, 10f, 200, DebugAreas.Combat);
			IDamage iDamage = hit.collider?.GetComponent<IDamage>();
			if (iDamage != null)
			{
				iDamage.DoDamage(GameCharacter, attackData.Damage, true, true, false);
				Weapon.SpawnDamageHitEffect(hit);
				break;
			}
		}
		Weapon.AddForceAimBuff(attackData.forceAimDuration);

		if (attackData.cameraShake)
			CameraController.Instance?.ShakeCamerea(attackData.shakeIndex);
	}


	public override void ActionInterupted()
	{
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= OnTimerFinished;
		GameCharacter.CombatComponent.AttackTimer.Stop();
		GameCharacter.AnimController.InUpperBodyAddativeState = false;
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
	}

	void OnTimerFinished()
	{
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= OnTimerFinished;
		GameCharacter.AnimController.InUpperBodyAddativeState = false;
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Shoot);
	}

	public override ActionBase CreateCopy()
	{
		ShootAttackNoProjectile copy = new ShootAttackNoProjectile();
		copy.attackData = attackData;
		return copy;
	}
}
