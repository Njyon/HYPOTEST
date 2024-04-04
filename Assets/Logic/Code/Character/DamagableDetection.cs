using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableDetection : TargetDetection<IDamage>
{
	public List<GameCharacter> TargetGameCharacters = new List<GameCharacter>();

	protected override void OnTriggerEnterCall(IDamage target)
	{
		base.OnTriggerEnterCall(target);
		if (target.IsGameCharacter())
		{
			GameCharacter gc = target.GetGameCharacter();

			gc.onGameCharacterDied += OnPlayerDiedDestroyed;
			gc.onGameCharacterDestroyed += OnPlayerDiedDestroyed;

			TargetGameCharacters.Add(gc);
		}
	}

	protected override void OnTriggerExitCall(IDamage target)
	{
		base.OnTriggerExitCall(target);
		if (target.IsGameCharacter())
		{
			GameCharacter gc = target.GetGameCharacter();

			gc.onGameCharacterDied -= OnPlayerDiedDestroyed;
			gc.onGameCharacterDestroyed -= OnPlayerDiedDestroyed;

			TargetGameCharacters.Remove(gc);
		}
	}

	void OnPlayerDiedDestroyed(GameCharacter target)
	{
		if (target == null) return;
		OnTriggerExitCall(target);
	}
}
