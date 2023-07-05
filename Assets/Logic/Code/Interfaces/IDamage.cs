using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage 
{
	public abstract void DoDamage(GameCharacter damageInitiator, float damage);
}
