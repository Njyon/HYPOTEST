using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
}

public abstract class WeaponBase
{
    GameCharacter gameCharacter;
    ScriptableWeapon weaponData;
    GameObject spawnedWeapon;
    GameObject spawnedWeaponBones;
	EExplicitAttackType currentAttackType;
	EExplicitAttackType lastAttackType;
    int attackIndex;
	bool ishitDetecting = false;
	GameObject hitDetectionGameObject;
	MeshCollider hitDetectionMeshCollider;
	ColliderHitScript hitDetectionColliderScript;
	MeshFilter hitDetectionMeshFilter;
	MeshRenderer hitDetectionMeshRenderer;
	AttackAnimationData currentAttack;
	protected List<GameObject> hitObjects;

	public GameCharacter GameCharacter { get { return gameCharacter; } }
    public ScriptableWeapon WeaponData { get { return weaponData; } }
    public GameObject SpawnedWeapon { get { return spawnedWeapon; } }
    public GameObject SpawnedWeaponBones { get { return spawnedWeaponBones; } }
	public bool IsHitDetecting { get { return ishitDetecting; } }
	public AttackAnimationData LastAttack { get { return currentAttack; } }

    public WeaponBase() { }
    public WeaponBase(GameCharacter gameCharacter, ScriptableWeapon weaponData)
    {
        this.gameCharacter = gameCharacter;
        this.weaponData = weaponData;
		hitObjects = new List<GameObject>();
    }

    public virtual void EquipWeapon()
	{
		SetUpWeaponAnimationData();
		GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.WeaponReady);
		SpawnVisualWeaponMesh();
		SetUpHitDetectionMeshLogic();
		gameCharacter.StateMachine.onStateChanged += OnGameCharacterStateChange;
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
				default:
					break;
			}
		}
	}

	private void SetUpHitDetectionMeshLogic()
	{
		hitDetectionGameObject = new GameObject("HitDetectionGameObject");
		if (hitDetectionGameObject == null) return;
		hitDetectionGameObject.transform.parent = gameCharacter.transform;
		hitDetectionGameObject.transform.position = Vector3.zero;
		hitDetectionMeshCollider = hitDetectionGameObject.AddComponent<MeshCollider>();
		if (hitDetectionMeshCollider == null) return;
		hitDetectionMeshCollider.convex = true;
		hitDetectionMeshCollider.isTrigger = true;
		hitDetectionColliderScript = hitDetectionGameObject.AddComponent<ColliderHitScript>();
		if (hitDetectionColliderScript == null) return;
		hitDetectionColliderScript.onOverlapEnter += WeaponColliderEnter;
		hitDetectionColliderScript.onOverlapExit += WeaponColliderExit;
		// Debug
		hitDetectionMeshFilter = hitDetectionGameObject.AddComponent<MeshFilter>();
		hitDetectionMeshRenderer = hitDetectionGameObject.AddComponent<MeshRenderer>();
		hitDetectionMeshRenderer.material = GameAssets.Instance.debugMaterial;
		hitDetectionMeshRenderer.enabled = false;
	}

	public virtual void UnEquipWeapon()
	{
		GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.WeaponReady);
        GameObject.Destroy(spawnedWeapon);
        GameObject.Destroy(spawnedWeaponBones);
		if (hitDetectionColliderScript != null) hitDetectionColliderScript.onOverlapEnter -= WeaponColliderEnter;
		if (hitDetectionColliderScript != null) hitDetectionColliderScript.onOverlapExit -= WeaponColliderExit;
		hitDetectionMeshCollider = null;
		hitDetectionColliderScript = null;
		GameObject.Destroy(hitDetectionGameObject);

	}

    public virtual void UpdateWeapon(float deltaTime)
	{
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
					hitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
					hitDetectionGameObject.transform.Translate(currentAttack.data.offset);
					Collider[] colliders = Physics.OverlapBox(hitDetectionGameObject.transform.position, currentAttack.data.boxDimensions / 2);
					foreach (Collider collider in colliders)
					{
						WeaponColliderEnter(collider);
					}
					//Ultra.Utilities.DrawBox(hitDetectionGameObject.transform.position, Quaternion.identity, lastAttack.data.boxDimensions, Color.red, 200);
					Ultra.Utilities.DrawBox(hitDetectionGameObject.transform.position, Quaternion.identity, currentAttack.data.boxDimensions, Color.red);
				}
				break;
			case EHitDetectionType.Capsul:
				{
					hitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
					hitDetectionGameObject.transform.Translate(currentAttack.data.offset);
					Collider[] colliders = Ultra.Utilities.OverlapCapsule(hitDetectionGameObject.transform.position, currentAttack.data.capsulHeight, currentAttack.data.radius);
					foreach (Collider collider in colliders)
					{
						WeaponColliderEnter(collider);
					}
					Ultra.Utilities.DrawCapsule(hitDetectionGameObject.transform.position, Quaternion.identity,currentAttack.data.capsulHeight, currentAttack.data.radius, Color.red);
					//Ultra.Utilities.DrawWireCapsule(hitDetectionGameObject.transform.position, Quaternion.identity, lastAttack.data.radius, lastAttack.data.capsulHeight, Color.red, 200);
					//Gizmos.DrawCube(hitDetectionGameObject.transform.position, Vector3.one);
				}
				break;
			default:
				{
					hitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
					hitDetectionGameObject.transform.Translate(currentAttack.data.offset);
					Collider[] colliders = Physics.OverlapSphere(hitDetectionGameObject.transform.position, currentAttack.data.radius);
					foreach (Collider collider in colliders)
					{
						WeaponColliderEnter(collider);
					}
					Ultra.Utilities.DrawWireSphere(hitDetectionGameObject.transform.position, currentAttack.data.radius, Color.red, 0f, 100);
				}
				break;
		}
	}

	public virtual void GroundAttack()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		BaseAttackLogic(EExplicitAttackType.GroundedDefaultAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundAttacks);
	}
	public virtual void GroundUpAttack()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundUpAttacks.Count > 0) BaseAttackLogic(EExplicitAttackType.GroundedUpAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundUpAttacks);
		else GroundAttack();
	}
    public virtual void GroundDownAttack()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDownAttacks.Count > 0) BaseAttackLogic(EExplicitAttackType.GroundedDownAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDownAttacks);
		else GroundAttack();
	}
    public virtual void GroundDirectionAttack()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDirectionAttacks.Count > 0) BaseAttackLogic(EExplicitAttackType.GroundedDirectionalAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].GroundDirectionAttacks);
		else GroundAttack();
	}

    public virtual void AirAttack()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		BaseAttackLogic(EExplicitAttackType.AirDefaultAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirAttacks);
	}
    public virtual void AirUpAttack()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirUpAttacks.Count > 0) BaseAttackLogic(EExplicitAttackType.AirUpAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirUpAttacks);
		else AirAttack();
	}
    public virtual void AirDownAttack()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks.Count > 0) BaseAttackLogic(EExplicitAttackType.AirDownAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDownAttacks);
		else AirAttack();
	}
    public virtual void AirDirectionAttack()
	{
		if (!WeaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) return;
		if (WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDirectionAttacks.Count > 0) BaseAttackLogic(EExplicitAttackType.AirDirectionalAttack, ref WeaponData.AnimationData[GameCharacter.CharacterData.Name].AirDirectionAttacks);
		else AirAttack();
	}

	public virtual void GroundAttackHit(GameObject hitObj)
	{

	}

	public virtual void GroundUpAttackHit(GameObject hitObj)
	{

	}

	public virtual void GroundDownAttackHit(GameObject hitObj)
	{

	}

	public virtual void GroundDirectionAttackHit(GameObject hitObj)
	{

	}

	public virtual void AirAttackHit(GameObject hitObj)
	{

	}

	public virtual void AirUpAttackHit(GameObject hitObj)
	{

	}

	public virtual void AirDownAttackHit(GameObject hitObj)
	{

	}

	public virtual void AirDirectionAttackHit(GameObject hitObj)
	{

	}

	public virtual void AttackEnd()
	{

	}

	void SetUpWeaponAnimationData()
	{
		if (weaponData.AnimationData.ContainsKey(GameCharacter.CharacterData.Name)) gameCharacter.AnimController.SetBodyLayerAnimClip(weaponData.AnimationData[GameCharacter.CharacterData.Name].WeaponReadyPose);
	}

	protected IDamage GetDamageInterface(GameObject obj)
	{
		return obj.GetComponent<IDamage>();
	}

	private void BaseAttackLogic(EExplicitAttackType explicitAttackType, ref List<AttackAnimationData> attackList)
	{
		if (currentAttackType != explicitAttackType)
		{
			attackIndex = 0;
			currentAttackType = explicitAttackType;
		}
		else
		{
			attackIndex++;
		}
		if (attackList == null || attackList.Count <= 0) return;
		attackIndex = attackIndex % attackList.Count;
		currentAttack = attackList[attackIndex];
		gameCharacter.AnimController.Attack(currentAttack.clip);
		gameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Attack);
		gameCharacter.CombatComponent.AttackTimer.Start(currentAttack.clip.length);
	}

	public virtual void HitDetectionStart()
	{
		if (currentAttack == null) return;

		ishitDetecting = true;
		switch (currentAttack.data.hitDetectionType)
		{
			case EHitDetectionType.Mesh:
				hitDetectionMeshCollider.sharedMesh = currentAttack.data.mesh;
				Vector3 localpos;
				Quaternion localrot;
				hitDetectionGameObject.transform.GetLocalPositionAndRotation(out localpos, out localrot);
				hitDetectionGameObject.transform.Translate(localpos * -1, Space.Self);
				hitDetectionGameObject.transform.Translate(currentAttack.data.offset, Space.Self);
				hitDetectionMeshFilter.mesh = currentAttack.data.mesh;
				hitDetectionMeshFilter.sharedMesh = currentAttack.data.mesh;
#if UNITY_EDITOR
				if (Ultra.Utilities.Instance.debugLevel >= 100) hitDetectionMeshRenderer.enabled = true;
#endif
				// Happens at the end of frame where collision got updated from movement above
				InitialMeshColliderCheck();
				break;
			default:
				break;
		}
	}

	async void InitialMeshColliderCheck()
	{
		await new WaitForEndOfFrame();
		foreach (Collider collider in hitDetectionColliderScript.OverlappingColliders)
		{
			WeaponColliderEnter(collider);
		}
	}

	public virtual void HitDetectionEnd() 
	{
		ishitDetecting = false;
		switch(currentAttack.data.hitDetectionType)
		{
			case EHitDetectionType.Mesh: hitDetectionMeshRenderer.enabled = false; break;
				default: break;
		}
		AttackEnd();
	}

	protected virtual void WeaponColliderEnter(Collider other)
	{
		if (!IsHitDetecting) return;
		if (other.gameObject == gameCharacter.gameObject) return;

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
			hitObjects.Clear();
		}

		if (newState?.GetStateType() != EGameCharacterState.Attack && oldState?.GetStateType() == EGameCharacterState.AttackRecovery)
		{
			lastAttackType = currentAttackType;
			currentAttackType = EExplicitAttackType.Unknown;
		}
	}
}
