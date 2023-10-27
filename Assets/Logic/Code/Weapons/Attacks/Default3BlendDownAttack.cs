using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[Serializable]
//public class Default3BlendDownAttackkData : ActionData
//{
//	public AimBlendAnimations downAttack;
//	public AimBlendAnimations downAttackHold;
//	public AnimationClip trigger;
//	public float smashDownDistance = 0.2f;
//	public float interpSpeed = 10f;
//	public float speed = 40f;
//}
//
//[Serializable]
//[CreateAssetMenu(fileName = "New Default3BlendDownAttack", menuName = "Assets/Weapons/Attacks/Default3BlendDownAttack")]
//public class Default3BlendDownAttack : BaseAttack
//{
//	public Default3BlendDownAttackkData attackData = new Default3BlendDownAttackkData();
//
//	Ultra.Timer backupFallTimer;
//	bool landed = false;
//	bool startFalling = false;
//	Vector3 targetDir = Vector3.zero;
//	float targetAngel;
//	bool StartFalling
//	{
//		get { return startFalling; }
//		set
//		{
//			startFalling = value;
//			if (startFalling)
//			{
//				GameCharacter.MovementComponent.MoveThroughCharacterLayer();
//				Weapon.HitDetectionStart();
//			}
//		}
//	}
//	public override void Init(GameCharacter gameCharacter, WeaponBase weapon)
//	{
//		base.Init(gameCharacter, weapon);
//
//		backupFallTimer = new Ultra.Timer(0.4f, true);
//		backupFallTimer.onTimerFinished += OnBackupTimerFinished;
//	}
//
//	public override void StartAttack()
//	{
//		Weapon.Attack3BlendSpace(attackData.downAttack, EAnimationType.Default);
//		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Attack);
//		Weapon.AttackAnimType = EAttackAnimType.Combat3Blend;
//
//		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += AttackTimerFinished;
//		GameCharacter.MovementComponent.onCharacterGroundedChanged += OnCharacterGroundedChanged;
//
//		landed = false;
//		StartFalling = false;
//
//		Vector3 maxDir = (GameCharacter.transform.forward + Vector3.down).normalized;
//		GameCharacter target = Ultra.HypoUttilies.FindCHaracterNearestToDirectionWithMinAngel(GameCharacter.MovementComponent.CharacterCenter, Vector3.down, GameCharacter.transform.forward, 45f, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
//		GameCharacter.AnimController.Combat3BlendDir = 0f;
//		backupFallTimer.Start();
//		if (target == null)
//		{
//			targetDir = maxDir;
//		}
//		else
//		{
//			// Aim towards feet for better results
//			targetDir = (target.transform.position - GameCharacter.MovementComponent.CharacterCenter).normalized;
//			float minDistance = GameCharacter.MovementComponent.Radius + target.MovementComponent.Radius + attackData.smashDownDistance;
//			Vector3 newTargetPos = target.transform.position + Ultra.Utilities.IgnoreAxis(targetDir * -1, EAxis.YZ).normalized * minDistance;
//			targetDir = (newTargetPos - GameCharacter.MovementComponent.CharacterCenter).normalized;
//		}
//		targetAngel = Vector3.Angle(targetDir, GameCharacter.transform.forward);
//	}
//
//	public override void OnHit(GameObject hitObj)
//	{
//		IDamage damageInterface = Weapon.GetDamageInterface(hitObj);
//		if (damageInterface == null) return;
//		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
//		if (enemyCharacter == null || enemyCharacter.StateMachine == null || enemyCharacter.CombatComponent == null) return;
//		if (enemyCharacter.CombatComponent.CanGetHooked() && enemyCharacter.StateMachine.CanSwitchToStateOrIsState(EGameCharacterState.HookedToCharacter))
//		{
//			enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
//			Weapon.HookCharacterToCharacter(enemyCharacter);
//			enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.HookedToCharacter);
//		}
//	}
//
//	public override void SetData(ActionData data)
//	{
//		attackData = (Default3BlendDownAttackkData)data;
//	}
//
//	public override Type GetAttackDataType()
//	{
//		return attackData.GetType();
//	}
//
//	void AttackTimerFinished()
//	{
//		
//		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
//		if (!Weapon.WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
//		if (Weapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0)
//		{
//			GameCharacter.AnimController.ApplyCombat3BlendTree(Weapon.WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks[Weapon.AttackIndex].aimBlendTypes.blendHoldAnimations);
//			GameCharacter.AnimController.InCombat3Blend = true;
//		}
//		
//	}
//
//	void OnCharacterGroundedChanged(bool newState)
//	{
//		
//		if (!landed) OnAirDownHitLanding();
//		
//	}
//
//	public override void AttackPhaseStart()
//	{
//		base.AttackPhaseStart();
//		StartFalling = true;
//	}
//
//	void OnAirDownHitLanding()
//	{
//		landed = true;
//		GameCharacter.MovementComponent.onCharacterGroundedChanged -= OnCharacterGroundedChanged;
//		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
//		GameCharacter.AnimController.InCombat3Blend = false;
//
//		if (attackData.trigger != null)
//		{
//			SetTriggerAnimation();
//		}
//
//		GameCharacter.AnimController.HoldAttack = false;
//		GameCharacter.AnimController.InAttack = false;
//		GameCharacter.MovementComponent.SetLayerToDefault();
//
//		CameraController.Instance.ShakeCamerea(2);
//
//		Weapon.UnHookAllHookedCharacerts();
//
//		foreach (GameObject obj in Weapon.HitObjects)
//		{
//			OnHit(obj);
//		}
//
//		Weapon.HitDetectionEnd();
//	}
//
//	private void SetTriggerAnimation()
//	{
//		GameCharacter.AnimController.SetTriggerAttack(attackData.trigger);
//		GameCharacter.CombatComponent.AttackTimer.Start(attackData.trigger.length);
//		GameCharacter.AnimController.AttackTriggerTimer.Start(attackData.trigger.length);
//
//		GameCharacter.AnimController.TriggerAttack = true;
//	}
//
//	void OnBackupTimerFinished()
//	{
//		if (!landed && !StartFalling)
//			StartFalling = true;
//	}
//
//	public override void PostAttackStateLogic(float deltaTime)
//	{
//		base.PostAttackStateLogic(deltaTime);
//		backupFallTimer.Update(deltaTime);
//
//		GameCharacter.AnimController.Combat3BlendDir = Mathf.Lerp(GameCharacter.AnimController.Combat3BlendDir, Ultra.Utilities.Remap(targetAngel, 0, 180, 1, -1), deltaTime * attackData.interpSpeed);
//		UpdateAirDownAttack(deltaTime);
//	}
//
//	void UpdateAirDownAttack(float deltaTime)
//	{
//		if (!landed && StartFalling)
//		{
//			Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
//			velocity = targetDir * attackData.speed;
//			GameCharacter.MovementComponent.MovementVelocity = velocity;
//		}
//	}
//}