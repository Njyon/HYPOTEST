using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public abstract class ActionBase
{
	public delegate void InitAction();

	GameCharacter gameCharacter;
	WeaponBase weapon;
	bool isActionInit = false;

	public GameCharacter GameCharacter { get { return gameCharacter; } }
	public WeaponBase Weapon { get { return weapon; } }

	public bool IsActionInit { get { return isActionInit; } }

	public ActionBase()
	{
		isActionInit = false;
	}

	/// <summary>
	/// Set Important Data on runtime
	/// </summary>
	/// <param name="gameCharacter"></param>
	/// <param name="weapon"></param>
	/// <param name="initAction"> Only Execute Once</param>
	public virtual void Init(GameCharacter gameCharacter, WeaponBase weapon, InitAction initAction = null)
	{
		if (!isActionInit || this.gameCharacter == null || this.weapon == null)
		{
			this.gameCharacter = gameCharacter;
			this.weapon = weapon;
			this.gameCharacter.onGameCharacterDied += OnGameCharacterDied;
			this.gameCharacter.onGameCharacterDestroyed += OnGameCharacterDestroyed;
			

			if (initAction != null)
			{
				initAction();
			}
			isActionInit = true;
		}
	}
	public abstract void StartAction();
	public abstract ActionBase CreateCopy();

	public virtual void StartActionInHold() { }
	public virtual void OnHit(GameObject hitObj) { }
	public virtual void StartAttackStateLogic() { }
	public virtual void PreAttackStateLogic(float deltaTime) { }
	public virtual void PostAttackStateLogic(float deltaTime) { }
	public virtual void EndAttackStateLogic() { }
	public virtual void AttackPhaseStart() { }
	public virtual void AttackPhaseEnd() { }
	public virtual void AttackRecoveryEnd() { }
	public virtual void DefensiveActionStateEnd() { }
	public virtual void CharacterArrivedAtRequestedLocation(GameCharacter movedCharacter) { }
	public virtual void CharacterMoveToAbort(GameCharacter movedCharacter) { }
	public virtual void CharacterMoveToEnd(GameCharacter movedCharacter) { }
	public virtual void DefensiveActionStart() { }
	public virtual void DefensiveActionEnd() { }
	public virtual void GroundReset() { }
	public virtual void ImplementUI() { }
	public virtual void RemoveUI() { }
	public virtual void SuccessfullParry(GameCharacter damageInitiator, float damage) { }
	public virtual void SuccessfullBlock(GameCharacter damageInitiator, float damage) { }
	/// <summary>
	/// <para> Everything that needs to be cleaned up or managed after use in the action should be managed here. Its safer that EndAttackState or other funktions </para>
	///
	/// <remarks> Gets Called when a newer action gets Set </remarks>
	/// <para> Can be called multiple times in a row </para>
	/// </summary>
	public virtual void ActionInterupted() { }
	public virtual void OnGameCharacterDied(GameCharacter gameCharacter) {
		if (this.gameCharacter != null) this.gameCharacter.onGameCharacterDied -= OnGameCharacterDied;
	}
	public virtual void OnGameCharacterDestroyed(GameCharacter gameCharacter)
	{
		if (this.gameCharacter != null) this.gameCharacter.onGameCharacterDestroyed -= OnGameCharacterDestroyed;
		if (this.gameCharacter != null) this.gameCharacter.onGameCharacterDied -= OnGameCharacterDied;
		isActionInit = false;
	}

	public virtual bool HasUIImplementation()
	{
		return false;
	}

	public virtual bool HasAttackInputInHold()
	{
		return false;
	}

	public virtual float MaxVerticalMovement()
	{
		return 0f;
	}

	public virtual float MaxHorizontallMovement()
	{
		return 0f;
	}

	public virtual float GetFreezTime()
	{
		return 1f;
	}

	public virtual float GetStopMovingRange()
	{
		return 1f;
	}

	public virtual float GetActionRanting()
	{
		return 1f;
	}

	public virtual float GetActionDischarge()
	{
		return 1f;
	}

	public virtual bool CanLeaveDefensiveState()
	{
		return true;
	}
}