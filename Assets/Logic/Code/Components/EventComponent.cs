using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	

	public EventComponent()
	{
		toBeEveluatedEvent = null;
		eveluatedEvent = null;
	}

	public void AddEvent(CharacterEvent newEvent)
	{
		toBeEveluatedEvent = newEvent;
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
				toBeEveluatedEvent = null;
			}
			else if (toBeEveluatedEvent.time <= 0)
			{
				toBeEveluatedEvent = null;
			}
		}
	}
}
