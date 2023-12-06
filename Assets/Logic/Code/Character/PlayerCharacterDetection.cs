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
		player.onGameCharacterDied += OnPlayerDied;
		onPlayerEnterEvent.Invoke();
	}

	protected override void OnTriggerExitCall(PlayerGameCharacter player)
	{
		base.OnTriggerExitCall(player);
		player.onGameCharacterDied -= OnPlayerDied;
		onPlayerExitEvent.Invoke();
	}

	void OnPlayerDied(GameCharacter target)
	{
		PlayerGameCharacter player = (PlayerGameCharacter)target;
		if (player == null) return;
		if (onOverlapExit != null) onOverlapExit(player);
		DetectedGameCharacters.Remove(player);
		OnTriggerExitCall(player);
	}
}
