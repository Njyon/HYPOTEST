using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameCharacterEvent
{
	Jump,
	WeaponSwitchEvent,
	Attack,
	DefensiveAction,
	Dodge,
}

[SerializeField]
public abstract class CharacterEvent
{
	public float time;
	public GameCharacter gameCharacter;
	public float inputTime;

	public CharacterEvent(GameCharacter gameCharacter, float time = 0.2f)
	{
		this.time = time;
		this.gameCharacter = gameCharacter;
		inputTime = Time.time;
	}

	public abstract EGameCharacterEvent GetCharacterEvenetType();
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
	
	List<CharacterEvent> holdEvents = new();
	public List<CharacterEvent> previousEventsOverTimeFrame = new List<CharacterEvent>();
	float timeframeOfList = 5f;

	public EventComponent()
	{
		toBeEveluatedEvent = null;
		eveluatedEvent = null;
	}

	public void AddEvent(CharacterEvent newEvent)
	{
		toBeEveluatedEvent = newEvent;
		previousEventsOverTimeFrame.Add(newEvent);
		ManagePreviousEvents();
		//Ultra.Utilities.Instance.DebugLogOnScreen("New Event Added! " + newEvent.time.ToString(), 2f, StringColor.Lightblue, 100, DebugAreas.Combat);
	}

	public void AddHoldEvent(CharacterEvent newHoldEvent)
	{
		holdEvents.Add(newHoldEvent);
	}

	public void RemoveHoldEvent(CharacterEvent HoldEvent)
	{
		if (holdEvents.Contains(HoldEvent))
		{
			holdEvents.Remove(HoldEvent);
		}
	}

	public void Update(float deltaTime)
	{
		ManageHoldEvents(deltaTime);
		if (holdEvents.Count > 0)
		{
			for (int i = holdEvents.Count - 1; i >= 0; i--)
			{
				CharacterEvent holdEvent = holdEvents[i];
				if (holdEvent.time <= 0 && holdEvent.CanBeExecuted())
				{
					holdEvent.StartEvent();
					if (onCharacterEventTriggered != null) onCharacterEventTriggered(holdEvent.GetCharacterEvenetType());
					RemoveHoldEvent(holdEvent);
					return;
				}
			}
		}
		ExecuteEventIfPossible(deltaTime);
	}

	private void ManagePreviousEvents()
	{
		float currentTime = Time.time;
		List<CharacterEvent> deleteEvents = new List<CharacterEvent>();
		foreach (CharacterEvent characterEvent in previousEventsOverTimeFrame)
		{
			float time = currentTime - characterEvent.inputTime;
			if (time >= timeframeOfList) deleteEvents.Add(characterEvent);
		}
		foreach (CharacterEvent characterEvent in deleteEvents)
		{
			previousEventsOverTimeFrame.Remove(characterEvent);
		}
	}

	void ExecuteEventIfPossible(float deltaTime)
	{
		if (toBeEveluatedEvent != null)
		{
			toBeEveluatedEvent.time -= deltaTime;
			if (toBeEveluatedEvent.CanBeExecuted())
			{
				eveluatedEvent = toBeEveluatedEvent;
				eveluatedEvent.StartEvent();
				if (onCharacterEventTriggered != null) onCharacterEventTriggered(eveluatedEvent.GetCharacterEvenetType());
				toBeEveluatedEvent = null;
				ManagePreviousEvents();
			}
			else if (toBeEveluatedEvent.time <= 0)
			{
				//Ultra.Utilities.Instance.DebugLogOnScreen("RequestGone! " + ToBeEveluatedEvent.GetType().Name, 2f, StringColor.Lightblue, 100, DebugAreas.Combat);
				toBeEveluatedEvent = null;
			}
		}
	}

	public void ManageHoldEvents(float deltaTime)
	{
		List<CharacterEvent> removeHolds = new();
		foreach (CharacterEvent holdEvent in holdEvents)
		{
			if (holdEvent == null)
			{
				removeHolds.Add(holdEvent);
				continue;
			}
			holdEvent.time -= deltaTime;
		}
		foreach (CharacterEvent holdEvent in removeHolds)
		{
			RemoveHoldEvent(holdEvent);
		}
	}
}
