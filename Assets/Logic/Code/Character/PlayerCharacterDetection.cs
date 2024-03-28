using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacterDetection : TargetDetection<PlayerGameCharacter>
{
	public UnityEvent onPlayerEnterEvent;
	public UnityEvent onPlayerExitEvent;
	public UltEvent<PlayerGameCharacter> onPlayerEnterEventObj;

	protected override void OnTriggerEnterCall(PlayerGameCharacter player)
	{
		base.OnTriggerEnterCall(player);
		player.onGameCharacterDied += OnPlayerDiedDestroyed;
		player.onGameCharacterDestroyed += OnPlayerDiedDestroyed;
		onPlayerEnterEvent.Invoke();
		onPlayerEnterEventObj.Invoke(player);
	}

	protected override void OnTriggerExitCall(PlayerGameCharacter player)
	{
		base.OnTriggerExitCall(player);
		player.onGameCharacterDied -= OnPlayerDiedDestroyed;
		player.onGameCharacterDestroyed -= OnPlayerDiedDestroyed;
		onPlayerExitEvent.Invoke();
	}

	void OnPlayerDiedDestroyed(GameCharacter target)
	{
		if (target == null) return;
		OnTriggerExitCall((PlayerGameCharacter)target);
	}
}
