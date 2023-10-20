using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearWeapon : WeaponBase
{
	//bool groundUpAttackhit = false;
	//bool groundUpAttackMove = false;
	//Ultra.Timer groundUpTimer = new Ultra.Timer();
	WeaponProjectile defensiveSpear = null;
	List<GameObject> thrownSpears = new List<GameObject>();

	public WeaponProjectile DefensiveSpear { get { return defensiveSpear; } set { defensiveSpear = value; } }


	public SpearWeapon() { }
	public SpearWeapon(GameCharacter gameCharacter, ScriptableWeapon weaponData) : base (gameCharacter, weaponData)
	{ }

    public override void EquipWeapon()
    {
        base.EquipWeapon();
	}

    public override void UnEquipWeapon()
    {
        base.UnEquipWeapon();

		GameCharacter.AnimController.InAimBlendTree = false;
		UnHookAllHookedCharacerts();
		GameCharacter.RequestBestCharacterState();
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
		SpawnedWeapon?.SetActive(true);

		//groundUpAttackhit = false;
		//groundUpAttackMove = false;

		if (defensiveSpear != null)
			GameObject.Destroy(defensiveSpear.gameObject);
	}

    public override void UpdateWeapon(float deltaTime)
    {
        base.UpdateWeapon(deltaTime);
    }

    public override AttackAnimationData GroundAttack(float attackDeltaTime)   
    {
      	return base.GroundAttack(attackDeltaTime);
    }
    public override AttackAnimationData GroundUpAttack(float attackDeltaTime)    
    {
		return base.GroundUpAttack(attackDeltaTime);
	}
    public override AttackAnimationData GroundDownAttack(float attackDeltaTime)  
    {
		return base.GroundDownAttack(attackDeltaTime);
	}
    public override AttackAnimationData GroundDirectionAttack(float attackDeltaTime)   
    {
        return base.GroundDirectionAttack(attackDeltaTime);
    }

    public override AttackAnimationData AirAttack(float attackDeltaTime)  
    {
        return base.AirAttack(attackDeltaTime);
    }
    public override AttackAnimationData AirUpAttack(float attackDeltaTime)
    {
        return base.AirUpAttack(attackDeltaTime);
    }
    public override AttackAnimationData AirDownAttack(float attackDeltaTime)  
    {
		return base.AirDownAttack(attackDeltaTime);
	}
    public override AttackAnimationData AirDirectionAttack(float attackDeltaTime) 
    {
        return base.AirDirectionAttack(attackDeltaTime);
	}

	public override void GroundAttackHit(GameObject hitObj)
	{
		base.GroundAttackHit(hitObj);
	}

	public override void GroundUpAttackHit(GameObject hitObj)
	{
		base.GroundAttackHit(hitObj);
	}

	public override void GroundDownAttackHit(GameObject hitObj)
	{
		base.GroundDownAttackHit(hitObj);
	}

	public override void GroundDirectionAttackHit(GameObject hitObj)
	{
		base.GroundDirectionAttackHit(hitObj);
	}

	public override void AirAttackHit(GameObject hitObj)
	{
		base.AirAttackHit(hitObj);
	}

	public override void AirUpAttackHit(GameObject hitObj)
	{
		base.AirUpAttackHit(hitObj);
	}

	public override void AirDownAttackHit(GameObject hitObj)
	{
		base.AirDownAttackHit(hitObj);
	}

	public override void AirDirectionAttackHit(GameObject hitObj)
	{
		base.AirDirectionAttackHit(hitObj);
	}

	public override void PostAttackStateLogic(float deltaTime)
	{
		base.PostAttackStateLogic(deltaTime);
	}

	public override void EndAttackStateLogic()
	{
		GameCharacter.MovementComponent.SetLayerToDefault();
		GameCharacter.MovementComponent.ResetCharacterCapsulToDefault();
		SpawnedWeapon.SetActive(true);

		UnHookAllHookedCharacerts();

		base.EndAttackStateLogic();
	}

	public override void AttackPhaseStart()
	{
		base.AttackPhaseEnd();
	}


	public override AttackAnimationData DefensiveAction()
	{
		AttackAnimationData returnData = null;
		base.DefensiveAction();
		//if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		//if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction.Count > 0) returnData = DefensiveActionAimLogic(ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction, EAnimationType.Default);
		//
		//
		//GameCharacter targetEnemy = Ultra.HypoUttilies.FindCharactereNearestToDirection(GameCharacter.MovementComponent.CharacterCenter, (GameCharacter.MovementInput.magnitude <= 0) ? GameCharacter.transform.forward : GameCharacter.MovementInput, ref GameCharacter.CharacterDetection.OverlappingGameCharacter);
		//if (targetEnemy == null) return null;
		//
		//
		//SpawnedWeapon.SetActive(false);
		//GameObject throwSpear = GameObject.Instantiate(GameAssets.Instance.ThrowSpear);
		//Vector3 spearDir = targetEnemy.MovementComponent.CharacterCenter - throwSpear.transform.position;
		//throwSpear.transform.position = new Vector3(SpawnedWeaponBones.transform.position.x, SpawnedWeaponBones.transform.position.y, 0);
		//throwSpear.transform.rotation = Quaternion.LookRotation(spearDir.normalized, Vector3.up);
		//throwSpear.transform.eulerAngles = new Vector3(throwSpear.transform.eulerAngles.x, throwSpear.transform.eulerAngles.y, 90f);
		//
		//GameCharacter.CombatComponent.AimCharacter = targetEnemy;
		//HookCharacterToCharacter(targetEnemy);
		//GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
		//
		//
		//defensiveSpear = throwSpear.GetComponent<WeaponProjectile>();
		//defensiveSpear.onProjectileHit += DefensiveActionHit;
		//defensiveSpear.Initialize(GameCharacter, throwSpear.transform.position, targetEnemy);

		return returnData;
	}

	//void DefensiveActionHit(GameObject hitObj)
	//{
	//	GameCharacter hitgameCharacter = hitObj.GetComponent<GameCharacter>();
	//	if (hitgameCharacter == null)
	//	{
	//		IDamage damageInterface = hitObj.GetComponent<IDamage>();
	//		if (damageInterface != null) damageInterface.DoDamage(GameCharacter, 0);
	//		return;
	//	}
	//
	//	GameCharacter.CombatComponent.AimCharacter = hitgameCharacter;
	//	HookCharacterToCharacter(hitgameCharacter);
	//	GameCharacter.CombatComponent.HookedCharacters.Add(hitgameCharacter);
	//	hitgameCharacter.CombatComponent.HookedToCharacter = GameCharacter;
	//	hitgameCharacter.CombatComponent.MoveToPosition = GameCharacter.transform.position + GameCharacter.transform.forward * 1f;
	//
	//	hitgameCharacter.StateMachine.RequestStateChange(EGameCharacterState.MoveToPosition);
	//	if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction.Count > 0) DefensiveActionAimLogic(ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction, EAnimationType.Trigger, false);
	//	
	//}

	//public override void CharacterArrivedAtRequestedLocation(GameCharacter movedCharacter)
	//{
	//	AfterDefensiveActionCleanUp();
	//}
	
	//public override void CharacterMoveToAbort(GameCharacter movedCharacter)
	//{
	//
	//	AfterDefensiveActionCleanUp();
	//}
	
	//public override void CharacterMoveToEnd(GameCharacter movedCharacter)
	//{
	//	AfterDefensiveActionCleanUp();
	//}
	
	//public override void DefensiveActionEnd()
	//{
	//	base.DefensiveActionEnd();
	//	AfterDefensiveActionCleanUp();
	//}
	
	//private void AfterDefensiveActionCleanUp()
	//{
	//	GameCharacter.AnimController.InAimBlendTree = false;
	//	UnHookAllHookedCharacerts();
	//	GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.Aim);
	//	GameCharacter.RequestBestCharacterState();
	//	SpawnedWeapon.SetActive(true);
	//	if (defensiveSpear != null)
	//		GameObject.Destroy(defensiveSpear.gameObject);
	//}
}