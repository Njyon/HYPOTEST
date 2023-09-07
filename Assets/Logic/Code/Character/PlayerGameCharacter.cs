using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameCharacter : GameCharacter
{
	CombatRatingComponent combatRatingComponent;
	PlayerUI playerUI;

	public PlayerUI PlayerUI { get { return playerUI; } }

	protected override void Awake()
	{
		base.Awake();
		combatRatingComponent = new CombatRatingComponent(0);
		combatRatingComponent.Init(this);

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

	protected override void AddRatingOnHit()
	{
		base.AddRatingOnHit();
		combatRatingComponent.AddRatingOnHit();

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
}
