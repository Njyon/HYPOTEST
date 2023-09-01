using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameCharacter : GameCharacter
{
	CombatRatingComponent combatRatingComponent;

	protected override void Awake()
	{
		base.Awake();
		combatRatingComponent = new CombatRatingComponent(0);
		combatRatingComponent.Init(this);
	}

	public override void CustomAwake()
	{
		base.CustomAwake();

	}

	new protected void Update()
	{
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

}
