using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamagableObject : MonoBehaviour, IDamage
{
	public bool toggable = true;
	[ConditionalField("toggable")]
	public bool switchable = false;
	[ConditionalField(new[] { nameof(toggable), nameof(switchable) }, new[] { false, true })]
	public float toggleTime = 1f;

	public UltEvents.UltEvent onGotDamagedEvent;
	[ConditionalField("switchable")]
	public UltEvents.UltEvent onSwitchOffEvent;
	[ConditionalField(new[] { nameof(toggable), nameof(switchable) }, new[] { false, true })]
	public UltEvents.UltEvent onAfterToggleTime;

	Ultra.Timer toggleTimer;
	bool isSwitchedOn = false;

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
		if (toggable && !switchable)
		{
			toggleTimer.Start();
		}
		else if (toggable && switchable) 
		{
			isSwitchedOn = !isSwitchedOn;

			if (isSwitchedOn)
				if (onGotDamagedEvent != null) onGotDamagedEvent.Invoke();
			else 
				if (onSwitchOffEvent != null) onSwitchOffEvent.Invoke();

			return;
		}

		if (onGotDamagedEvent != null) onGotDamagedEvent.Invoke();
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
		if (toggable && !switchable)
		{
			if (onAfterToggleTime != null) onAfterToggleTime.Invoke();
		}
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public bool CanBeDamaged()
	{
		return true;
	}
}
