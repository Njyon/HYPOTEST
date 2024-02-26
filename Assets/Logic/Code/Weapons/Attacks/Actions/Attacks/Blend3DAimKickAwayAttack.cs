using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using static Ultra.Timer;

[Serializable]
public class Blend3DAimKickAwayAttackData : AttackData
{
	public AnimationClip shootAddativeAnimation;
	public float forceAimDuration = 1f;
	public float stunTime = 1f;
	public float kickAwayStrenght = 20f;
}

public class Blend3DAimKickAwayAttack : AttackBase
{
	[Tooltip("Alaways Use HitDetection Box on this Attack")]
	[Header("Alaways Use HitDetection Box on this Attack")]
	public Blend3DAimKickAwayAttackData attackData;
	WeaponObjData weaponObjData;

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon, action);

		weaponObjData = GameCharacter.CombatComponent.CurrentWeapon.SpawnedWeapon.GetComponent<WeaponObjData>();
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

		GameCharacter.CombatComponent.HitDetectionGameObject.transform.position = weaponTipPos;
		// Save roation & reset it after calculation sadly i dont know a better way to fix the rotation right now
		var rot = GameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation;
		if (target != null)
		{
			// Safer than using weaponTip for rotation because weapon might be influenced by animations
			GameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation = Quaternion.LookRotation((target.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter).normalized) * Quaternion.Euler(0, -90, 0);
		} else
		{
			GameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation = weaponObjData.weaponTip.transform.rotation;
		}
		GameCharacter.CombatComponent.HitDetectionGameObject.transform.Translate(Weapon.CurrentAction.data.offset, Space.Self);
		Collider[] colliders = Physics.OverlapBox(GameCharacter.CombatComponent.HitDetectionGameObject.transform.position, Weapon.CurrentAction.data.boxDimensions / 2, GameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation, GameCharacter.CharacterLayer, QueryTriggerInteraction.Ignore);
		foreach (Collider collider in colliders)
		{
			HitObj(collider.gameObject);
		}
		//Ultra.Utilities.DrawBox(hitDetectionGameObject.transform.position, Quaternion.identity, lastAttack.data.boxDimensions, Color.red, 200);
		Ultra.Utilities.DrawBox(GameCharacter.CombatComponent.HitDetectionGameObject.transform.position, GameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation, Weapon.CurrentAction.data.boxDimensions, Color.red);


		Weapon.AddForceAimBuff(attackData.forceAimDuration);
		// Apply saved rotation so default hit ditection works like normal again, sadly found no better way to fix the roation
		GameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation = rot;
	}

	void HitObj(GameObject hitObj)
	{
		if (hitObj == GameCharacter.gameObject) return;

		GameCharacter gc = hitObj.GetComponent<GameCharacter>();
		if (gc != null)
		{
			DoDamage(hitObj, attackData.Damage);
			// Kickaway needs to happen after damage
			Weapon.KickAway(gc, attackData.stunTime, (gc.MovementComponent.CharacterCenter - GameCharacter.MovementComponent.CharacterCenter).normalized, attackData.kickAwayStrenght);
			Weapon.SpawnDamageHitEffect(gc);
		}else
		{
			DoDamage(hitObj, attackData.Damage);
		}
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
		Blend3DAimKickAwayAttack copy = new Blend3DAimKickAwayAttack();
		copy.attackData = attackData;
		return copy;
	}
}
