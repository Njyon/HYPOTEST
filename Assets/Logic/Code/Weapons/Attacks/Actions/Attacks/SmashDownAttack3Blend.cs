using JetBrains.Annotations;
using System;
using UnityEngine;

[Serializable]
public class SmashDownAttack3BlendData : AttackData
{
	public AimBlendAnimations downAttack;
	public AimBlendAnimations downAttackHold;
	public AnimationClip trigger;
	public float smashDownDistance = 0.2f;
	public float interpSpeed = 10f;
	public float speed = 40f;
}
[Serializable]
public class SmashDownAttack3BlendKickUpDataOnLanding
{
	public float stunTime = 0.2f;
	public Vector3 kickDir;
	public float kickStrenght = 1f;
	public float freezTimeAftetKick = 1f;
}

[Serializable]
public class SmashDownAttack3Blend : ActionBase
{
	public SmashDownAttack3BlendData attackData;
	public SmashDownAttack3BlendKickUpDataOnLanding landingData;

	Ultra.Timer backupFallTimer;
	bool landed = false;
	bool startFalling = false;
	Vector3 targetDir = Vector3.zero;
	float targetAngel;
	bool StartFalling
	{
		get { return startFalling; }
		set
		{
			startFalling = value;
			if (startFalling)
			{
				GameCharacter.MovementComponent.MoveThroughCharacterLayer();
				Weapon.HitDetectionStart();
				GameCharacter.MovementComponent.IgnoreGravity = true;
			}
		}
	}
	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction action = null)
	{
		base.Init(gameCharacter, weapon);

		backupFallTimer = new Ultra.Timer(0.4f, true);
		backupFallTimer.onTimerFinished += OnBackupTimerFinished;
	}

	public override void StartAction()
	{
		Weapon.Attack3BlendSpace(attackData.downAttack, EAnimationType.Default);
		GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Attack);
		Weapon.AttackAnimType = EAttackAnimType.Combat3Blend;
		GameCharacter.CombatComponent.AttackTimer.Start(attackData.downAttack.midAnimation.length);

		GameCharacter.CombatComponent.AttackTimer.onTimerFinished += AttackTimerFinished;
		GameCharacter.MovementComponent.onCharacterGroundedChanged += OnCharacterGroundedChanged;

		landed = false;
		StartFalling = false;

		Vector3 maxDir = (GameCharacter.transform.forward + Vector3.down).normalized;
		GameCharacter target = Ultra.HypoUttilies.FindCHaracterNearestToDirectionWithMinAngel(GameCharacter.MovementComponent.CharacterCenter, Vector3.down, GameCharacter.transform.forward, 45f, ref GameCharacter.CharacterDetection.TargetGameCharacters);
		GameCharacter.AnimController.Combat3BlendDir = 0f;
		backupFallTimer.Start();
		if (target == null)
		{
			targetDir = maxDir;
		}
		else
		{
			// Aim towards feet for better results
			targetDir = (target.transform.position - GameCharacter.MovementComponent.CharacterCenter).normalized;
			float minDistance = GameCharacter.MovementComponent.Radius + target.MovementComponent.Radius + attackData.smashDownDistance;
			Vector3 newTargetPos = target.transform.position + Ultra.Utilities.IgnoreAxis(targetDir * -1, EAxis.YZ).normalized * minDistance;
			targetDir = (newTargetPos - GameCharacter.MovementComponent.CharacterCenter).normalized;
		}
		targetAngel = Vector3.Angle(targetDir, GameCharacter.transform.forward);
	}

	public override void OnHit(GameObject hitObj)
	{
		IDamage damageInterface = Weapon.GetDamageInterface(hitObj);
		if (damageInterface == null) return;
		GameCharacter enemyCharacter = hitObj.GetComponent<GameCharacter>();
		if (enemyCharacter == null || enemyCharacter.StateMachine == null || enemyCharacter.CombatComponent == null) return;
		if (enemyCharacter.CombatComponent.CanGetHooked() && enemyCharacter.StateMachine.CanSwitchToStateOrIsState(EGameCharacterState.HookedToCharacter))
		{
			enemyCharacter.CombatComponent.HookedToCharacter = GameCharacter;
			Weapon.HookCharacterToCharacter(enemyCharacter);
			enemyCharacter.MovementComponent.OverrideMovementByHookedCharacter = true;
			enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.HookedToCharacter);
		}
	}

	void AttackTimerFinished()
	{

		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
		if (Weapon.AnimationData == null) return;
		if (Weapon.AnimationData.AirDownAttacks.Count > 0)
		{
			GameCharacter.AnimController.ApplyCombat3BlendTree(attackData.downAttack);
			GameCharacter.AnimController.InCombat3Blend = true;
		}

	}

	void OnCharacterGroundedChanged(bool newState)
	{

		if (!landed) OnAirDownHitLanding();

	}

	public override void AttackPhaseStart()
	{
		base.AttackPhaseStart();
		StartFalling = true;
	}

	void OnAirDownHitLanding()
	{
		landed = true;
		GameCharacter.MovementComponent.IgnoreGravity = false;
		GameCharacter.MovementComponent.onCharacterGroundedChanged -= OnCharacterGroundedChanged;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
		GameCharacter.AnimController.InCombat3Blend = false;

		if (attackData.trigger != null)
		{
			SetTriggerAnimation();
		}

		GameCharacter.AnimController.HoldAttack = false;
		GameCharacter.AnimController.InAttack = false;
		GameCharacter.MovementComponent.SetLayerToDefault();

		CameraController.Instance.ShakeCamerea(2);

		Weapon.UnHookAllHookedCharacerts();

		foreach (GameObject obj in Weapon.HitObjects)
		{
			OnGroundAttackHit(obj);
		}

		Weapon.HitDetectionEnd();
	}

	void OnGroundAttackHit(GameObject hitObject)
	{
		IDamage damageInterface = Weapon.GetDamageInterface(hitObject);
		if (damageInterface == null) return;
		damageInterface.DoDamage(GameCharacter, Weapon.GetDamage(attackData.Damage));

		GameCharacter enemyCharacter = hitObject.GetComponent<GameCharacter>();
		if (enemyCharacter == null) return;
		GameCharacter.CombatComponent.KickAway(enemyCharacter, landingData.stunTime, landingData.kickDir, landingData.kickStrenght);
		GameCharacter.CombatComponent.FreezAfterPush(enemyCharacter, landingData.stunTime, landingData.freezTimeAftetKick);
	}

	private void SetTriggerAnimation()
	{
		GameCharacter.AnimController.SetTriggerAttack(attackData.trigger);
		GameCharacter.CombatComponent.AttackTimer.Start(attackData.trigger.length);
		GameCharacter.AnimController.AttackTriggerTimer.Start(attackData.trigger.length);

		GameCharacter.AnimController.TriggerAttack = true;
	}

	void OnBackupTimerFinished()
	{
		if (!landed && !StartFalling)
			StartFalling = true;
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		base.PostAttackStateLogic(deltaTime);
		backupFallTimer.Update(deltaTime);

		GameCharacter.AnimController.Combat3BlendDir = Mathf.Lerp(GameCharacter.AnimController.Combat3BlendDir, Ultra.Utilities.Remap(targetAngel, 0, 180, 1, -1), deltaTime * attackData.interpSpeed);
		UpdateAirDownAttack(deltaTime);
	}

	void UpdateAirDownAttack(float deltaTime)
	{
		if (!landed && StartFalling)
		{
			Vector3 velocity = GameCharacter.MovementComponent.MovementVelocity;
			velocity = targetDir * attackData.speed;
			GameCharacter.MovementComponent.MovementVelocity = velocity;
		}
	}

	public override void ActionInterupted()
	{
		GameCharacter.MovementComponent.onCharacterGroundedChanged -= OnCharacterGroundedChanged;
		GameCharacter.CombatComponent.AttackTimer.onTimerFinished -= AttackTimerFinished;
		Weapon.UnHookAllHookedCharacerts();
		GameCharacter.MovementComponent.IgnoreGravity = false;
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
		SmashDownAttack3Blend copy = new SmashDownAttack3Blend();
		copy.attackData = attackData;
		copy.landingData = landingData;
		return copy;
	}
}