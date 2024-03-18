using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage 
{
	public abstract void DoDamage(GameCharacter damageInitiator, float damage, bool shouldStagger = true, bool removeCharge = true, bool shouldFreezGame = true);
	public abstract HyppoliteTeam GetTeam();
	public abstract bool IsGameCharacter();
	public abstract GameCharacter GetGameCharacter();
	public abstract Vector3 GetPosition();
}