using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public enum EExplicitAttackType
{
    Unknown,
    GroundedDefaultAttack,
    GroundedDirectionalAttack,
    GroundedUpAttack,
    GroundedDownAttack,
    AirDefaultAttack,
    AirDirectionalAttack,
    AirUpAttack,
    AirDownAttack,
	DefensiveAction,
}

public enum EAnimationType
{
	Default,
	Trigger,
	Hold
}

public enum EAttackAnimType
{
	Default,
	AimBlendSpace,
	Combat3Blend,
}

public abstract class WeaponBase
{
	public delegate void OnChargeValueChanged(float newCharge, float oldCharge);
	public OnChargeValueChanged onChargeValueChanged;

	GameCharacter gameCharacter;
    ScriptableWeapon weaponData;
    GameObject spawnedWeapon;
    GameObject spawnedWeaponBones;
	EExplicitAttackType currentAttackType;
	EExplicitAttackType lastAttackType;
    int attackIndex;
    int defensiveActionIndex;
	bool ishitDetecting = false;
	AttackAnimationData currentAttack;
	AttackAnimationData currentDefensiveAction;
	protected List<GameObject> hitObjects;
	EAttackAnimType attackAnimType;
	int comboIndexInSameAttack;
	float charge = 0;
	float chargeAfterTime = 0;
	Ultra.Timer maxChargeAfterEquipTimer;

	// Particle Save
	List<List<ParticleSystem>> groundLightAttackParticleList;
	List<List<ParticleSystem>> groundHeavyAttackParticleList;
	List<List<ParticleSystem>> groundUpAttackParticleList;
	List<List<ParticleSystem>> groundDownAttackParticleList;
	List<List<ParticleSystem>> airLightAttackParticleList;
	List<List<ParticleSystem>> airHeavyAttackParticleList;
	List<List<ParticleSystem>> airUpAttackParticleList;
	List<List<ParticleSystem>> airDownAttackParticleList;
	List<List<ParticleSystem>> defensiveActionParticleList;

	public GameCharacter GameCharacter { get { return gameCharacter; } }
    public ScriptableWeapon WeaponData { get { return weaponData; } }
    public GameObject SpawnedWeapon { get { return spawnedWeapon; } }
    public GameObject SpawnedWeaponBones { get { return spawnedWeaponBones; } }
	public bool IsHitDetecting { get { return ishitDetecting; } }
	public AttackAnimationData CurrentAttack { get { return currentAttack; } }
	public AttackAnimationData CurrentDefensiveAction { get { return currentDefensiveAction; } }
	public EExplicitAttackType CurrentAttackType { get { return currentAttackType; } }
	public EExplicitAttackType LastAttackType { get { return lastAttackType; } }
	public EAttackAnimType AttackAnimType { get { return attackAnimType; } set { attackAnimType = value; } }
	public int ComboIndexInSameAttack { get { return comboIndexInSameAttack; } }
	public List<GameObject> HitObjects { get { return hitObjects; } }	
	public int AttackIndex { get { return attackIndex; } }
	public float Charge { 
		get { return charge; } 
		set { 
			value = Mathf.Clamp(value, 0, weaponData.MaxChargeAmount);
			if (!MaxChargeAfterEquipTimer.IsRunning)
			{
				float oldChargeValue = charge;
				charge = value; 

				if (onChargeValueChanged != null) onChargeValueChanged(charge, oldChargeValue);
			}
			chargeAfterTime = value;
		} 
	}
	public Ultra.Timer MaxChargeAfterEquipTimer { get { return maxChargeAfterEquipTimer; } }

	public WeaponBase() { }
    public WeaponBase(GameCharacter gameCharacter, ScriptableWeapon weaponData)
    {
        this.gameCharacter = gameCharacter;
        this.weaponData = weaponData;
		hitObjects = new List<GameObject>();
		maxChargeAfterEquipTimer = new Ultra.Timer();
	}

	public virtual void InitWeapon()
	{
		if (!weaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;

		groundLightAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref weaponData.AnimationData[GameCharacter.CharacterData.Name].GroundAttacks, ref groundLightAttackParticleList);
		groundHeavyAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref weaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDirectionAttacks, ref groundHeavyAttackParticleList);
		groundUpAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref weaponData.AnimationData[GameCharacter.CharacterData.Name].GroundUpAttacks, ref groundUpAttackParticleList);
		groundDownAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref weaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDownAttacks, ref groundDownAttackParticleList);
		airLightAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref weaponData.AnimationData[GameCharacter.CharacterData.Name].AirAttacks, ref airLightAttackParticleList);
		airHeavyAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref weaponData.AnimationData[GameCharacter.CharacterData.Name].AirDirectionAttacks, ref airHeavyAttackParticleList);
		airUpAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref weaponData.AnimationData[GameCharacter.CharacterData.Name].AirUpAttacks, ref airUpAttackParticleList);
		airDownAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref weaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks, ref airDownAttackParticleList);
		defensiveActionParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref weaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction, ref defensiveActionParticleList);
	}

    public virtual void EquipWeapon()
	{
		SetUpWeaponAnimationData();
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.WeaponReady);
		SpawnVisualWeaponMesh();
		SetUpHitDetectionMeshLogic();
		gameCharacter.StateMachine.onStateChanged += OnGameCharacterStateChange;
		defensiveActionIndex = 0;
		if (Charge >= weaponData.MaxChargeAmount)
		{
			MaxChargeAfterEquipTimer.Start(weaponData.TimeAfterEqupingMaxChargedWeapon);
			MaxChargeAfterEquipTimer.onTimerFinished += OnMaxChargeAfterEquipTimerFinished;
		}

	}

	public virtual void UnEquipWeapon()
	{
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.WeaponReady);
		MaxChargeAfterEquipTimer.onTimerFinished -= OnMaxChargeAfterEquipTimerFinished;
		if (MaxChargeAfterEquipTimer.IsRunning)
		{
			MaxChargeAfterEquipTimer.Stop();
			OnMaxChargeAfterEquipTimerFinished();
		}
		GameObject.Destroy(spawnedWeapon);
		GameObject.Destroy(spawnedWeaponBones);
		if (gameCharacter.CombatComponent.HitDetectionColliderScript != null) gameCharacter.CombatComponent.HitDetectionColliderScript.onOverlapEnter -= WeaponColliderEnter;
		if (gameCharacter.CombatComponent.HitDetectionColliderScript != null) gameCharacter.CombatComponent.HitDetectionColliderScript.onOverlapExit -= WeaponColliderExit;

	}

	private void SpawnVisualWeaponMesh()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;

		if (WeaponData.WeaponMeshData.cascadeurSetUp)
		{
			switch (WeaponData.AnimationData[GameCharacter.CharacterData.Name].HandType)
			{
				case EWeaponHandType.RightHand:
					if (weaponData.WeaponMeshData.WeaponMesh == null) break;
					spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.transform);
					spawnedWeaponBones = spawnedWeapon.transform.GetChild(0).gameObject;
					spawnedWeapon.transform.GetChild(0).transform.parent = gameCharacter.GameCharacterData.HandROnjectPoint;
					spawnedWeaponBones.transform.position = Vector3.zero;
					spawnedWeaponBones.transform.rotation = Quaternion.identity;
					spawnedWeaponBones.transform.localScale = Vector3.one;
					spawnedWeaponBones.transform.localPosition = Vector3.zero;
					spawnedWeaponBones.transform.localRotation = Quaternion.identity;
					spawnedWeaponBones.transform.Translate(WeaponData.WeaponMeshData.WeaponOffset, Space.Self);
					spawnedWeaponBones.transform.localRotation = Quaternion.Euler(WeaponData.WeaponMeshData.WeaponRotationEuler);
					spawnedWeaponBones.transform.localScale = WeaponData.WeaponMeshData.WeaponScale;
					break;
				case EWeaponHandType.LeftHand:
					if (weaponData.WeaponMeshData.WeaponMesh == null) break;
					spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.transform);
					spawnedWeaponBones = spawnedWeapon.transform.GetChild(0).gameObject;
					spawnedWeapon.transform.GetChild(0).transform.parent = gameCharacter.GameCharacterData.HandLOnjectPoint;
					spawnedWeaponBones.transform.position = Vector3.zero;
					spawnedWeaponBones.transform.rotation = Quaternion.identity;
					spawnedWeaponBones.transform.localScale = Vector3.one;
					spawnedWeaponBones.transform.localPosition = Vector3.zero;
					spawnedWeaponBones.transform.localRotation = Quaternion.identity;
					spawnedWeaponBones.transform.Translate(WeaponData.WeaponMeshData.WeaponOffset, Space.Self);
					spawnedWeaponBones.transform.localRotation = Quaternion.Euler(WeaponData.WeaponMeshData.WeaponRotationEuler);
					spawnedWeaponBones.transform.localScale = WeaponData.WeaponMeshData.WeaponScale;
					break;
				default:
					break;
			}
		}
		else
		{
			switch (WeaponData.AnimationData[GameCharacter.CharacterData.Name].HandType)
			{
				case EWeaponHandType.RightHand:
					if (weaponData.WeaponMeshData.WeaponMesh == null) break;
					spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.GameCharacterData.HandROnjectPoint);
					spawnedWeapon.transform.Translate(WeaponData.WeaponMeshData.WeaponOffset, Space.Self);
					spawnedWeapon.transform.rotation = Quaternion.Euler(spawnedWeapon.transform.rotation.eulerAngles + WeaponData.WeaponMeshData.WeaponRotationEuler);
					spawnedWeapon.transform.localScale = WeaponData.WeaponMeshData.WeaponScale;
					break;
				case EWeaponHandType.LeftHand:
					if (weaponData.WeaponMeshData.WeaponMesh == null) break;
					spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.GameCharacterData.HandLOnjectPoint);
					spawnedWeapon.transform.Translate(WeaponData.WeaponMeshData.WeaponOffset, Space.Self);
					spawnedWeapon.transform.rotation = Quaternion.Euler(spawnedWeapon.transform.rotation.eulerAngles + WeaponData.WeaponMeshData.WeaponRotationEuler);
					spawnedWeapon.transform.localScale = WeaponData.WeaponMeshData.WeaponScale;
					break;
				case EWeaponHandType.NoHands:
					if (weaponData.WeaponMeshData.WeaponMesh == null) break;
					spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.transform);
					break;
				default:
					break;
			}
		}
	}

	private void SetUpHitDetectionMeshLogic()
	{
		if (gameCharacter.CombatComponent.HitDetectionGameObject == null) gameCharacter.CombatComponent.SetUpHitDetection();
		gameCharacter.CombatComponent.HitDetectionColliderScript.onOverlapEnter += WeaponColliderEnter;
		gameCharacter.CombatComponent.HitDetectionColliderScript.onOverlapExit += WeaponColliderExit;
	}

    public virtual void UpdateWeapon(float deltaTime)
	{
		MaxChargeAfterEquipTimer.Update(deltaTime);

		HitDetection();
	}

	private void HitDetection()
	{
		if (!ishitDetecting) return;
		if (currentAttack == null) return;

		switch (currentAttack.data.hitDetectionType)
		{
			case EHitDetectionType.Mesh:
				// We Dont need to do anything here because the hitDetectionObject does the job
				break;
			case EHitDetectionType.Box:
				{
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.Translate(currentAttack.data.offset);
					Collider[] colliders = Physics.OverlapBox(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, currentAttack.data.boxDimensions / 2);
					foreach (Collider collider in colliders)
					{
						WeaponColliderEnter(collider);
					}
					//Ultra.Utilities.DrawBox(hitDetectionGameObject.transform.position, Quaternion.identity, lastAttack.data.boxDimensions, Color.red, 200);
					Ultra.Utilities.DrawBox(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, Quaternion.identity, currentAttack.data.boxDimensions, Color.red);
				}
				break;
			case EHitDetectionType.Capsul:
				{
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.Translate(currentAttack.data.offset);
					Collider[] colliders = Ultra.Utilities.OverlapCapsule(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, currentAttack.data.capsulHeight, currentAttack.data.radius);
					foreach (Collider collider in colliders)
					{
						WeaponColliderEnter(collider);
					}
					Ultra.Utilities.DrawCapsule(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, Quaternion.identity,currentAttack.data.capsulHeight, currentAttack.data.radius, Color.red);
					//Ultra.Utilities.DrawWireCapsule(hitDetectionGameObject.transform.position, Quaternion.identity, lastAttack.data.radius, lastAttack.data.capsulHeight, Color.red, 200);
					//Gizmos.DrawCube(hitDetectionGameObject.transform.position, Vector3.one);
				}
				break;
			default:
				{
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.Translate(currentAttack.data.offset);
					Collider[] colliders = Physics.OverlapSphere(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, currentAttack.data.radius);
					foreach (Collider collider in colliders)
					{
						WeaponColliderEnter(collider);
					}
					Ultra.Utilities.DrawWireSphere(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, currentAttack.data.radius, Color.red, 0f, 100);
				}
				break;
		}
	}

	public virtual AttackAnimationData GroundAttack(float attackDeltaTime)
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		SetCurrentAttack(EExplicitAttackType.GroundedDefaultAttack, attackDeltaTime);
		if (currentAttack.attackDataHolder.Attack != null)
		{
			currentAttack.attackDataHolder.Attack.Init(GameCharacter, this);
			currentAttack.attackDataHolder.Attack.StartAttack();
		}
		return currentAttack;
		return BaseAttackLogic(EExplicitAttackType.GroundedDefaultAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundAttacks, attackDeltaTime);
	}
	public virtual AttackAnimationData GroundUpAttack(float attackDeltaTime)
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundUpAttacks.Count > 0)
		{
			SetCurrentAttack(EExplicitAttackType.GroundedUpAttack, attackDeltaTime);
			if (currentAttack.attackDataHolder.Attack != null)
			{
				currentAttack.attackDataHolder.Attack.Init(GameCharacter, this);
				currentAttack.attackDataHolder.Attack.StartAttack();
			}
			return currentAttack;
			return BaseAttackLogic(EExplicitAttackType.GroundedUpAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundUpAttacks, attackDeltaTime);
		}
		else return GroundAttack(attackDeltaTime);
	}
    public virtual AttackAnimationData GroundDownAttack(float attackDeltaTime)
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDownAttacks.Count > 0)
		{
			SetCurrentAttack(EExplicitAttackType.GroundedDownAttack, attackDeltaTime);
			if (currentAttack.attackDataHolder.Attack != null)
			{
				currentAttack.attackDataHolder.Attack.Init(GameCharacter, this);
				currentAttack.attackDataHolder.Attack.StartAttack();
			}
			return currentAttack;
			return BaseAttackLogic(EExplicitAttackType.GroundedDownAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDownAttacks, attackDeltaTime);
		}
		else return GroundAttack(attackDeltaTime);
	}
    public virtual AttackAnimationData GroundDirectionAttack(float attackDeltaTime)
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDirectionAttacks.Count > 0)
		{
			SetCurrentAttack(EExplicitAttackType.GroundedDirectionalAttack, attackDeltaTime);
			if (currentAttack.attackDataHolder.Attack != null)
			{
				currentAttack.attackDataHolder.Attack.Init(GameCharacter, this);
				currentAttack.attackDataHolder.Attack.StartAttack();
			}
			return currentAttack;
			return BaseAttackLogic(EExplicitAttackType.GroundedDirectionalAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDirectionAttacks, attackDeltaTime);
		}
		else return GroundAttack(attackDeltaTime);
	}

    public virtual AttackAnimationData AirAttack(float attackDeltaTime)
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		SetCurrentAttack(EExplicitAttackType.AirDefaultAttack, attackDeltaTime);
		if (currentAttack.attackDataHolder.Attack != null)
		{
			currentAttack.attackDataHolder.Attack.Init(GameCharacter, this);
			currentAttack.attackDataHolder.Attack.StartAttack();
		}
		return currentAttack;
		return BaseAttackLogic(EExplicitAttackType.AirDefaultAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirAttacks, attackDeltaTime);
	}
    public virtual AttackAnimationData AirUpAttack(float attackDeltaTime)
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirUpAttacks.Count > 0)
		{
			SetCurrentAttack(EExplicitAttackType.AirUpAttack, attackDeltaTime);
			if (currentAttack.attackDataHolder.Attack != null)
			{
				currentAttack.attackDataHolder.Attack.Init(GameCharacter, this);
				currentAttack.attackDataHolder.Attack.StartAttack();
			}
			return currentAttack;
			return BaseAttackLogic(EExplicitAttackType.AirUpAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirUpAttacks, attackDeltaTime);
		}
		else return AirAttack(attackDeltaTime);
	}
    public virtual AttackAnimationData AirDownAttack(float attackDeltaTime)
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0)
		{
			SetCurrentAttack(EExplicitAttackType.AirDownAttack, attackDeltaTime);
			if (currentAttack.attackDataHolder.Attack != null)
			{
				currentAttack.attackDataHolder.Attack.Init(GameCharacter, this);
				currentAttack.attackDataHolder.Attack.StartAttack();
			}
			return currentAttack;
			return BaseAttackLogic(EExplicitAttackType.AirDownAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks, attackDeltaTime);
		}
		else return AirAttack(attackDeltaTime);
	}
    public virtual AttackAnimationData AirDirectionAttack(float attackDeltaTime)
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDirectionAttacks.Count > 0)
		{
			SetCurrentAttack(EExplicitAttackType.AirDirectionalAttack, attackDeltaTime);
			if (currentAttack.attackDataHolder.Attack != null)
			{
				currentAttack.attackDataHolder.Attack.Init(GameCharacter, this);
				currentAttack.attackDataHolder.Attack.StartAttack();
			}
			return currentAttack;
			return BaseAttackLogic(EExplicitAttackType.AirDirectionalAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDirectionAttacks, attackDeltaTime);
		}
		else return AirAttack(attackDeltaTime);
	}

	public virtual AttackAnimationData DefensiveAction()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return null;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction.Count > 0)
		{
			SetCurrentDefensiveAction();
			if (currentAttack.attackDataHolder.Attack != null)
			{
				currentAttack.attackDataHolder.Attack.Init(GameCharacter, this);
				currentAttack.attackDataHolder.Attack.StartAttack();
			}
			return currentDefensiveAction;
			return DefensiceActionLogic(ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction);
		}
		return null;
	}

	public virtual void GroundAttackHit(GameObject hitObj)
	{
		CurrentDefensiveAction.attackDataHolder.Attack?.OnHit(hitObj);
	}

	public virtual void GroundUpAttackHit(GameObject hitObj)
	{
		CurrentDefensiveAction.attackDataHolder.Attack?.OnHit(hitObj);
	}

	public virtual void GroundDownAttackHit(GameObject hitObj)
	{
		CurrentDefensiveAction.attackDataHolder.Attack?.OnHit(hitObj);
	}

	public virtual void GroundDirectionAttackHit(GameObject hitObj)
	{
		CurrentDefensiveAction.attackDataHolder.Attack?.OnHit(hitObj);
	}

	public virtual void AirAttackHit(GameObject hitObj)
	{
		CurrentDefensiveAction.attackDataHolder.Attack?.OnHit(hitObj);
	}

	public virtual void AirUpAttackHit(GameObject hitObj)
	{
		CurrentDefensiveAction.attackDataHolder.Attack?.OnHit(hitObj);
	}

	public virtual void AirDownAttackHit(GameObject hitObj)
	{
		CurrentDefensiveAction.attackDataHolder.Attack?.OnHit(hitObj);
	}

	public virtual void AirDirectionAttackHit(GameObject hitObj)
	{
		CurrentDefensiveAction.attackDataHolder.Attack?.OnHit(hitObj);
	}

	void SetUpWeaponAnimationData()
	{
		if (weaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) gameCharacter.AnimController.SetBodyLayerAnimClip(weaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyPose);
	}

	public IDamage GetDamageInterface(GameObject obj)
	{
		if (obj == null) return null;	
		return obj.GetComponent<IDamage>();
	}

	public List<AttackAnimationData> GetAttackListBasedOnEExplicitAttackType(EExplicitAttackType explicitAttackType)
	{
		switch (explicitAttackType) 
		{
			case EExplicitAttackType.GroundedDefaultAttack: return WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundAttacks;
			case EExplicitAttackType.GroundedDownAttack: return WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDownAttacks;
			case EExplicitAttackType.GroundedUpAttack: return WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundUpAttacks;
			case EExplicitAttackType.GroundedDirectionalAttack: return WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDirectionAttacks;
			case EExplicitAttackType.AirDefaultAttack: return WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirAttacks;
			case EExplicitAttackType.AirDownAttack: return WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks;
			case EExplicitAttackType.AirUpAttack: return WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirUpAttacks;
			case EExplicitAttackType.AirDirectionalAttack: return WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDirectionAttacks;
			case EExplicitAttackType.DefensiveAction: return WeaponData.AnimationData[GameCharacter.CharacterData.Name].DefensiveAction;
			default:
				Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "GetAttackListBasedOnEExplicitAttackType", "Coundn't find List, ExplicitAttackType not Implemented");
				return new List<AttackAnimationData>();
		}
	}

	public void SetCurrentAttack(EExplicitAttackType explicitAttackType, float attackDeltaTime, bool updatedIndex = true)
	{
		List<AttackAnimationData> attackList = GetAttackListBasedOnEExplicitAttackType(explicitAttackType);
		if (!CheckForAttackBrenchesInLastAttack(explicitAttackType, updatedIndex, attackDeltaTime))
		{
			if (currentAttackType != explicitAttackType || !updatedIndex)
			{
				attackIndex = 0;
				currentAttackType = explicitAttackType;
			}
			else
			{
				attackIndex++;
			}
		}

		if (attackList == null || attackList.Count <= 0) return;
		attackIndex = attackIndex % attackList.Count;
		currentAttack = attackList[attackIndex];
	}

	bool CheckForAttackBrenchesInLastAttack(EExplicitAttackType explicitAttackType, bool updatedIndex, float attackDeltaTime)
	{
		if (updatedIndex && currentAttack != null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("AttackDeltaTime" + attackDeltaTime, 1f, StringColor.Green, 100, DebugAreas.Combat);
			if (AttackDeltatimeIsValid(attackDeltaTime) && currentAttack.timedCombatBrenches.TryGetValue(explicitAttackType, out int x))
			{
				attackIndex = x;
				return true;
			}
			else if (currentAttackType != EExplicitAttackType.Unknown && currentAttack.combatBranches.TryGetValue(explicitAttackType, out int i))
			{
				attackIndex = i;
				return true;
			}
		}
		return false;
	}

	private static bool AttackDeltatimeIsValid(float attackDeltaTime)
	{
		float minTime = 0.5f;
		float maxTime = 1.0f;
		return attackDeltaTime > minTime && attackDeltaTime < maxTime;
	}

	public void SetCurrentDefensiveAction(bool updateAttackType = true)
	{
		List<AttackAnimationData> defensiveList = GetAttackListBasedOnEExplicitAttackType(EExplicitAttackType.DefensiveAction);
		if (updateAttackType) currentAttackType = EExplicitAttackType.DefensiveAction;
		defensiveActionIndex++;
		if (defensiveList == null || defensiveList.Count <= 0) return;
		defensiveActionIndex = defensiveActionIndex % defensiveList.Count;
		currentDefensiveAction = defensiveList[defensiveActionIndex];
	}

	protected AttackAnimationData BaseAttackLogic(EExplicitAttackType explicitAttackType, ref List<AttackAnimationData> attackList, float attackDeltaTime)
	{
		SetCurrentAttack(explicitAttackType, attackDeltaTime);
		if (currentAttack == null || currentAttack.clip == null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("AttackClip was null!", 5f, StringColor.Red, 100, DebugAreas.Combat);
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "BaseAttackLogic", "AttackClip was null!"));
			return null;
		}
		gameCharacter.AnimController.Attack(currentAttack.clip);
		gameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Attack);
		attackAnimType = EAttackAnimType.Default;

		return currentAttack;
	}
	protected AttackAnimationData Attack3BlendLogic(EExplicitAttackType explicitAttackType, ref List<AttackAnimationData> attackList, EAnimationType animType, float attackDeltaTime)
	{
		SetCurrentAttack(explicitAttackType, attackDeltaTime);
		//Attack3BlendSpace(animType);
		gameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Attack);
		attackAnimType = EAttackAnimType.Combat3Blend;

		return currentAttack;
	}
	protected AttackAnimationData AttackAimLogic(EExplicitAttackType explicitAttackType, ref List<AttackAnimationData> attackList, EAnimationType animType, float attackDeltaTime)
	{
		SetCurrentAttack(explicitAttackType, attackDeltaTime);
		AimBlendSpace(ref attackList, animType); 
		gameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Aim);
		attackAnimType = EAttackAnimType.AimBlendSpace;

		return currentAttack;
	}

	protected AttackAnimationData DefensiceActionLogic(ref List<AttackAnimationData> defensiveList)
	{
		SetCurrentDefensiveAction();
		if (currentDefensiveAction == null || currentDefensiveAction.clip == null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("DefensiveActionClip was null!", 5f, StringColor.Red, 100, DebugAreas.Combat);
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "DefensiceActionLogic", "DefensiveActionClip was null!"));
			return null;
		}
		gameCharacter.AnimController.SetDefensiveAction(currentDefensiveAction.clip);
		gameCharacter.StateMachine.RequestStateChange(EGameCharacterState.DefensiveAction);
		attackAnimType = EAttackAnimType.Default;

		return currentDefensiveAction;
	}

	protected AttackAnimationData DefensiveActionAimLogic(ref List<AttackAnimationData> defensiveList, EAnimationType animType, bool upDateAttackType = true)
	{
		SetCurrentDefensiveAction(upDateAttackType);
		AimBlendSpace(ref defensiveList, animType);
		gameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.Aim);
		gameCharacter.StateMachine.RequestStateChange(EGameCharacterState.DefensiveAction);
		attackAnimType = EAttackAnimType.AimBlendSpace;

		return currentDefensiveAction;
	}

	protected void AimBlendSpace(ref List<AttackAnimationData> defensiveList, EAnimationType animType)
	{
		AttackAnimationData attackData = GetAttackAnimationData();
		if (attackData == null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("AttackData was null!", 5f, StringColor.Red, 100, DebugAreas.Combat);
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "AimBlendSpace", "AttackData was null!"));
			return;
		}
		switch (animType)
		{
			case EAnimationType.Hold: gameCharacter.AnimController.ApplyBlendTree(attackData.aimBlendTypes.blendHoldAnimations); break;
			case EAnimationType.Trigger: gameCharacter.AnimController.ApplyBlendTree(attackData.aimBlendTypes.blendTriggerAnimations); break;
			default:
				gameCharacter.AnimController.ApplyBlendTree(attackData.aimBlendTypes.blendAnimations);
				break;
		}
	}

	public void Attack3BlendSpace(AimBlendAnimations blendTreeAnims, EAnimationType animType)
	{
		if (blendTreeAnims == null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("AttackData was null!", 5f, StringColor.Red, 100, DebugAreas.Combat);
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "AimBlendSpace", "AttackData was null!"));
			return;
		}
		switch (animType)
		{
			case EAnimationType.Hold: gameCharacter.AnimController.ApplyCombat3BlendTree(blendTreeAnims); break;
			case EAnimationType.Trigger: gameCharacter.AnimController.ApplyCombat3BlendTree(blendTreeAnims); break;
			default:
				gameCharacter.AnimController.ApplyCombat3BlendTree(blendTreeAnims);
				break;
		}
	}

	AttackAnimationData GetAttackAnimationData()
	{
		switch (CurrentAttackType)
		{
			case EExplicitAttackType.DefensiveAction:
				return currentDefensiveAction;
			default:
				return currentAttack;
		}
	}

	public void HoldAttackAfterAttack(EExplicitAttackType explicitAttackType, ref List<AttackAnimationData> attackList, float attackDeltaTime)
	{
		SetCurrentAttack(explicitAttackType, attackDeltaTime, false);
		if (currentAttack.holdAnimation == null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("HoldAnimation was null!", 5f, StringColor.Red, 100, DebugAreas.Combat);
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "HoldAttackAfterAttack", "HoldAnimation was null!"));
			return;
		}

		gameCharacter.AnimController.SetHoldAttack(currentAttack.holdAnimation);
		gameCharacter.CombatComponent.AttackTimer.IsPaused = true;

		gameCharacter.AnimController.HoldAttack = true;
	}

	public void TriggerAttack(EExplicitAttackType explicitAttackType, ref List<AttackAnimationData> attackList, float attackDeltaTime)
	{
		SetCurrentAttack(explicitAttackType, attackDeltaTime, false);
		if (currentAttack.triggerAnimation == null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("TriggerAnimation was null!", 5f, StringColor.Red, 100, DebugAreas.Combat);
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "TriggerAttack", "TriggerAnimation was null!"));
			return;
		}
		gameCharacter.AnimController.SetTriggerAttack(currentAttack.triggerAnimation);
		gameCharacter.CombatComponent.AttackTimer.Start(currentAttack.triggerAnimation.length);
		gameCharacter.AnimController.AttackTriggerTimer.Start(currentAttack.triggerAnimation.length);

		gameCharacter.AnimController.TriggerAttack = true;
	}

	public virtual void HitDetectionStart()
	{
		if (currentAttack == null) 
			return;

		if (ishitDetecting) return;

		ishitDetecting = true;
		switch (currentAttack.data.hitDetectionType)
		{
			case EHitDetectionType.Mesh:
				gameCharacter.CombatComponent.HitDetectionMeshCollider.sharedMesh = currentAttack.data.mesh;
				gameCharacter.CombatComponent.HitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
				gameCharacter.CombatComponent.HitDetectionGameObject.transform.Translate(currentAttack.data.offset);
				gameCharacter.CombatComponent.HitDetectionMeshFilter.mesh = currentAttack.data.mesh;
				gameCharacter.CombatComponent.HitDetectionMeshFilter.sharedMesh = currentAttack.data.mesh;
#if UNITY_EDITOR
				if (Ultra.Utilities.Instance.debugLevel >= 100) gameCharacter.CombatComponent.HitDetectionMeshRenderer.enabled = true;
#endif
				// Happens at the end of frame where collision got updated from movement above
				InitialMeshColliderCheck();
				break;
			default:
				break;
		}
		AttackPhaseStart();
	}

	async void InitialMeshColliderCheck()
	{
		await new WaitForEndOfFrame();
		foreach (Collider collider in gameCharacter.CombatComponent.HitDetectionColliderScript.OverlappingColliders)
		{
			WeaponColliderEnter(collider);
		}
	}

	public virtual void HitDetectionEnd()
	{
		hitObjects.Clear();

		if (!ishitDetecting) return;

		ishitDetecting = false;
		switch(currentAttack.data.hitDetectionType)
		{
			case EHitDetectionType.Mesh: gameCharacter.CombatComponent.HitDetectionMeshRenderer.enabled = false; break;
				default: break;
		}
		AttackPhaseEnd();
		comboIndexInSameAttack++;
	}

	protected virtual void WeaponColliderEnter(Collider other)
	{
		if (!IsHitDetecting) return;
		if (other.gameObject == gameCharacter.gameObject) return;

		GameCharacter possibleGameCharacter = other.GetComponent<GameCharacter>();
		if (possibleGameCharacter != null && possibleGameCharacter.CheckForSameTeam(gameCharacter.Team)) return;

		if (hitObjects.Contains(other.gameObject)) return;
		hitObjects.Add(other.gameObject);

		if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Red + "Hit Object: " + other.gameObject.name + StringColor.EndColor, (currentAttack.data.hitDetectionType == EHitDetectionType.Mesh) ? 1f : 0f);
		switch (currentAttackType)
		{
			case EExplicitAttackType.GroundedDefaultAttack: GroundAttackHit(other.gameObject); break;
			case EExplicitAttackType.GroundedDirectionalAttack: GroundDirectionAttackHit(other.gameObject); break;
			case EExplicitAttackType.GroundedDownAttack: GroundDownAttackHit(other.gameObject); break;
			case EExplicitAttackType.GroundedUpAttack: GroundUpAttackHit(other.gameObject); break;
			case EExplicitAttackType.AirDefaultAttack: AirAttackHit(other.gameObject); break;
			case EExplicitAttackType.AirDirectionalAttack: AirDirectionAttackHit(other.gameObject); break;
			case EExplicitAttackType.AirDownAttack: AirDownAttackHit(other.gameObject); break;
			case EExplicitAttackType.AirUpAttack: AirUpAttackHit(other.gameObject); break;
			default: break;
		}
	}

	protected virtual void WeaponColliderExit(Collider other)
	{
		if (!IsHitDetecting) return;

	}

	void OnGameCharacterStateChange(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		if (oldState?.GetStateType() == EGameCharacterState.Attack)
		{
			comboIndexInSameAttack = 0;
		}
		if (!(newState?.GetStateType() == EGameCharacterState.Attack || newState?.GetStateType() == EGameCharacterState.DefensiveAction) && oldState?.GetStateType() == EGameCharacterState.AttackRecovery)
		{
			lastAttackType = currentAttackType;
			currentAttackType = EExplicitAttackType.Unknown;
		}
	}

	public virtual void StartAttackStateLogic()
	{
		CurrentAttack.attackDataHolder.Attack?.StartAttackStateLogic();
	}

	public virtual void PreAttackStateLogic(float deltaTime)
	{
		CurrentAttack.attackDataHolder.Attack?.PreAttackStateLogic(deltaTime);
	}

	public virtual void PostAttackStateLogic(float deltaTime)
	{
		CurrentAttack.attackDataHolder.Attack?.PostAttackStateLogic(deltaTime);
	}

	public virtual void EndAttackStateLogic()
	{
		CurrentAttack.attackDataHolder.Attack?.EndAttackStateLogic();
		hitObjects.Clear();
	}

	public virtual void AttackPhaseStart()
	{
		CurrentAttack.attackDataHolder.Attack?.AttackPhaseStart();
	}

	public virtual void AttackPhaseEnd()
	{
		CurrentAttack.attackDataHolder.Attack?.AttackPhaseEnd();
	}

	public virtual void AttackRecoveryEnd()
	{
		CurrentAttack.attackDataHolder.Attack?.AttackRecoveryEnd();
	}

	public virtual void DefensiveActionStateEnd()
	{
		CurrentAttack.attackDataHolder.Attack?.DefensiveActionStateEnd();
	}

	public void StartParticelEffect(int index)
	{
		switch (currentAttackType)
		{
			case EExplicitAttackType.GroundedDefaultAttack: PlayParticleEffect(groundLightAttackParticleList[attackIndex][index], CurrentAttack.particleList[index]); break;
			case EExplicitAttackType.GroundedDirectionalAttack: PlayParticleEffect(groundHeavyAttackParticleList[attackIndex][index], CurrentAttack.particleList[index]); break;
			case EExplicitAttackType.GroundedUpAttack: PlayParticleEffect(groundUpAttackParticleList[attackIndex][index], CurrentAttack.particleList[index]); break;
			case EExplicitAttackType.GroundedDownAttack: PlayParticleEffect(groundDownAttackParticleList[attackIndex][index], CurrentAttack.particleList[index]); break;
			case EExplicitAttackType.AirDefaultAttack: PlayParticleEffect(airLightAttackParticleList[attackIndex][index], CurrentAttack.particleList[index]); break;
			case EExplicitAttackType.AirDirectionalAttack: PlayParticleEffect(airHeavyAttackParticleList[attackIndex][index], CurrentAttack.particleList[index]); break;
			case EExplicitAttackType.AirDownAttack: PlayParticleEffect(airDownAttackParticleList[attackIndex][index], CurrentAttack.particleList[index]); break;
			case EExplicitAttackType.AirUpAttack: PlayParticleEffect(airUpAttackParticleList[attackIndex][index], CurrentAttack.particleList[index]); break;
			case EExplicitAttackType.DefensiveAction: PlayParticleEffect(defensiveActionParticleList[defensiveActionIndex][index], CurrentDefensiveAction.particleList[index]); break;
			default: break;
		}
	}

	void PlayParticleEffect(ParticleSystem particle, GameObject baseParticle)
	{
		particle.transform.parent = GameCharacter.GameCharacterData.Root;
		particle.transform.localPosition = baseParticle.transform.localPosition;
		particle.transform.localRotation = baseParticle.transform.localRotation;
		particle.Play();
		particle.transform.parent = null;
	}

	void InitParticleForAttackList(ref List<AttackAnimationData> animList, ref List<List<ParticleSystem>> particleList)
	{
		for (int i = 0; i < animList.Count; i++)
		{
			particleList.Add(new List<ParticleSystem>());
			for (int j = 0; j < animList[i].particleList.Count; j++)
			{
				GameObject particle = GameObject.Instantiate(animList[i].particleList[j], GameCharacter.GameCharacterData.Root);
				ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
				particleList[i].Add(particleSystem);
			}
		}
	}

	public virtual void CharacterArrivedAtRequestedLocation(GameCharacter movedCharacter)
	{
		CurrentAttack.attackDataHolder.Attack?.CharacterArrivedAtRequestedLocation(movedCharacter);
	}

	public virtual void CharacterMoveToAbort(GameCharacter movedCharacter)
	{
		CurrentAttack.attackDataHolder.Attack?.CharacterMoveToAbort(movedCharacter);
	}

	public virtual void CharacterMoveToEnd(GameCharacter movedCharacter)
	{
		CurrentAttack.attackDataHolder.Attack?.CharacterMoveToEnd(movedCharacter);
	}

	public virtual void DefensiveActionStart()
	{
		CurrentAttack.attackDataHolder.Attack?.DefensiveActionStart();
	}

	public virtual void DefensiveActionEnd()
	{
		CurrentAttack.attackDataHolder.Attack?.DefensiveActionEnd();
	}

	public virtual bool CanLeaveDefensiveState()
	{
		if (CurrentAttack.attackDataHolder.Attack == null) return true;
		return CurrentAttack.attackDataHolder.Attack.CanLeaveDefensiveState();
	}

	public virtual void GroundReset()
	{

	}

	public void UnHookAllHookedCharacerts()
	{
		foreach (GameCharacter character in GameCharacter.CombatComponent.HookedCharacters)
		{
			character.CombatComponent.HookedToCharacter = null;
		}
		GameCharacter.CombatComponent.HookedCharacters.Clear();
	}

	public void HookCharacterToCharacter(GameCharacter enemyCharacter)
	{
		if (!GameCharacter.CombatComponent.HookedCharacters.Contains(enemyCharacter))
			GameCharacter.CombatComponent.HookedCharacters.Add(enemyCharacter);
	}

	protected void RequestFlyAway(GameCharacter enemyCharacter)
	{
		if (enemyCharacter.CombatComponent.CanRequestFlyAway())
		{
			//enemyCharacter.StateMachine.RequestStateChange(EGameCharacterState.FlyAway);
			enemyCharacter.StateMachine.ForceStateChange(EGameCharacterState.FlyAway);
			enemyCharacter.CombatComponent.FlyAwayTime = CurrentAttack.extraData.flyAwayTime;
			if (Mathf.Sign(GameCharacter.transform.forward.x) < 0)
			{
				Vector3 direction = Quaternion.Euler(CurrentAttack.extraData.flyAwayDirection) * GameCharacter.transform.forward;
				direction.y = direction.y * -1;
				enemyCharacter.MovementComponent.MovementVelocity = direction * CurrentAttack.extraData.flyAwayStrengh;
			}
			else
			{
				Vector3 direction = Quaternion.Euler(CurrentAttack.extraData.flyAwayDirection) * GameCharacter.transform.forward;
				enemyCharacter.MovementComponent.MovementVelocity = direction * CurrentAttack.extraData.flyAwayStrengh;
			}

			Ultra.Utilities.DrawArrow(enemyCharacter.MovementComponent.CharacterCenter, enemyCharacter.MovementComponent.MovementVelocity.normalized, 5f, Color.magenta, 10f, 100, DebugAreas.Combat);
		}
	}

	void OnMaxChargeAfterEquipTimerFinished()
	{
		Charge = chargeAfterTime;
	}

	public virtual float GetDamage()
	{
		float chargeValue = Ultra.Utilities.Remap(Charge, 0, weaponData.MaxChargeAmount, 0.1f, 1f);
		//Ultra.Utilities.Instance.DebugLogOnScreen("Damage => " + CurrentAttack.extraData.Damage, 1f, StringColor.Magenta);
		return chargeValue * CurrentAttack.extraData.Damage;
	}
}
