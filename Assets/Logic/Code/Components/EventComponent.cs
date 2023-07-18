using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameCharacterEvent
{
	Jump,
	WeaponSwitchEvent,
	Attack,
}

[SerializeField]
public abstract class CharacterEvent
{
	public float time;
	public GameCharacter gameCharacter;

	public CharacterEvent(GameCharacter gameCharacter, float time = 0.2f)
	{
		this.time = time;
		this.gameCharacter = gameCharacter;
	}

	public abstract EGameCharacterEvent GetGameCharacterEvenetType();
	public abstract bool CanBeExecuted();
	public abstract void StartEvent();
}

public class EventComponent
{
	/// <summary>
	/// We are only interessted in the newest input event
	/// </summary>
	CharacterEvent toBeEveluatedEvent;
	CharacterEvent eveluatedEvent;

	public CharacterEvent ToBeEveluatedEvent { get { return toBeEveluatedEvent; } }
	public CharacterEvent EveluatedEvent { get { return eveluatedEvent; } }

	public delegate void OnCharacterEventTriggered(EGameCharacterEvent type);
	public OnCharacterEventTriggered onCharacterEventTriggered;
	

	public EventComponent()
	{
		toBeEveluatedEvent = null;
		eveluatedEvent = null;
	}

	public void AddEvent(CharacterEvent newEvent)
	{
		toBeEveluatedEvent = newEvent;
		//Ultra.Utilities.Instance.DebugLogOnScreen("New Event Added! " + newEvent.time.ToString(), 2f, StringColor.Lightblue, 100, DebugAreas.Combat);
	}

	public void Update(float deltaTime)
	{
		if (toBeEveluatedEvent != null)
		{
			toBeEveluatedEvent.time -= deltaTime;
			if (toBeEveluatedEvent.CanBeExecuted())
			{
				eveluatedEvent = toBeEveluatedEvent;
				eveluatedEvent.StartEvent();
				if (onCharacterEventTriggered != null) onCharacterEventTriggered(eveluatedEvent.GetGameCharacterEvenetType());
				toBeEveluatedEvent = null;
			}
			else if (toBeEveluatedEvent.time <= 0)
			{

				//Ultra.Utilities.Instance.DebugLogOnScreen("RequestGone! " + ToBeEveluatedEvent.GetType().Name, 2f, StringColor.Lightblue, 100, DebugAreas.Combat);
				toBeEveluatedEvent = null;
			}
		}
	}
}
