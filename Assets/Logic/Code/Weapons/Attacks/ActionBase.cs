using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public abstract class ActionBase
{
	GameCharacter gameCharacter;
	WeaponBase weapon;

	public GameCharacter GameCharacter { get { return gameCharacter; } }
	public WeaponBase Weapon { get { return weapon; } }

	public ActionBase()
	{

	}

	public virtual void Init(GameCharacter gameCharacter, WeaponBase weapon)
	{
		this.gameCharacter = gameCharacter;
		this.weapon = weapon;
	}
	public abstract void StartAction();

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
	/// <summary>
	/// <para> Everything that needs to be cleaned up or managed after use in the action should be managed here. Its safer that EndAttackState or other funktions </para>
	///
	/// <remarks> Gets Called when a newer action gets Set </remarks>
	/// <para> Can be called multiple times in a row </para>
	/// </summary>
	public virtual void ActionInteruped() { }

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