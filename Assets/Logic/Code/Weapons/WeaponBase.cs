using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    GapCloser,
	DefensiveAction,
	Ultimate,
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
	public OnChargeValueChanged onUltChargeValueChanged;

	GameCharacter gameCharacter;
    ScriptableWeapon weaponData;
    GameObject spawnedWeapon;
    GameObject secondSpawnedWeapon;
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
	float ultCharge = 0;
	Ultra.Timer maxChargeAfterEquipTimer;
	AttackAnimationData lastData;
	ScriptableWeaponAnimationData animationData;
	int lastAttackSoundIndex;
	int lastHitSoundIndex;
	bool shouldPlayHitSound = true;

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
    public virtual ScriptableWeapon WeaponData { get { return weaponData; } }
    public virtual GameObject SpawnedWeapon { get { return spawnedWeapon; } }
    public virtual GameObject SecondSpawnedWeapon { get { return secondSpawnedWeapon; } }
    public virtual GameObject SpawnedWeaponBones { get { return spawnedWeaponBones; } }
	public virtual bool IsHitDetecting { get { return ishitDetecting; } }
	public virtual AttackAnimationData CurrentAction { get { return GetAttackAnimationData(); } }
	public virtual EExplicitAttackType CurrentAttackType { get { return currentAttackType; } }
	public virtual EExplicitAttackType LastAttackType { get { return lastAttackType; } }
	public virtual EAttackAnimType AttackAnimType { get { return attackAnimType; } set { attackAnimType = value; } }
	public virtual int ComboIndexInSameAttack { get { return comboIndexInSameAttack; } }
	public virtual List<GameObject> HitObjects { get { return hitObjects; } }	
	public virtual int AttackIndex { get { return attackIndex; } }
	public virtual bool ShouldPlayHitSound { get { return shouldPlayHitSound; } set { shouldPlayHitSound = value; } }
	//public virtual WeaponBase This { get { return this; } }
	public virtual ScriptableWeaponAnimationData AnimationData { 
		get {
			if (animationData == null)
			{
				var data = ScriptableObject.CreateInstance<ScriptableWeaponAnimationData>();
				data.Copy(weaponData.AnimationData[gameCharacter.CharacterData.Name]);
				if (weaponData.AnimationData.ContainsKey(gameCharacter.CharacterData.Name)) animationData = data;
			}
			return animationData; 
		} 
		set { animationData = value; }
	}
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
	public float UltCharge
	{
		get { return ultCharge; }
		set
		{
			// If Ult is ready dont take it away from player, feels shity
			if (ultCharge >= weaponData.MaxUltChargeAmount && value < weaponData.MaxUltChargeAmount) return;

			value = Mathf.Clamp(value, 0, weaponData.MaxUltChargeAmount); 
			if (ultCharge != value)
			{
				float oldUltCharge = ultCharge;
				ultCharge = value;

				if (onUltChargeValueChanged != null) onUltChargeValueChanged(ultCharge, oldUltCharge);
			}
		}
	}
	public Ultra.Timer MaxChargeAfterEquipTimer { get { return maxChargeAfterEquipTimer; } }

	public WeaponBase() { }
    public WeaponBase(GameCharacter gameCharacter, ScriptableWeapon weaponData)
    {
        this.gameCharacter = gameCharacter;
        this.weaponData = weaponData;
		charge = weaponData.DefaultChargeAmount;
		hitObjects = new List<GameObject>();
		maxChargeAfterEquipTimer = new Ultra.Timer();
		//if (weaponData.AnimationData.ContainsKey(gameCharacter.CharacterData.Name)) animationData = weaponData.AnimationData[gameCharacter.CharacterData.Name].Copy();
		Dleayed_Internal_Init();
	}

	~WeaponBase() { }

	public virtual void InitWeapon()
	{
		if (AnimationData == null) return;

		groundLightAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref AnimationData.GroundAttacks, ref groundLightAttackParticleList);
		groundHeavyAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref AnimationData.GroundDirectionAttacks, ref groundHeavyAttackParticleList);
		groundUpAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref AnimationData.GroundUpAttacks, ref groundUpAttackParticleList);
		groundDownAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref AnimationData.GroundDownAttacks, ref groundDownAttackParticleList);
		airLightAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref AnimationData.AirAttacks, ref airLightAttackParticleList);
		airHeavyAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref AnimationData.AirDirectionAttacks, ref airHeavyAttackParticleList);
		airUpAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref AnimationData.AirUpAttacks, ref airUpAttackParticleList);
		airDownAttackParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref AnimationData.AirDownAttacks, ref airDownAttackParticleList);
		defensiveActionParticleList = new List<List<ParticleSystem>>();
		InitParticleForAttackList(ref AnimationData.DefensiveAction, ref defensiveActionParticleList);
	}

    public virtual void EquipWeapon()
	{
		SetWeaponReadyPose();
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
		if (AnimationData != null && gameCharacter.IsPlayerCharacter)
		{
			foreach(AttackAnimationData data in AnimationData.DefensiveAction)
			{
				if (data != null && data.Action != null && data.Action.HasUIImplementation())
				{
					data.Action.Init(gameCharacter, this);
					data.Action.ImplementUI();
					break;
				}
			}
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
		if (spawnedWeapon != null) GameObject.Destroy(spawnedWeapon);
		if (secondSpawnedWeapon != null) GameObject.Destroy(secondSpawnedWeapon);
		if (spawnedWeaponBones != null) GameObject.Destroy(spawnedWeaponBones);

		CurrentAction?.Action?.ActionInterupted();
		HitDetectionEnd();

		if (AnimationData != null && gameCharacter.IsPlayerCharacter)
		{
			foreach (AttackAnimationData data in AnimationData.DefensiveAction)
			{
				if (data != null && data.Action != null && data.Action.HasUIImplementation())
				{
					data.Action.RemoveUI();
				}
			}
		}

		if (gameCharacter.CombatComponent.HitDetectionColliderScript != null) gameCharacter.CombatComponent.HitDetectionColliderScript.onOverlapEnter -= WeaponColliderEnter;
		if (gameCharacter.CombatComponent.HitDetectionColliderScript != null) gameCharacter.CombatComponent.HitDetectionColliderScript.onOverlapExit -= WeaponColliderExit;

	}

	private void SpawnVisualWeaponMesh()
	{
		if (AnimationData == null) return;

		if (WeaponData.WeaponMeshData.cascadeurSetUp)
		{
			switch (AnimationData.HandType)
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
			switch (AnimationData.HandType)
			{
				case EWeaponHandType.RightHand:
					if (weaponData.WeaponMeshData.WeaponMesh == null) break;
					spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.GameCharacterData.HandROnjectPoint);
					spawnedWeapon.transform.position = Vector3.zero;
					spawnedWeapon.transform.rotation = Quaternion.identity;
					spawnedWeapon.transform.localScale = Vector3.one;
					spawnedWeapon.transform.localPosition = Vector3.zero;
					spawnedWeapon.transform.localRotation = Quaternion.identity;
					spawnedWeapon.transform.Translate(WeaponData.WeaponMeshData.WeaponOffset, Space.Self);
					spawnedWeapon.transform.localRotation = Quaternion.Euler(WeaponData.WeaponMeshData.WeaponRotationEuler);
					spawnedWeapon.transform.localScale = WeaponData.WeaponMeshData.WeaponScale;
					break;
				case EWeaponHandType.LeftHand:
					if (weaponData.WeaponMeshData.WeaponMesh == null) break;
					spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.GameCharacterData.HandLOnjectPoint);
					spawnedWeapon.transform.position = Vector3.zero;
					spawnedWeapon.transform.rotation = Quaternion.identity;
					spawnedWeapon.transform.localScale = Vector3.one;
					spawnedWeapon.transform.localPosition = Vector3.zero;
					spawnedWeapon.transform.localRotation = Quaternion.identity;
					spawnedWeapon.transform.Translate(WeaponData.WeaponMeshData.WeaponOffset, Space.Self);
					spawnedWeapon.transform.localRotation = Quaternion.Euler(WeaponData.WeaponMeshData.WeaponRotationEuler);
					spawnedWeapon.transform.localScale = WeaponData.WeaponMeshData.WeaponScale;
					break;
				case EWeaponHandType.NoHands:
					if (weaponData.WeaponMeshData.WeaponMesh == null) break;
					spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.transform);
					break;
				case EWeaponHandType.Both:
					if (weaponData.WeaponMeshData.WeaponMesh == null) break;
					spawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.GameCharacterData.HandROnjectPoint);
					spawnedWeapon.transform.position = Vector3.zero;
					spawnedWeapon.transform.rotation = Quaternion.identity;
					spawnedWeapon.transform.localScale = Vector3.one;
					spawnedWeapon.transform.localPosition = Vector3.zero;
					spawnedWeapon.transform.localRotation = Quaternion.identity;
					spawnedWeapon.transform.Translate(WeaponData.WeaponMeshData.WeaponOffset, Space.Self);
					spawnedWeapon.transform.localRotation = Quaternion.Euler(WeaponData.WeaponMeshData.WeaponRotationEuler);
					spawnedWeapon.transform.localScale = WeaponData.WeaponMeshData.WeaponScale;

					secondSpawnedWeapon = GameObject.Instantiate(WeaponData.WeaponMeshData.WeaponMesh, gameCharacter.GameCharacterData.HandLOnjectPoint);
					secondSpawnedWeapon.transform.position = Vector3.zero;
					secondSpawnedWeapon.transform.rotation = Quaternion.identity;
					secondSpawnedWeapon.transform.localScale = Vector3.one;
					secondSpawnedWeapon.transform.localPosition = Vector3.zero;
					secondSpawnedWeapon.transform.localRotation = Quaternion.identity;
					secondSpawnedWeapon.transform.Translate(WeaponData.WeaponMeshData.secondWeaponOffsets.WeaponOffset, Space.Self);
					secondSpawnedWeapon.transform.localRotation = Quaternion.Euler(WeaponData.WeaponMeshData.secondWeaponOffsets.WeaponRotationEuler);
					secondSpawnedWeapon.transform.localScale = WeaponData.WeaponMeshData.secondWeaponOffsets.WeaponScale;
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
		if (!GameCharacter.CharacterHasAggro)
		{
			if (Charge < weaponData.DefaultChargeAmount)
			{
				Charge = Mathf.Lerp(Charge, weaponData.DefaultChargeAmount, Time.deltaTime * 0.5f);
			}
		}
		ShouldPlayHitSound = true;

		if (ishitDetecting && (gameCharacter.StateMachine.CurrentState.GetStateType() != EGameCharacterState.Attack && gameCharacter.StateMachine.CurrentState.GetStateType() != EGameCharacterState.AttackRecovery))
			HitDetectionEnd();

		HitDetection();
	}

	private void HitDetection()
	{
		if (!ishitDetecting) return;
		if (CurrentAction == null) return;

		switch (CurrentAction.data.hitDetectionType)
		{
			case EHitDetectionType.Mesh:
				// We Dont need to do anything here because the hitDetectionObject does the job
				break;
			case EHitDetectionType.Box:
				{
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
					//gameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation = Quaternion.identity;
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.Translate(CurrentAction.data.offset);
					Collider[] colliders = Physics.OverlapBox(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, CurrentAction.data.boxDimensions / 2, Quaternion.identity, GameCharacter.CharacterLayer, QueryTriggerInteraction.Ignore);
					foreach (Collider collider in colliders)
					{
						WeaponColliderEnter(collider);
					}
					//Ultra.Utilities.DrawBox(hitDetectionGameObject.transform.position, Quaternion.identity, lastAttack.data.boxDimensions, Color.red, 200);
					Ultra.Utilities.DrawBox(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, gameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation, CurrentAction.data.boxDimensions, Color.red);
				}
				break;
			case EHitDetectionType.Capsul:
				{
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
					//gameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation = Quaternion.identity;
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.Translate(CurrentAction.data.offset);
					Collider[] colliders = Ultra.Utilities.OverlapCapsule(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, CurrentAction.data.capsulHeight, CurrentAction.data.radius, GameCharacter.CharacterLayer, QueryTriggerInteraction.Ignore);
					foreach (Collider collider in colliders)
					{
						WeaponColliderEnter(collider);
					}
					Ultra.Utilities.DrawCapsule(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, gameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation, CurrentAction.data.capsulHeight, CurrentAction.data.radius, Color.red);
					//Ultra.Utilities.DrawWireCapsule(hitDetectionGameObject.transform.position, Quaternion.identity, lastAttack.data.radius, lastAttack.data.capsulHeight, Color.red, 200);
					//Gizmos.DrawCube(hitDetectionGameObject.transform.position, Vector3.one);
				}
				break;
			default:
				{
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
					//gameCharacter.CombatComponent.HitDetectionGameObject.transform.rotation = Quaternion.identity;
					gameCharacter.CombatComponent.HitDetectionGameObject.transform.Translate(CurrentAction.data.offset);
					Collider[] colliders = Physics.OverlapSphere(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, CurrentAction.data.radius, GameCharacter.CharacterLayer, QueryTriggerInteraction.Ignore);
					foreach (Collider collider in colliders)
					{
						WeaponColliderEnter(collider);
					}
					Ultra.Utilities.DrawWireSphere(gameCharacter.CombatComponent.HitDetectionGameObject.transform.position, CurrentAction.data.radius, Color.red, 0f, 100);
				}
				break;
		}
	}
	private void TryStartingAction(EExplicitAttackType attackType, float attackDeltaTime)
	{
		if (gameCharacter.PluginStateMachine.IsPluginStatePlugedIn(EPluginCharacterState.DefensiveActionHold))
		{
			if (currentDefensiveAction != null && currentDefensiveAction.Action != null && currentDefensiveAction.Action.HasAttackInputInHold())
			{
				if (CurrentAction != currentDefensiveAction) Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "TryStartingAction", "CurrentAction was not DefensiveAction, but was suppose to!");
				currentDefensiveAction.Action.StartActionInHold();
			}
			else
			{
				EvaluateAndStartAction(attackType, attackDeltaTime);
			}
		}
		else
		{
			EvaluateAndStartAction(attackType, attackDeltaTime);
		}
	}

	private void EvaluateAndStartAction(EExplicitAttackType attackType, float attackDeltaTime)
	{
		if (SetCurrentAttack(attackType, attackDeltaTime))
			CurrentAction?.Action?.StartAction();
	}

	public virtual AttackAnimationData GroundAttack(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.GroundAttacks.Count > 0)
		{
			TryStartingAction(EExplicitAttackType.GroundedDefaultAttack, attackDeltaTime);
			return CurrentAction;
		}
		else
		{
			return null;
		}
	}
	public virtual AttackAnimationData GroundUpAttack(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.GroundUpAttacks.Count > 0)
		{
			TryStartingAction(EExplicitAttackType.GroundedUpAttack, attackDeltaTime);
			return CurrentAction;
		}
		else return GroundAttack(attackDeltaTime);
	}
    public virtual AttackAnimationData GroundDownAttack(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.GroundDownAttacks.Count > 0)
		{
			TryStartingAction(EExplicitAttackType.GroundedDownAttack, attackDeltaTime);
			return CurrentAction;
		}
		else return GroundAttack(attackDeltaTime);
	}
    public virtual AttackAnimationData GroundDirectionAttack(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.GroundDirectionAttacks.Count > 0)
		{
			TryStartingAction(EExplicitAttackType.GroundedDirectionalAttack, attackDeltaTime);
			return CurrentAction;
		}
		else return GroundAttack(attackDeltaTime);
	}

    public virtual AttackAnimationData AirAttack(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.AirAttacks.Count > 0)
		{
			TryStartingAction(EExplicitAttackType.AirDefaultAttack, attackDeltaTime);
			return CurrentAction;
		}
		else
		{
			return null;
		}
	}
    public virtual AttackAnimationData AirUpAttack(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.AirUpAttacks.Count > 0)
		{
			TryStartingAction(EExplicitAttackType.AirUpAttack, attackDeltaTime);
			return CurrentAction;
		}
		else return AirAttack(attackDeltaTime);
	}
    public virtual AttackAnimationData AirDownAttack(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.AirDownAttacks.Count > 0)
		{
			TryStartingAction(EExplicitAttackType.AirDownAttack, attackDeltaTime);
			return CurrentAction;
		}
		else return AirAttack(attackDeltaTime);
	}
    public virtual AttackAnimationData AirDirectionAttack(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.AirDirectionAttacks.Count > 0)
		{
			TryStartingAction(EExplicitAttackType.AirDirectionalAttack, attackDeltaTime);
			return CurrentAction;
		}
		else return AirAttack(attackDeltaTime);
	}

	public virtual AttackAnimationData GapCloserAttack(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.GapCloserAttacks.Count > 0)
		{
			TryStartingAction(EExplicitAttackType.GapCloser, attackDeltaTime);
			return CurrentAction;
		}
		return null;
	}

	public virtual AttackAnimationData Ultimate(float attackDeltaTime)
	{
		if (AnimationData == null) return null;
		if (AnimationData.Ultimate.Count > 0 && UltCharge >= weaponData.MaxUltChargeAmount)
		{
			TryStartingAction(EExplicitAttackType.Ultimate, attackDeltaTime);
			return CurrentAction;
		}
		return null;
	}

	public virtual AttackAnimationData DefensiveAction()
	{
		if (AnimationData == null) return null;
		if (AnimationData.DefensiveAction.Count > 0)
		{
			if (SetCurrentDefensiveAction())
				CurrentAction?.Action?.StartAction();
			return CurrentAction;
		}
		return null;
	}

	public virtual void GroundAttackHit(GameObject hitObj)
	{
		CurrentAction?.Action?.OnHit(hitObj);
	}

	public virtual void GroundUpAttackHit(GameObject hitObj)
	{
		CurrentAction?.Action?.OnHit(hitObj);
	}

	public virtual void GroundDownAttackHit(GameObject hitObj)
	{
		CurrentAction?.Action?.OnHit(hitObj);
	}

	public virtual void GroundDirectionAttackHit(GameObject hitObj)
	{
		CurrentAction?.Action?.OnHit(hitObj);
	}

	public virtual void AirAttackHit(GameObject hitObj)
	{
		CurrentAction?.Action?.OnHit(hitObj);
	}

	public virtual void AirUpAttackHit(GameObject hitObj)
	{
		CurrentAction?.Action?.OnHit(hitObj);
	}

	public virtual void AirDownAttackHit(GameObject hitObj)
	{
		CurrentAction?.Action?.OnHit(hitObj);
	}

	public virtual void AirDirectionAttackHit(GameObject hitObj)
	{
		CurrentAction?.Action?.OnHit(hitObj);
	}

	public virtual void DefensiveActionHit(GameObject hitObj)
	{
		CurrentAction?.Action?.OnHit(hitObj);
	}

	public virtual void SetWeaponReadyPoseBasedOnStates()
	{
 		switch (GameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.InAir:
			case EGameCharacterState.Moving:
				if (GameCharacter.PluginStateMachine.ContainsPluginState(EPluginCharacterState.Aim))
					SetAimWeaponReadyPoseRun();
				else
					SetWeapnReadyPoseRun();
				break;
			default:
				SetWeaponReadyPose();
				break;
		}
	}

	public void SetWeaponReadyPose()
	{
		if (AnimationData != null) gameCharacter.AnimController.SetBodyLayerAnimClip(AnimationData.WeaponReadyPose);
	}

	public void SetWeapnReadyPoseRun()
	{
		if (AnimationData == null || AnimationData.WeaponReadyPoseRun == null) return;

		gameCharacter.AnimController.SetBodyLayerAnimClip(AnimationData.WeaponReadyPoseRun);
	}

	public void SetAimWeaponReadyPoseRun()
	{
		if (AnimationData == null || AnimationData.AimData.AimWeaponReadyPoseRun == null) return;

		gameCharacter.AnimController.SetBodyLayerAnimClip(AnimationData.AimData.AimWeaponReadyPoseRun);
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
			case EExplicitAttackType.GroundedDefaultAttack: return AnimationData.GroundAttacks;
			case EExplicitAttackType.GroundedDownAttack: return AnimationData.GroundDownAttacks;
			case EExplicitAttackType.GroundedUpAttack: return AnimationData.GroundUpAttacks;
			case EExplicitAttackType.GroundedDirectionalAttack: return AnimationData.GroundDirectionAttacks;
			case EExplicitAttackType.AirDefaultAttack: return AnimationData.AirAttacks;
			case EExplicitAttackType.AirDownAttack: return AnimationData.AirDownAttacks;
			case EExplicitAttackType.AirUpAttack: return AnimationData.AirUpAttacks;
			case EExplicitAttackType.AirDirectionalAttack: return AnimationData.AirDirectionAttacks;
			case EExplicitAttackType.GapCloser: return AnimationData.GapCloserAttacks;
			case EExplicitAttackType.DefensiveAction: return AnimationData.DefensiveAction;
			case EExplicitAttackType.Ultimate: return AnimationData.Ultimate;
			default:
				Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "GetAttackListBasedOnEExplicitAttackType", "Coundn't find List, ExplicitAttackType not Implemented");
				return new List<AttackAnimationData>();
		}
	}

	public bool SetCurrentAttack(EExplicitAttackType explicitAttackType, float attackDeltaTime, bool updatedIndex = true)
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

		if (attackList == null || attackList.Count <= 0) return false;
		attackIndex = attackIndex % attackList.Count;
		AttackAnimationData newAttack = attackList[attackIndex];
		currentAttack?.Action?.ActionInterupted();
		currentAttack = newAttack;
		currentAttack?.Action?.Init(GameCharacter, this);
		return true;
	}

	bool CheckForAttackBrenchesInLastAttack(EExplicitAttackType explicitAttackType, bool updatedIndex, float attackDeltaTime)
	{
		if (updatedIndex && CurrentAction != null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("AttackDeltaTime" + attackDeltaTime, 1f, StringColor.Green, 100, DebugAreas.Combat);
			if (AttackDeltatimeIsValid(attackDeltaTime) && CurrentAction.timedCombatBrenches.TryGetValue(explicitAttackType, out int x))
			{
				attackIndex = x;
				return true;
			}
			else if (currentAttackType != EExplicitAttackType.Unknown && CurrentAction.combatBranches.TryGetValue(explicitAttackType, out int i))
			{
				currentAttackType = explicitAttackType;
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

	public bool SetCurrentDefensiveAction(bool updateAttackType = true)
	{
		List<AttackAnimationData> defensiveList = GetAttackListBasedOnEExplicitAttackType(EExplicitAttackType.DefensiveAction);
		if (updateAttackType) currentAttackType = EExplicitAttackType.DefensiveAction;
		defensiveActionIndex++;
		if (defensiveList == null || defensiveList.Count <= 0) return false;
		defensiveActionIndex = defensiveActionIndex % defensiveList.Count;
		AttackAnimationData newDefensiveAction = defensiveList[defensiveActionIndex];
		currentDefensiveAction?.Action?.ActionInterupted();
		currentDefensiveAction = newDefensiveAction;
		currentDefensiveAction?.Action?.Init(GameCharacter, this);
		return true;
	}

	protected void AimBlendSpace(AimBlendAnimations blendTreeAnims, EAnimationType animType)
	{
		if (blendTreeAnims == null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("AttackData was null!", 5f, StringColor.Red, 100, DebugAreas.Combat);
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "AimBlendSpace", "blendTreeAnims was null!"));
			return;
		}
		switch (animType)
		{
			case EAnimationType.Hold: gameCharacter.AnimController.ApplyBlendTree(blendTreeAnims); break;
			case EAnimationType.Trigger: gameCharacter.AnimController.ApplyBlendTree(blendTreeAnims); break;
			default:
				gameCharacter.AnimController.ApplyBlendTree(blendTreeAnims);
				break;
		}
	}

	public void Attack3BlendSpace(AimBlendAnimations blendTreeAnims, EAnimationType animType)
	{
		if (blendTreeAnims == null)
		{
			Ultra.Utilities.Instance.DebugLogOnScreen("AttackData was null!", 5f, StringColor.Red, 100, DebugAreas.Combat);
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "AimBlendSpace", "blendTreeAnims was null!"));
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

	public virtual AttackAnimationData GetAttackAnimationData()
	{
		switch (CurrentAttackType)
		{
			case EExplicitAttackType.DefensiveAction:
				if (lastData != currentDefensiveAction)
				{
					lastData?.Action?.ActionInterupted();
					lastData = currentDefensiveAction;
				}
				return currentDefensiveAction;
			default:
				if (lastData != currentAttack)
				{
					lastData?.Action?.ActionInterupted();
					lastData = currentAttack;
				}
				lastData = currentAttack;
				return currentAttack;
		}
	}

	public virtual void HitDetectionStart()
	{
		if (CurrentAction == null) 
			return;

		if (ishitDetecting) return;

		ishitDetecting = true;
		switch (CurrentAction.data.hitDetectionType)
		{
			case EHitDetectionType.Mesh:
				gameCharacter.CombatComponent.HitDetectionMeshCollider.sharedMesh = CurrentAction.data.mesh;
				gameCharacter.CombatComponent.HitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
				gameCharacter.CombatComponent.HitDetectionGameObject.transform.Translate(CurrentAction.data.offset);
				gameCharacter.CombatComponent.HitDetectionMeshFilter.mesh = CurrentAction.data.mesh;
				gameCharacter.CombatComponent.HitDetectionMeshFilter.sharedMesh = CurrentAction.data.mesh;
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
		switch(CurrentAction.data.hitDetectionType)
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
		if (other.gameObject == gameCharacter.gameObject || IsChildFromParent(other)) return;

		GameCharacter possibleGameCharacter = other.GetComponent<GameCharacter>();
		if (possibleGameCharacter != null && possibleGameCharacter.CheckForSameTeam(gameCharacter.Team)) return;

		if (hitObjects.Contains(other.gameObject)) return;
		hitObjects.Add(other.gameObject);

		if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Red + "Hit Object: " + other.gameObject.name + StringColor.EndColor, (CurrentAction.data.hitDetectionType == EHitDetectionType.Mesh) ? 1f : 0f);
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
			case EExplicitAttackType.DefensiveAction: DefensiveActionHit(other.gameObject); break;
			default: break;
		}
		if (ShouldPlayHitSound)
		{
			PlayHitSound();
			ShouldPlayHitSound = false;
		}
	}

	bool IsChildFromParent(Collider other)
	{
		Transform currentTransform = other.transform;

		// Solange es ein übergeordnetes Transform gibt, setze currentTransform auf das übergeordnete Transform
		while (currentTransform.parent != null)
		{
			currentTransform = currentTransform.parent;
		}

		if (currentTransform.gameObject == GameCharacter.gameObject)
			return true;
		return false;
	}

	public virtual void WeaponColliderExit(Collider other)
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
		CurrentAction?.Action?.StartAttackStateLogic();
	}

	public virtual void PreAttackStateLogic(float deltaTime)
	{
		CurrentAction?.Action?.PreAttackStateLogic(deltaTime);
	}

	public virtual void PostAttackStateLogic(float deltaTime)
	{
		CurrentAction?.Action?.PostAttackStateLogic(deltaTime);
	}

	public virtual void EndAttackStateLogic()
	{
		CurrentAction?.Action?.EndAttackStateLogic();
		hitObjects.Clear();
	}

	public virtual void AttackPhaseStart()
	{
		CurrentAction?.Action?.AttackPhaseStart();
	}

	public virtual void AttackPhaseEnd()
	{
		CurrentAction?.Action?.AttackPhaseEnd();
	}

	public virtual void AttackRecoveryEnd()
	{
		CurrentAction?.Action?.AttackRecoveryEnd();
	}

	public virtual void DefensiveActionStateEnd()
	{
		CurrentAction?.Action?.DefensiveActionStateEnd();
	}

	public virtual void StartParticelEffect(int index)
	{
		switch (currentAttackType)
		{
			case EExplicitAttackType.GroundedDefaultAttack: CheckParticleEffectsBevorPlaying(ref groundLightAttackParticleList, ref CurrentAction.particleList, index, attackIndex); break;
			case EExplicitAttackType.GroundedDirectionalAttack: CheckParticleEffectsBevorPlaying(ref groundHeavyAttackParticleList, ref CurrentAction.particleList, index, attackIndex); break;
			case EExplicitAttackType.GroundedUpAttack: CheckParticleEffectsBevorPlaying(ref groundUpAttackParticleList, ref CurrentAction.particleList, index, attackIndex); break;
			case EExplicitAttackType.GroundedDownAttack: CheckParticleEffectsBevorPlaying(ref groundDownAttackParticleList, ref CurrentAction.particleList, index, attackIndex); break;
			case EExplicitAttackType.AirDefaultAttack: CheckParticleEffectsBevorPlaying(ref airLightAttackParticleList, ref CurrentAction.particleList, index, attackIndex); break;
			case EExplicitAttackType.AirDirectionalAttack: CheckParticleEffectsBevorPlaying(ref airHeavyAttackParticleList, ref CurrentAction.particleList, index, attackIndex); break;
			case EExplicitAttackType.AirDownAttack: CheckParticleEffectsBevorPlaying(ref airDownAttackParticleList, ref CurrentAction.particleList, index, attackIndex); break;
			case EExplicitAttackType.AirUpAttack: CheckParticleEffectsBevorPlaying(ref airUpAttackParticleList, ref CurrentAction.particleList, index, attackIndex); break;
			case EExplicitAttackType.DefensiveAction: CheckParticleEffectsBevorPlaying(ref defensiveActionParticleList, ref CurrentAction.particleList, index, defensiveActionIndex); break;
			default: break;
		}
	}

	void CheckParticleEffectsBevorPlaying(ref List<List<ParticleSystem>> particleListList, ref List<AttackParticleData> weaponParticleList, int index, int particleIndex)
	{
		if (particleListList.Count > attackIndex && particleListList[attackIndex].Count > index && CurrentAction.particleList.Count > index)
			PlayParticleEffect(particleListList[particleIndex][index], weaponParticleList[index]);
	}

	void PlayParticleEffect(ParticleSystem particle, AttackParticleData partilceData)
	{
		particle.transform.parent = GameCharacter.GameCharacterData.Root;
		particle.transform.position = Vector3.zero;
		particle.transform.localPosition = partilceData.ParticleSystem.transform.localPosition;
		particle.transform.localRotation = partilceData.ParticleSystem.transform.localRotation;
		particle.transform.Translate(partilceData.Offset, Space.Self);
		if (partilceData.attachToCharacter)
		{
			if (partilceData.attachToSpecialBone)
			{
				Transform bone = GameCharacter.RigDataComponent.Bones[partilceData.boneName];
				if (bone != null)
				{
					particle.transform.parent = bone;
					particle.transform.localPosition = partilceData.ParticleSystem.transform.localPosition;
					particle.transform.localRotation = partilceData.ParticleSystem.transform.localRotation;
					particle.transform.Translate(partilceData.Offset, Space.Self);
				}
				else
					Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "PlayParticleEffect", "Bone Name was invalid!");
			}
			else
			{
				particle.transform.parent = GameCharacter.GameCharacterData.Root;
			}

			if (partilceData.removeAfterTime)
			{
				ParticleDelayedActionHelper psHelper = particle.GetComponent<ParticleDelayedActionHelper>();
				if (psHelper == null)
				{
					psHelper = particle.AddComponent<ParticleDelayedActionHelper>();
				}
				psHelper.DetachParticleEffectFromParentAfterTime(particle, partilceData.removeTime);
			}
		}
		else
		{
			particle.transform.parent = null;
		}
		particle.Clear(true);
		particle.Play();
		particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		particle.Play();
	}

	void InitParticleForAttackList(ref List<AttackAnimationData> animList, ref List<List<ParticleSystem>> particleList)
	{
		for (int i = 0; i < animList.Count; i++)
		{
			particleList.Add(new List<ParticleSystem>());
			for (int j = 0; j < animList[i].particleList.Count; j++)
			{
				if (animList[i].particleList[j].ParticleSystem == null) continue;
				ParticleSystem particle = GameObject.Instantiate(animList[i].particleList[j].ParticleSystem, GameCharacter.GameCharacterData.Root);
				particle.name = ">> " + particle.name;
				particleList[i].Add(particle);
			}
		}
	}

	public virtual void CharacterArrivedAtRequestedLocation(GameCharacter movedCharacter)
	{
		CurrentAction?.Action?.CharacterArrivedAtRequestedLocation(movedCharacter);
	}

	public virtual void CharacterMoveToAbort(GameCharacter movedCharacter)
	{
		CurrentAction?.Action?.CharacterMoveToAbort(movedCharacter);
	}

	public virtual void CharacterMoveToEnd(GameCharacter movedCharacter)
	{
		CurrentAction?.Action?.CharacterMoveToEnd(movedCharacter);
	}

	public virtual void DefensiveActionStart()
	{
		CurrentAction?.Action?.DefensiveActionStart();
	}

	public virtual void DefensiveActionEnd()
	{
		CurrentAction?.Action?.DefensiveActionEnd();
	}

	public virtual bool CanLeaveDefensiveState()
	{
		if (CurrentAction == null || CurrentAction.Action == null) return true;
		return CurrentAction.Action.CanLeaveDefensiveState();
	}

	public void UnHookAllHookedCharacerts()
	{
		foreach (GameCharacter character in GameCharacter.CombatComponent.HookedCharacters)
		{
			character.CombatComponent.HookedToCharacter = null;
			character.MovementComponent.OverrideMovementByHookedCharacter = false;
		}
		GameCharacter.CombatComponent.HookedCharacters.Clear();
	}

	public void HookCharacterToCharacter(GameCharacter enemyCharacter)
	{
		if (!GameCharacter.CombatComponent.HookedCharacters.Contains(enemyCharacter))
			GameCharacter.CombatComponent.HookedCharacters.Add(enemyCharacter);
	}

	void OnMaxChargeAfterEquipTimerFinished()
	{
		Charge = chargeAfterTime;
	}

	public virtual float GetDamage(float damage)
	{
		//float chargeValue = Ultra.Utilities.Remap(Charge, 0, weaponData.MaxChargeAmount, 0.2f, 1f);
		//Ultra.Utilities.Instance.DebugLogOnScreen("Damage => " + CurrentAttack.extraData.Damage, 1f, StringColor.Magenta);
		return /*chargeValue **/ damage;
	}

	public virtual void SetTriggerAttack(AnimationClip triggerAttack)
	{
		if (triggerAttack == null)
		{
			Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "SetTriggerAttack", "Trigger Attack was null!");
			return;
		}
		gameCharacter.AnimController.SetTriggerAttack(triggerAttack);
		gameCharacter.CombatComponent.AttackTimer.Start(triggerAttack.length);
		gameCharacter.AnimController.AttackTriggerTimer.Start(triggerAttack.length);

		gameCharacter.AnimController.TriggerAttack = true;
		gameCharacter.AnimController.HoldAttack = false;
		gameCharacter.AnimController.InAttack = false;
	}

	public virtual void SetHoldAttack(AnimationClip holdAttack)
	{
		if (holdAttack == null)
		{
			Ultra.Utilities.Instance.DebugErrorString("WeaponBase", "SetHoldAttack", "Hold Attack was null!");
			return;
		}
		gameCharacter.AnimController.SetHoldAttack(holdAttack);
		gameCharacter.CombatComponent.AttackTimer.IsPaused = true;

		gameCharacter.AnimController.HoldAttack = true;
		gameCharacter.AnimController.TriggerAttack = false;
		gameCharacter.AnimController.InAttack = false;
	}

	public virtual void PlayAttackSound(int index = -1)
	{
		if (WeaponData.DefaultAttackSounds.Count <= 0) return;

		index--;
		if (index < 0)
		{
			int randIndex = UnityEngine.Random.Range(0, WeaponData.DefaultAttackSounds.Count);
			if (randIndex == lastAttackSoundIndex)
			{
				randIndex++;
				randIndex %= WeaponData.DefaultAttackSounds.Count;
			}
			var soundClip = WeaponData.DefaultAttackSounds[randIndex];
			SoundManager.Instance.PlaySound(soundClip);
			lastAttackSoundIndex = randIndex;
		}
		else
		{
			var soundClip = WeaponData.DefaultAttackSounds[index];
			SoundManager.Instance.PlaySound(soundClip);
			lastAttackSoundIndex = index;
		}
	}

	public virtual void PlayHitSound()
	{
		if (WeaponData.DefaultHitSounds.Count <= 0) return;

		int randIndex = UnityEngine.Random.Range(0, WeaponData.DefaultHitSounds.Count);
		if (randIndex == lastHitSoundIndex)
		{
			randIndex++;
			randIndex %= WeaponData.DefaultHitSounds.Count;
		}
		var soundClip = WeaponData.DefaultHitSounds[randIndex];
		SoundManager.Instance.PlaySound(soundClip);
		lastHitSoundIndex = randIndex;
	}

	public void SpawnWeaponFlash(WeaponObjData weaponObjData, bool spawnAttached = true)
	{
		ParticleSystemPool pool = GetRangeWeaponFlashParticlePool();
		if (pool == null || pool.ParticleInstance == null) return;
		ParticleSystem ps = pool.GetValue();
		if (spawnAttached)
			ps.transform.parent = weaponObjData.transform;
		else
			ps.transform.parent = null;

		ps.transform.position = weaponObjData.weaponTip.transform.position;
		ps.transform.rotation = weaponObjData.transform.rotation;
	}

	public void SpawnDamageHitEffect(RaycastHit hit)
	{
		GameCharacter gc = hit.collider.GetComponent<GameCharacter>();
		ParticleSystemPool pool = GetRangeWeaponHitParticlePool();
		if (pool == null || pool.ParticleInstance == null) return;
		ParticleSystem ps = pool.GetValue();
		ps.transform.parent = hit.collider.transform;
		if (gc != null) ps.transform.position = gc.MovementComponent.CharacterCenter;
		else ps.transform.position = hit.point;
	}
	public void SpawnDamageHitEffect(GameObject hitObj)
	{
		GameCharacter gc = hitObj.GetComponent<GameCharacter>();
		SpawnDamageHitEffect(gc);
	}
	public void SpawnDamageHitEffect(GameCharacter gc)
	{
		if (gc == null) return;
		ParticleSystemPool pool = GetRangeWeaponHitParticlePool();
		if (pool == null || pool.ParticleInstance == null) return;
		ParticleSystem ps = pool.GetValue();
		ps.transform.parent = gc.transform;
		if (gc != null) ps.transform.position = gc.MovementComponent.CharacterCenter;
	}

	public void AddForceAimBuff(float duration)
	{
 		if (GameCharacter.BuffComponent.IsBuffActive(EBuff.ForceAim))
			GameCharacter.BuffComponent.ResetDurationOffActiveBuff(EBuff.ForceAim);
		else
			GameCharacter.BuffComponent.AddBuff(new ForceAimBuff(GameCharacter, duration));
	}

	public virtual ParticleSystemPool GetRangeWeaponFlashParticlePool()
	{
		return null;
	}

	public virtual ParticleSystemPool GetRangeWeaponHitParticlePool()
	{
		return null;
	}

	public virtual ParticleSystemPool GetWeaponVFXPool(int index)
	{
		return null;
	}

	async void Dleayed_Internal_Init()
	{
		await new WaitForEndOfFrame();
		await new WaitForEndOfFrame();

		CreateWeaponVFXPools();
	}

	protected virtual void CreateWeaponVFXPools()
	{

	}

	public virtual bool SpawnRangedAttackShootPartilceAttached()
	{
		return true;
	}

	public abstract WeaponBase CreateCopy(GameCharacter gameCharacter, ScriptableWeapon weapon);
}
