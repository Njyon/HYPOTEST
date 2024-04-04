using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGameCharacterComp : MonoBehaviour
{
	public void KillGameCharacter(GameCharacter gameCharacter)
	{
		gameCharacter.Health.AddCurrentValue(-gameCharacter.Health.CurrentValue);
	}
}
