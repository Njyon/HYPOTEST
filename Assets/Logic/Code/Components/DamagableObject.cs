using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamagableObject : MonoBehaviour, IDamage
{
	public delegate void ObjectEvent();
	public ObjectEvent onGotDamaged;
	public ObjectEvent onToggleEvent;

	public bool toggable = false;
	[ConditionalField("toggable", inverse:true)]
	public bool switchable = false;
	[ConditionalField(new[] { nameof(toggable), nameof(switchable) }, new[] { true, true })]
	public float toggleTime = 1f;

	public UnityEvent onGotDamagedEvent;
	public UnityEvent onAfterToggleTime;

	Ultra.Timer toggleTimer;

	void Awake()
	{
		toggleTimer = new Ultra.Timer(toggleTime);
		toggleTimer.onTimerFinished += OnToggleTimerFinished;
	}

	void Update()
	{
		if (toggleTimer.IsRunning)
			toggleTimer.Update(Time.deltaTime);
	}

	public void DoDamage(GameCharacter damageInitiator, float damage, bool shouldStagger = true, bool removeCharge = true, bool shouldFreezGame = true)
	{
		if (onGotDamagedEvent != null) onGotDamagedEvent.Invoke();
		if (onGotDamaged != null) onGotDamaged();

		if (toggable && !switchable)
		{
			toggleTimer.Start();
		}
	}

	public GameCharacter GetGameCharacter()
	{
		return null;
	}

	public HyppoliteTeam GetTeam()
	{
		return HyppoliteTeam.None;
	}

	public bool IsGameCharacter()
	{
		return false;
	}

	void OnToggleTimerFinished()
	{
		if (onAfterToggleTime != null) onAfterToggleTime.Invoke();
		if (onToggleEvent != null) onToggleEvent();
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}
}
