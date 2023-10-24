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

	public override void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction initAction = null)
	{
		base.Init(gameCharacter, weapon, () => {
			if (updateHelper == null)
			{
				GameObject go = new GameObject(GameCharacter.name + " UpdateHelper | ParryAction");
				updateHelper = go.AddComponent<UpdateHelper>();
				updateHelper.onUpdate += OnUpdateHelperUpdate;
			}
			parryTimer = new Ultra.Timer();
			parryTimer.onTimerFinished += OnParryTimerFinished;

			parryEffectPool = new ParticleSystemPool(attackData.parryParticleEffect, new GameObject(gameCharacter.name + " ParryEffect Holder"));
			blockEffectPool = new ParticleSystemPool(attackData.blockParticleEffect, new GameObject(gameCharacter.name + " BlockEffect Holder"));
		});
	}

	public override void StartAction()
	{
		isInterupted = false;
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Parry);
		parryTimer.Start(attackData.parryTime);
		GameCharacter.StateMachine.AddLazyState(EGameCharacterState.DefensiveAction);
		Weapon.AttackAnimType = EAttackAnimType.Default;
		GameCharacter.AnimController.SetDefensiveAction(attackData.defensiveAction);

		Debug.Log("StartAction");
	}

	public override void SuccessfullParry(GameCharacter damageInitiator, float damage)
	{
		ParticleSystem ps = parryEffectPool.GetValue();
		ps.transform.position = GameCharacter.MovementComponent.CharacterCenter;
		ps.transform.LookAt(damageInitiator.MovementComponent.CharacterCenter.IgnoreAxis(EAxis.Z));
	}


	public override void SuccessfullBlock(GameCharacter damageInitiator, float damage)
	{
		ParticleSystem ps = blockEffectPool.GetValue();
		ps.transform.position = GameCharacter.MovementComponent.CharacterCenter;
		ps.transform.LookAt(damageInitiator.MovementComponent.CharacterCenter.IgnoreAxis(EAxis.Z));
	}

	public override void StartActionInHold()
	{
		RemoveParryBlockStates();
		StartAttack(attackData.counterAttack);

		Debug.Log("StartActionInHold");
	}

	public override void OnHit(GameObject hitObj)
	{
		DoDamage(hitObj, attackData.Damage);
	}

	public override void ImplementUI()
	{
		PlayerGameCharacter player = (PlayerGameCharacter)GameCharacter;
		SpawnUIElement(player);

		Debug.Log("ImplementUI");
	}

	void OnUpdateHelperUpdate()
	{
		if (!isInterupted && !IsInterupted())
		{
			if (GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.Parry))
				parryTimer.Update(Time.deltaTime);
		}
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
		}
		else
		{
			ui.gameObject.SetActive(true);
		}
	
	}

	void OnParryTimerFinished()
	{
		if (!isInterupted && !IsInterupted())
		{
			GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Parry);
			GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Block);
			GameCharacter.AnimController.SetHoldAttack(attackData.defensiveActionHold);
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
