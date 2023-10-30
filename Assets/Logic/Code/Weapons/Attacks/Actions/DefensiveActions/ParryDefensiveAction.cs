using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParryDefensiveActionData : AttackData
{
	public AnimationClip defensiveAction;
	public AnimationClip defensiveActionHold;
	public AnimationClip counterAttack;
	public GameObject uiELement;
	public GameObject parryParticleEffect;
	public GameObject blockParticleEffect;
	public float parryTime = 0.3f;
	public float addParryTimeOnParry = 0.1f;
	public float parryPoints = 20;
	public float blockPoints = -10;
	public float rotSpeed = 10;
	public float damageMinMultiplier = 0.5f;
	public float damageMaxMultiplier = 3;
}

public class ParryDefensiveAction : AttackBase
{
	public ParryDefensiveActionData attackData;
	Ultra.Timer parryTimer;
	CounterAttackUI ui;
	bool isInterupted = true;
	UpdateHelper updateHelper;
	ParticleSystemPool parryEffectPool;
	ParticleSystemPool blockEffectPool;
	float currentParryValue;
	bool didHitSomething = false;
	Quaternion newDir;

	float CurrentParryValue
	{
		get { return currentParryValue; } 
		set {
			value = Mathf.Clamp(value, 0, 100);
			if (currentParryValue != value)
			{
				currentParryValue = value;
				if (ui != null)
					ui.SetBarFillValue(Unity.Mathematics.math.remap(0, 100, 0, 1, currentParryValue));
			}
		}
	}

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction initAction = null)
	{
		base.Init(gameCharacter, weapon, () =>
		{
			if (updateHelper == null)
			{
				GameObject go = new GameObject(">> " + GameCharacter.name + " UpdateHelper | ParryAction");
				updateHelper = go.AddComponent<UpdateHelper>();
				updateHelper.onUpdate += OnUpdateHelperUpdate;
			}
			parryTimer = new Ultra.Timer();
			parryTimer.onTimerFinished += OnParryTimerFinished;

			SetupParticlePools(gameCharacter);
		});
	}

	private void SetupParticlePools(GameCharacter gameCharacter)
	{
		parryEffectPool = new ParticleSystemPool(attackData.parryParticleEffect, gameCharacter.CreateHolderChild(gameCharacter.name + " ParryEffect Holder"), 2);
		blockEffectPool = new ParticleSystemPool(attackData.blockParticleEffect, gameCharacter.CreateHolderChild(gameCharacter.name + " BlockEffect Holder"), 2);
	}

	public override void StartAction()
	{
		isInterupted = false;
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Parry);
		parryTimer.Start(attackData.parryTime);
		GameCharacter.StateMachine.AddLazyState(EGameCharacterState.DefensiveAction);
		Weapon.AttackAnimType = EAttackAnimType.Default;
		GameCharacter.AnimController.CrossFadeToNextState(GameCharacter.AnimController.DefensiveActionStateHash, 0.2f);
		GameCharacter.AnimController.SetDefensiveAction(attackData.defensiveAction);

		newDir = GameCharacter.transform.rotation;
	}

	public override void SuccessfullParry(GameCharacter damageInitiator, float damage)
	{
		ParticleSystem ps = parryEffectPool.GetValue();
		ps.transform.position = GameCharacter.MovementComponent.CharacterCenter;
		ps.transform.LookAt(damageInitiator.MovementComponent.CharacterCenter.IgnoreAxis(EAxis.Z));
		parryTimer.AddTime(attackData.addParryTimeOnParry);

		newDir = Quaternion.LookRotation(damageInitiator.MovementComponent.CharacterCenter.IgnoreAxis(EAxis.YZ) - GameCharacter.MovementComponent.CharacterCenter.IgnoreAxis(EAxis.YZ), Vector3.up);

		CurrentParryValue += attackData.parryPoints;
	}


	public override void SuccessfullBlock(GameCharacter damageInitiator, float damage)
	{
		ParticleSystem ps = blockEffectPool.GetValue();
		ps.transform.position = GameCharacter.MovementComponent.CharacterCenter;
		ps.transform.LookAt(damageInitiator.MovementComponent.CharacterCenter.IgnoreAxis(EAxis.Z));

		newDir = Quaternion.LookRotation(damageInitiator.MovementComponent.CharacterCenter.IgnoreAxis(EAxis.YZ) - GameCharacter.MovementComponent.CharacterCenter.IgnoreAxis(EAxis.YZ), Vector3.up);

		CurrentParryValue += attackData.blockPoints;
	}

	public override void StartActionInHold()
	{
		RemoveParryBlockStates();
		StartAttack(attackData.counterAttack);
		GameCharacter.AnimController.InAttack = true;
		parryTimer.Stop();
		GameCharacter.StateMachine.RemoveLazyState(EGameCharacterState.DefensiveAction);
		didHitSomething = false;
	}

	public override void OnHit(GameObject hitObj)
	{
		DoDamage(hitObj, attackData.Damage * Unity.Mathematics.math.remap(0, 100, attackData.damageMinMultiplier, attackData.damageMaxMultiplier, CurrentParryValue));
		var character = hitObj.GetComponent<GameCharacter>();
		if (character != null)
		{
			didHitSomething = true;
		}
	}

	public override void ImplementUI()
	{
		PlayerGameCharacter player = (PlayerGameCharacter)GameCharacter;
		SpawnUIElement(player);
	}

	void OnUpdateHelperUpdate()
	{
		if (!isInterupted && !IsInterupted())
		{
			if (GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.Parry))
				parryTimer.Update(Time.deltaTime);
		}
	}

	public override void AttackPhaseEnd()
	{
		if (didHitSomething)
		{
			CurrentParryValue = 0;
			didHitSomething = false;
		}
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		GameCharacter.transform.rotation = (Quaternion.Lerp(GameCharacter.transform.rotation, newDir, Time.deltaTime * attackData.rotSpeed));
	}

	public override void RemoveUI()
	{
		if (ui != null)
			ui.gameObject.SetActive(false);
	}

	async void SpawnUIElement(PlayerGameCharacter player)
	{
		if (ui == null)
		{
			await new WaitUntil(() => player.PlayerUI != null);
			if (player.CombatComponent.CurrentWeapon != Weapon)
				return;
			GameObject spawnedUIElement = GameObject.Instantiate(attackData.uiELement, player.PlayerUI.transform);
			ui = spawnedUIElement.GetComponent<CounterAttackUI>();
			if (ui != null) ui.SetBarFillValue(Unity.Mathematics.math.remap(0, 100, 0, 1, CurrentParryValue));
		}
		else
		{
			if (ui != null)
			{
				ui.gameObject.SetActive(true);
				ui.SetBarFillValue(Unity.Mathematics.math.remap(0, 100, 0, 1, CurrentParryValue));
			}
		}
	
	}

	void OnParryTimerFinished()
	{
		if (!isInterupted && !IsInterupted())
		{
			GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Parry);
			GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Block);
			GameCharacter.AnimController.SetHoldAttack(attackData.defensiveActionHold);
			GameCharacter.AnimController.CrossFadeToNextState(GameCharacter.AnimController.HoldStateHash, 0.2f);
			GameCharacter.AnimController.InDefensiveAction = false;
			GameCharacter.AnimController.HoldAttack = true;
		}
	}

	private bool IsInterupted()
	{
		if (!GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.DefensiveActionHold))
		{
			ActionInterupted();
			return true;
		}
		return false;
	}

	public override void ActionInterupted()
	{
		isInterupted = true;
		GameCharacter.StateMachine.RemoveLazyState(EGameCharacterState.DefensiveAction);
		parryTimer.Stop();
		RemoveParryBlockStates();
		GameCharacter.RequestBestCharacterState();
	}

	void RemoveParryBlockStates()
	{
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Parry);
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Block);
		GameCharacter.AnimController.InDefensiveAction = false;
		GameCharacter.AnimController.HoldAttack = false;
	}

	public override bool HasAttackInputInHold()
	{
		return true;
	}

	public override bool HasUIImplementation()
	{
		return true;
	}

	public override ActionBase CreateCopy()
	{
		ParryDefensiveAction copy = new ParryDefensiveAction();
		copy.attackData = attackData;
		return copy;
	}
}
