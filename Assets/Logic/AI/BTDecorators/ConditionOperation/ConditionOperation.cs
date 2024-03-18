using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class ConditionOperation
{
	GameCharacter gameCharacter;
	GameCharacter target;

	protected GameCharacter GameCharacter {  get { return gameCharacter; } }
	protected GameCharacter Target {  get { return target; } }

	public virtual void Init(GameCharacter character, GameCharacter target)
	{
		this.gameCharacter = character;
		this.target = target;
	}

	public abstract bool DoOperation();
}
