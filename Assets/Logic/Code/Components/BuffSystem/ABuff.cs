using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ABuff
{
	public delegate void BuffEvent(ABuff buff);
	public BuffEvent onBuffFinished;

	Ultra.Timer durationTimer;
	GameCharacter gameCharacter;

	public Ultra.Timer DurationTimer { get { return durationTimer; } }
	public GameCharacter GameCharacter { get { return gameCharacter; } }	

	public GUID ID { get; private set; }
	public bool IsActive { get; private set; }

	public ABuff(GameCharacter gameCharacter ,float duration) 
	{ 
		this.gameCharacter = gameCharacter;
		this.durationTimer = new Ultra.Timer(duration, duration < 0 ? true : false);
		durationTimer.onTimerFinished += OnBuffTimerFinished;
		ID = GUID.Generate();
		IsActive = true;
	}

	public virtual void Update(float deltaTime)
	{
		if (durationTimer != null) durationTimer.Update(deltaTime);
	}

	public virtual void LateUpdate(float deltaTime) { }
	public virtual void FixedUpdate(float deltaTime) { }

	/// <summary>
	/// Get the Type of the buff, needs type for unique ID
	/// </summary>
	/// <returns></returns>
	public abstract EBuff GetBuffType();
	/// <summary>
	/// Buff Clean up! Remove All subscriptions etc here!
	/// </summary>
	public abstract void BuffEnds();

	public void INTERN_BuffEnds()
	{
		BuffEnds();
		IsActive = false;
	}

	void OnBuffTimerFinished()
	{
		if (durationTimer != null) durationTimer.onTimerFinished -= OnBuffTimerFinished;
		if (onBuffFinished != null) onBuffFinished(this);
	}
}
