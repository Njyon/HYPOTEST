using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//public interface IAttack 
//{
//	public void StartAttack();
//	public void OnHit(GameObject hitObj);
//}

[Serializable]
public class ActionData
{
	public ActionData()
	{

	}
}

[Serializable]
public class AttackData : ActionData
{
	public AttackData()
	{

	}

	public float Damage = 10f;
	public float Rating = 100f;
	public float Discharge = 100f;
}

//[Serializable]
//public abstract class BaseAttack : ScriptableObject, IAttack
//{
//	GameCharacter gameCharacter;
//	WeaponBase weapon;
//
//	public GameCharacter GameCharacter { get { return gameCharacter; } }
//	public WeaponBase Weapon { get {  return weapon; } }
//
//	public BaseAttack()
//	{
//
//	}
//
//	public virtual void Init(GameCharacter gameCharacter, WeaponBase weapon)
//	{
//		this.gameCharacter = gameCharacter;
//		this.weapon = weapon;
//	}
//	public abstract void StartAttack();
//	public abstract void OnHit(GameObject hitObj);
//	public abstract Type GetAttackDataType();
//	public abstract void SetData(AttackData data);
//
//	public virtual void StartAttackStateLogic()
//	{
//
//	}
//
//	public virtual void PreAttackStateLogic(float deltaTime)
//	{
//
//	}
//
//	public virtual void PostAttackStateLogic(float deltaTime)
//	{
//
//	}
//
//	public virtual void EndAttackStateLogic()
//	{
//
//	}
//
//	public virtual void AttackPhaseStart()
//	{
//
//	}
//
//	public virtual void AttackPhaseEnd()
//	{
//
//	}
//
//	public virtual void AttackRecoveryEnd()
//	{
//
//	}
//
//	public virtual void DefensiveActionStateEnd()
//	{
//
//	}
//
//	public virtual void CharacterArrivedAtRequestedLocation(GameCharacter movedCharacter)
//	{
//
//	}
//
//	public virtual void CharacterMoveToAbort(GameCharacter movedCharacter)
//	{
//
//	}
//
//	public virtual void CharacterMoveToEnd(GameCharacter movedCharacter)
//	{
//
//	}
//
//	public virtual void DefensiveActionStart()
//	{
//
//	}
//
//	public virtual void DefensiveActionEnd()
//	{
//
//	}
//
//	public virtual void GroundReset()
//	{
//
//	}
//
//	public virtual bool CanLeaveDefensiveState()
//	{
//		return true;
//	}
//}
