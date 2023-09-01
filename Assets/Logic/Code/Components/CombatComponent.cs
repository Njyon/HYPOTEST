using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum EAttackType
{
	Default,
	AttackHorizontal,
	AttackUp,
	AttackDown
}

public class CombatComponent
{
	public delegate void OnWeaponChanged(WeaponBase newWeapon, WeaponBase oldWeapon);
	public OnWeaponChanged onWeaponChanged;

	public delegate void OnAttack(ref ShelfList<AttackAnimationData> lastAttacks);
	public OnAttack onAttack;

	GameCharacter gameCharacter;
	ScriptableWeapon[] weapons = new ScriptableWeapon[4];
	WeaponBase currentWeapon;
	WeaponBase nextWeapon;
	int currentWeaponIndex;
	int equipedWeaponAmount;
	Ultra.Timer attackTimer;
	Ultra.Timer defensiveTimer;
	Ultra.Timer flyAwayTimer;
	GameCharacter hookedToCharacter;
	List<GameCharacter> hookedCharacters;
	GameCharacter aimToCharacter;
	Vector3 moveToPosition;
	GameObject hitDetectionGameObject;
	MeshCollider hitDetectionMeshCollider;
	ColliderHitScript hitDetectionColliderScript;
	MeshFilter hitDetectionMeshFilter;
	MeshRenderer hitDetectionMeshRenderer;
	float flyAwayTime = 1f;
	ShelfList<AttackAnimationData> previousAttacks;
	int equipedWeapons = 0;

	public ScriptableWeapon[] Weapons { get { return  weapons; } }
	public Ultra.Timer AttackTimer { get { return attackTimer; } }
	public Ultra.Timer DefensiveTimer { get { return defensiveTimer; } }
	public Ultra.Timer FlyAwayTimer { get { return flyAwayTimer; } }
	public GameCharacter HookedToCharacter { get { return hookedToCharacter; } set { hookedToCharacter = value; } }
	public List<GameCharacter> HookedCharacters { get { return hookedCharacters; } set { hookedCharacters = value; } }
	public GameCharacter AimCharacter { get { return aimToCharacter; } set { aimToCharacter = value; } }
	public Vector3 MoveToPosition { get { return moveToPosition; } set { moveToPosition = value; } }
	public GameObject HitDetectionGameObject { get { return hitDetectionGameObject; } }
	public MeshCollider HitDetectionMeshCollider { get { return hitDetectionMeshCollider; } }
	public ColliderHitScript HitDetectionColliderScript { get { return hitDetectionColliderScript; } }
	public MeshFilter HitDetectionMeshFilter { get { return hitDetectionMeshFilter; } }
	public MeshRenderer HitDetectionMeshRenderer { get { return hitDetectionMeshRenderer; } }
	public float FlyAwayTime { get { return flyAwayTime; } set { flyAwayTime = value; } }
	public ShelfList<AttackAnimationData> PreviousAttacks { get { return previousAttacks; } }
	public int EquipedWeapons { get { return equipedWeapons; } }	

	public WeaponBase NextWeapon
	{
		get { return nextWeapon; }
		set
		{
			if (nextWeapon == value) return;

			nextWeapon = value;
		}
	}

	public WeaponBase CurrentWeapon { 
		get { return currentWeapon; } 
		private set
		{
			if (currentWeapon == value) return;

			WeaponBase oldWeapon = currentWeapon;
			if (oldWeapon != null) oldWeapon.UnEquipWeapon();
			currentWeapon = value;
			if (currentWeapon != null)
			{
				currentWeapon.EquipWeapon();
				if (onWeaponChanged != null) onWeaponChanged(currentWeapon, oldWeapon);
			}
		}
	}

	public CombatComponent(GameCharacter gameCharacter)
	{
		this.gameCharacter = gameCharacter;
		nextWeapon = null;
		hookedCharacters = new List<GameCharacter>();
		previousAttacks = new ShelfList<AttackAnimationData>(gameCharacter.GameCharacterData.CombatAttackListLenght);
	}

	~CombatComponent()
	{
		if (gameCharacter != null)
		{
			gameCharacter.StateMachine.onStateChanged -= OnStateChanged;
		}
	}

	public void SetUpHitDetection()
	{
		hitDetectionGameObject = new GameObject("HitDetectionGameObject");
		if (hitDetectionGameObject == null) return;
		hitDetectionGameObject.transform.position = gameCharacter.MovementComponent.CharacterCenter;
		hitDetectionGameObject.transform.parent = gameCharacter.transform;
		hitDetectionGameObject.transform.localPosition = Vector3.zero;
		hitDetectionGameObject.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
		hitDetectionMeshCollider = hitDetectionGameObject.AddComponent<MeshCollider>();
		if (hitDetectionMeshCollider == null) return;
		hitDetectionMeshCollider.convex = true;
		hitDetectionMeshCollider.isTrigger = true;
		hitDetectionColliderScript = hitDetectionGameObject.AddComponent<ColliderHitScript>();
		if (hitDetectionColliderScript == null) return;
		// Debug
		hitDetectionMeshFilter = hitDetectionGameObject.AddComponent<MeshFilter>();
		hitDetectionMeshRenderer = hitDetectionGameObject.AddComponent<MeshRenderer>();
		hitDetectionMeshRenderer.material = GameAssets.Instance.debugMaterial;
		hitDetectionMeshRenderer.enabled = false;

	}

	public void DestroyHitDestection()
	{
		hitDetectionMeshCollider = null;
		hitDetectionColliderScript = null;
		GameObject.Destroy(hitDetectionGameObject);
	}

	public void StartComponent()
	{
		InitWeapons();
		attackTimer = new Ultra.Timer(0f, true);
		defensiveTimer = new Ultra.Timer(0f, true);
		flyAwayTimer = new Ultra.Timer(0f, true);

		gameCharacter.StateMachine.onStateChanged += OnStateChanged;
	}

	public void UpdateComponent(float deltaTime) 
	{
		if (attackTimer != null) attackTimer.Update(deltaTime);
		if (defensiveTimer != null) defensiveTimer.Update(deltaTime);
		if (flyAwayTimer != null) flyAwayTimer.Update(deltaTime);
		if (CurrentWeapon != null) CurrentWeapon.UpdateWeapon(deltaTime);

		if (NextWeapon != null)
		{
			CheckIfCanSwitchWeapon(gameCharacter.StateMachine.GetCurrentStateType());
		}
	}

	private void CheckIfCanSwitchWeapon(EGameCharacterState state)
	{
		switch (state)
		{
			case EGameCharacterState.Attack:
			case EGameCharacterState.DefensiveAction:
				break;
			default:
				UpdateWeapon();
				break;
		}
	}

	private void UpdateWeapon()
	{
		CurrentWeapon = NextWeapon;
		NextWeapon = null;
	}

	void InitWeapons()
	{
		equipedWeapons = 0;
		equipedWeaponAmount = gameCharacter.GameCharacterData.Weapons.Count;
		for (int i = 0; i < gameCharacter.GameCharacterData.Weapons.Count; i++)
		{
			if (i >= 4) break;
			weapons[i] = gameCharacter.GameCharacterData.Weapons[i];
			WeaponBase[] weaponArray = Ultra.Utilities.GetAll<WeaponBase>().ToArray();
			for (int j = 0; j < weaponArray.Length; j++)
			{
				if (weaponArray[j].GetType().Name == weapons[i].WeaponClassName)
				{
					weapons[i].Weapon = Activator.CreateInstance(weaponArray[j].GetType(), gameCharacter, weapons[i]) as WeaponBase;
					equipedWeapons++;
					break;
				}
			}
		}
		if (weapons.Length >= 1 && weapons[0] != null && weapons[0].Weapon != null)
		{
			for (int i = 0; i < weapons.Length; i++) 
			{
				weapons[i]?.Weapon?.InitWeapon();
			}

			currentWeaponIndex = 0;
			NextWeapon = weapons[currentWeaponIndex].Weapon;
		}
	}

	public void SwitchWeapon(int index)
	{
		if (weapons.Length >= index && weapons[index] != null && weapons[index].Weapon != null)
		{
			currentWeaponIndex = index;
			NextWeapon = weapons[currentWeaponIndex].Weapon;
		}
	}

	public void EquipNextWeapon()
	{
		int index = currentWeaponIndex + 1;
		index %= equipedWeaponAmount;
		SwitchWeapon(index);
	}

	public void EquipPreviousWeapon()
	{
		int index = currentWeaponIndex - 1;
		if (index < 0) index = equipedWeaponAmount - 1;
		SwitchWeapon(index);
	}

	public void Attack(EAttackType attackType)
	{
		if (NextWeapon != null)
			UpdateWeapon();

		// Used for attackTracking for Evaluate CombatRating
		AttackAnimationData newAttack = null;

		if (gameCharacter.MovementComponent.IsGrounded)
		{
			switch (attackType)
			{
				case EAttackType.Default: newAttack = CurrentWeapon?.GroundAttack(); break;
				case EAttackType.AttackHorizontal: newAttack = CurrentWeapon?.GroundDirectionAttack(); break;
				case EAttackType.AttackUp: newAttack = CurrentWeapon?.GroundUpAttack(); break;
				case EAttackType.AttackDown: newAttack = CurrentWeapon?.GroundDownAttack(); break;
				default: break;
			}
		}else
		{
			switch (attackType)
			{
				case EAttackType.Default: newAttack = CurrentWeapon?.AirAttack(); break;
				case EAttackType.AttackHorizontal: newAttack = CurrentWeapon?.AirDirectionAttack(); break;
				case EAttackType.AttackUp: newAttack = CurrentWeapon?.AirUpAttack(); break;
				case EAttackType.AttackDown: newAttack = CurrentWeapon?.AirDownAttack(); break;
				default: break;
			}
		}

		if (newAttack != null)
		{
			previousAttacks.Add(newAttack);
			if (onAttack != null) onAttack(ref previousAttacks);
		}
	}

	public void DefensiveAction()
	{
		if (NextWeapon != null)
			UpdateWeapon();

		CurrentWeapon?.DefensiveAction();
	}

	void OnStateChanged(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		if (NextWeapon != null) 
			CheckIfCanSwitchWeapon(newState.GetStateType());
	}

	public bool CanRequestFlyAway()
	{
		return true;
	}

	public bool CanRequestFreez()
	{
		return true;
	}

	public void RequestFreez(float freezTime = 1f)
	{
		if (CanRequestFreez())
		{
			if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Freez)
			{
				gameCharacter.AddFreezTime(freezTime);
			}else
			{
				gameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
				gameCharacter.FreezTimeOverride = freezTime;
			}
		}
	}
}
