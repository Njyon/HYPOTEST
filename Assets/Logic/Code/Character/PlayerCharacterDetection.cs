using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacterDetection : CharacterDetection<PlayerGameCharacter>
{
	public UnityEvent onPlayerEnterEvent;
	public UnityEvent onPlayerExitEvent;

	protected override void OnTriggerEnterCall(PlayerGameCharacter player)
	{
		base.OnTriggerEnterCall(player);
		onPlayerEnterEvent.Invoke();
	}

	protected override void OnTriggerExitCall(PlayerGameCharacter player)
	{
		base.OnTriggerExitCall(player);
		onPlayerExitEvent.Invoke();
	}
}
