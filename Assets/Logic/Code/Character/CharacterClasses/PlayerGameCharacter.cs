using EasyButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class PlayerGameCharacter : GameCharacter
{
	CombatRatingComponent combatRatingComponent;
	PlayerUI playerUI;

	public PlayerUI PlayerUI { get { return playerUI; } }
	public CombatRatingComponent CombatRatingComponent { get { return combatRatingComponent; } }

	protected override void Awake()
	{
		base.Awake();
		if (!AIManager.Instance.IsBehaviorTreeStackInit) AIManager.Instance.InitBehaviorTreeStack();

		combatRatingComponent = new CombatRatingComponent(0);
		combatRatingComponent.onStyleRankingChanged += StyleRankingChanged;
		combatRatingComponent.Init(this);

		onGameCharacterAggroChanged += OnAggroChanged;

		if (!LoadingChecker.Instance.FinishLoading)
		{
			LoadingChecker.Instance.onLoadingFinished += FinsihLoading;
		}
		else
		{
			FinsihLoading();
		}
	}

	void FinsihLoading()
	{
		LoadingChecker.Instance.onLoadingFinished -= FinsihLoading;

		UIManager.Instance.onAllUIsUnloaded += OnAllUIsUnloaded;
		UIManager.Instance.UnloadAll();
	}

	public override void CustomAwake()
	{
		base.CustomAwake();
	}

	new protected void Update()
	{
		if (!IsInitialized) return;
		base.Update();
		combatRatingComponent?.Update(Time.deltaTime);
		Ultra.Utilities.Instance.DebugLogOnScreen("Current StyleRank => " + combatRatingComponent.CurrentValue, 0f, StringColor.Red);
	}

	new protected void OnDestroy()
	{
		if (combatRatingComponent != null) combatRatingComponent.onStyleRankingChanged -= StyleRankingChanged;
		onGameCharacterAggroChanged -= OnAggroChanged;
		base.OnDestroy();

	}

	protected override void OnDamaged(GameCharacter damageInitiator, float damage, bool removeCharge)
	{
		base.OnDamaged(damageInitiator, damage, removeCharge);
		combatRatingComponent.OnGotHit(damageInitiator, damage, removeCharge);
	}

	protected override void AddRatingOnHit(float damage)
	{
		base.AddRatingOnHit(damage);
		combatRatingComponent.AddRatingOnHit(damage);

	}

	void OnAllUIsUnloaded()
	{
		UIManager.Instance.onAllUIsUnloaded -= OnAllUIsUnloaded;
		UIManager.Instance.LoadPlayerUI(OnPlayerUILoaded);
	}

	void OnPlayerUILoaded()
	{
		playerUI = FindObjectOfType<PlayerUI>();
		if (playerUI != null)
			playerUI.Init(this);
	}

	void StyleRankingChanged(int newRankIndex, int oldRankIndex)
	{
		if (!gameObject.activeSelf) return;
		MusicManager.Instance.SetVolumeTarget(combatRatingComponent.StyleRanks[newRankIndex].musicVolumeTarget);
	}

	void OnAggroChanged()
	{
		if (!gameObject.activeSelf) return;
		if (CharacterHasAggro)
		{
			MusicManager.Instance.Play();
			MusicManager.Instance.SetVolumeTarget(combatRatingComponent.StyleRanks[combatRatingComponent.CurrentStyleRankIndex].musicVolumeTarget);
		}
		else
		{
			MusicManager.Instance.Stop();
		}
	}

	protected override void SuccsessfullDodge(GameCharacter damageInitiator, float damage)
	{
		base.SuccsessfullDodge(damageInitiator, damage);
		combatRatingComponent.AddRatingByAvoidingDamage(damage);
	}

	protected override void SuccessfullParry(GameCharacter damageInitiator, float damage)
	{
		base.SuccessfullParry(damageInitiator, damage);
		combatRatingComponent.AddRatingByAvoidingDamage(damage);
	}

	public override void RespawnCharacter()
	{
		transform.position = RespawnObj.transform.position;
		Health.Reset();

		foreach (Rigidbody rBody in RigDataComponent.RegdollRigidBodys)
		{
			rBody.useGravity = false;
			rBody.isKinematic = true;
		}
		foreach (Collider collider in RigDataComponent.Colliders)
		{
			collider.enabled = false;
		}

		IsGameCharacterDead = false;
		Animator.enabled = true;
		MovementComponent.CapsuleCollider.enabled = true;
		MovementComponent.UnityMovementController.enabled = true;

		AnimController.ResetAnimStatesHARD();
		AnimController.ResetAnimIK_HARD();

		foreach (IPluginState<EPluginCharacterState> state in PluginStateMachine.DictionaryOfPluginStates.Values.ToArray())
		{
			PluginStateMachine.RemovePluginState(state.GetStateType());
		}

		ApplyDefaultPluginStates();

		StateMachine.RequestStateChange(EGameCharacterState.Standing, true);

		if (onGameCharacterRespawnes != null) onGameCharacterRespawnes(this);
	}

	[Button("Die")]
	protected override void DieButton()
	{
		base.DieButton();
	}
}
