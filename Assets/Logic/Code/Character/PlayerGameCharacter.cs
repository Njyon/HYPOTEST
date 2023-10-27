using EasyButtons;
using System;
using System.Collections;
using System.Collections.Generic;
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
		combatRatingComponent.Init(this);

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
		base.OnDestroy();

	}

	protected override void OnDamaged(GameCharacter damageInitiator, float damage)
	{
		base.OnDamaged(damageInitiator, damage);
		combatRatingComponent.OnGotHit(damageInitiator, damage);
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

	[Button("Die")]
	protected override void DieButton()
	{
		base.DieButton();
	}
}
