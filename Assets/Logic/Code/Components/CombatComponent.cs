using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public enum EAttackType
{
	Default,
	AttackHorizontal,
	AttackUp,
	AttackDown,
	GapCloser
}

public enum EComboChangeType
{
	Add,
	TimerEnds,
	Break,
}

public class CheckBase
{
	public bool Value;
}

public class PositionCheck : CheckBase
{
	public Vector3 Position;
}

public class CombatComponent
{
	public delegate void OnWeaponChanged(WeaponBase newWeapon, WeaponBase oldWeapon, GameCharacter gameCharacter);
	public OnWeaponChanged onWeaponChanged;
	public OnWeaponChanged onNextWeapon;

	public delegate void OnComboCountChanged(int newComboCount, int oldColboCount, EComboChangeType type);
	public OnComboCountChanged onComboCountChanged;

	public delegate void OnCharacterDamagedCharacter(GameCharacter attackingCharacter, GameCharacter damageCharacter, float damage);
	public OnCharacterDamagedCharacter onCharacterDamagedCharacter;

	public delegate void OnAttack(ref ShelfList<AttackAnimationData> lastAttacks);
	public OnAttack onAttack;

	public delegate void OnDodgeLeftChanged(int newDodgeAmount, int oldDodgeAmount);
	public OnDodgeLeftChanged onDodgeLeftChanged;

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
	int comboCount = 0;
	Ultra.Timer comboTimer = new Ultra.Timer();
	float comboTime = 2f;
	int dodgesLeft = 0;
	Ultra.Timer dodgeRecoveryTimer;
	PositionCheck aimPositionCheck;
	bool allowEarlyLeaveAttackRecovery = false;

	public ScriptableWeapon[] Weapons { get { return weapons; } }
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
	public int ComboCount { get { return comboCount; } }
	public Ultra.Timer DodgeRecoveryTimer { get { return dodgeRecoveryTimer; } }
	public PositionCheck AimPositionCheck { get { return aimPositionCheck; } }
	public bool AllowEarlyLeaveAttackRecovery { get { return allowEarlyLeaveAttackRecovery; } set { allowEarlyLeaveAttackRecovery = value; } }
	public int DodgesLeft { get { return dodgesLeft; }  
		private set {
			int oldDodges = DodgesLeft;
			dodgesLeft = Mathf.Clamp(value, 0, gameCharacter.GameCharacterData.MaxDodgeAmount); 

			if (dodgesLeft == gameCharacter.GameCharacterData.MaxDodgeAmount)
			{
				if (dodgeRecoveryTimer.IsRunning)
					dodgeRecoveryTimer.Stop();
			}
			if (dodgesLeft < gameCharacter.GameCharacterData.MaxDodgeAmount)
			{
				if (!dodgeRecoveryTimer.IsRunning)
				{
					dodgeRecoveryTimer.onTimerFinished += OnDodgeRecoveryTimerFinished;
					dodgeRecoveryTimer.Start();
				}
			}

			if (DodgesLeft != oldDodges)
				if (onDodgeLeftChanged != null) onDodgeLeftChanged(DodgesLeft, oldDodges);
		} 
	}

	public WeaponBase NextWeapon
	{
		get { return nextWeapon; }
		set
		{
			if (nextWeapon == value) return;

			WeaponBase oldNextWeapon = nextWeapon;
			nextWeapon = value;

			if (onNextWeapon != null) onNextWeapon(nextWeapon, oldNextWeapon, gameCharacter);
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
				if (onWeaponChanged != null) onWeaponChanged(currentWeapon, oldWeapon, gameCharacter);
			}
		}
	}

	public CombatComponent(GameCharacter gameCharacter)
	{
		this.gameCharacter = gameCharacter;
		nextWeapon = null;
		hookedCharacters = new List<GameCharacter>();
		previousAttacks = new ShelfList<AttackAnimationData>(gameCharacter.GameCharacterData.CombatAttackListLenght);
		dodgesLeft = gameCharacter.GameCharacterData.MaxDodgeAmount;
		dodgeRecoveryTimer = new Ultra.Timer(gameCharacter.GameCharacterData.DodgeRecoveryTime, true);
		aimPositionCheck = new PositionCheck();
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
		hitDetectionGameObject = new GameObject(">> HitDetectionGameObject");
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
		comboTimer = new Ultra.Timer(comboTime, true);
		comboTimer.onTimerFinished += OnComboTimerFinished;

		gameCharacter.StateMachine.onStateChanged += OnStateChanged;
	}

	public void UpdateComponent(float deltaTime) 
	{
		if (attackTimer != null) attackTimer.Update(deltaTime);
		if (defensiveTimer != null) defensiveTimer.Update(deltaTime);
		if (flyAwayTimer != null) flyAwayTimer.Update(deltaTime);
		if (comboTimer != null) comboTimer.Update(deltaTime);
		if (dodgeRecoveryTimer != null) dodgeRecoveryTimer.Update(deltaTime);
		Profiler.BeginSample("UpdateWeapon");
		foreach (ScriptableWeapon scriptableWeapon in weapons)
		{
			scriptableWeapon?.Weapon?.UpdateWeapon(deltaTime);
		}
		Profiler.EndSample();
		//if (CurrentWeapon != null) CurrentWeapon.UpdateWeapon(deltaTime);

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

			if (weapons[i] == null) continue;

			weapons[i] = weapons[i].CreateCopy();
			weapons[i]?.CreateWeapon(gameCharacter);

			//switch (weapons[i].WeaponClassName)
			//{
			//	case "FistWeapon":	weapons[i].Weapon = new FistWeapon(gameCharacter, weapons[i]); break;
			//	case "SpearWeapon": weapons[i].Weapon = new SpearWeapon(gameCharacter, weapons[i]); break;
			//	case "SwordWeapon": weapons[i].Weapon = new SwordWeapon(gameCharacter, weapons[i]); break;
			//	case "TommyGun":	weapons[i].Weapon = new TommyGun(gameCharacter, weapons[i]); break;
			//	default:
			//		Ultra.Utilities.Instance.DebugErrorString("CombatComponent", "InitWeapons", "No Valid Weapon Implementation Found!");
			//		break;
			//}
			if (weapons[i].Weapon != null)
			{
				equipedWeapons++;
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

	public void Attack(EAttackType attackType, float deltaAttackTime = -1f)
	{
		if (NextWeapon != null)
			UpdateWeapon();

		// Used for attackTracking for Evaluate CombatRating
		AttackAnimationData newAttack = null;

		if (gameCharacter.MovementComponent.IsGrounded)
		{
			switch (attackType)
			{
				case EAttackType.Default: newAttack = CurrentWeapon?.GroundAttack(deltaAttackTime); break;
				case EAttackType.AttackHorizontal: newAttack = CurrentWeapon?.GroundDirectionAttack(deltaAttackTime); break;
				case EAttackType.AttackUp: newAttack = CurrentWeapon?.GroundUpAttack(deltaAttackTime); break;
				case EAttackType.AttackDown: newAttack = CurrentWeapon?.GroundDownAttack(deltaAttackTime); break;
				case EAttackType.GapCloser: newAttack = CurrentWeapon?.GapCloserAttack(deltaAttackTime); break;
				default: break;
			}
		}else
		{
			switch (attackType)
			{
				case EAttackType.Default: newAttack = CurrentWeapon?.AirAttack(deltaAttackTime); break;
				case EAttackType.AttackHorizontal: newAttack = CurrentWeapon?.AirDirectionAttack(deltaAttackTime); break;
				case EAttackType.AttackUp: newAttack = CurrentWeapon?.AirUpAttack(deltaAttackTime); break;
				case EAttackType.AttackDown: newAttack = CurrentWeapon?.AirDownAttack(deltaAttackTime); break;
				case EAttackType.GapCloser: newAttack = CurrentWeapon?.GapCloserAttack(deltaAttackTime); break;
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
		if (gameCharacter.IsGameCharacterDead) return false;
		return gameCharacter.StaggerComponent.IsStaggerable;
	}

	public bool CanRequestFreez()
	{
		if (gameCharacter.IsGameCharacterDead) return false;
		return gameCharacter.StaggerComponent.IsStaggerable;
	}

	public bool CanGetHooked()
	{
		if (gameCharacter.IsGameCharacterDead) return false;
		return gameCharacter.StaggerComponent.IsStaggerable;
	}

	public bool CanRequestMoveTo()
	{
		if (gameCharacter.IsGameCharacterDead) return false;
		return gameCharacter.StaggerComponent.IsStaggerable;
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
				gameCharacter.StateMachine.AddLazyState(EGameCharacterState.Freez);
				gameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
				gameCharacter.FreezTimeOverride = freezTime;
			}
		}
	}

	public void AddCombo()
	{
		SetCombo(ComboCount + 1, EComboChangeType.Add);
		comboTimer.Start(comboTime);
	}

	void OnComboTimerFinished()
	{
		SetCombo(0, EComboChangeType.TimerEnds);
	}

	public void ComboBreak()
	{
		SetCombo(0, EComboChangeType.Break);
	}

	void SetCombo(int newCombo, EComboChangeType type)
	{
		if (newCombo == comboCount) return;

		int oldComboCount = comboCount;
		comboCount = newCombo;

		if (onComboCountChanged != null) onComboCountChanged(comboCount, oldComboCount, type);
	}

	public void CharacterDidDamageTo(GameCharacter damagedCharacter, float damage)
	{
		DodgesLeft++;
		if (onCharacterDamagedCharacter != null) onCharacterDamagedCharacter(gameCharacter, damagedCharacter, damage);
	}

	public void Dodge()
	{
		if (DodgesLeft > 0)
		{
			DodgesLeft--;
			gameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Dodge);
		}else
		{
			// TODO: Add Fail feedback
		}
	}

	public async void FreezAfterPush(GameCharacter character, float waitTime, float freezTime)
	{
		await new WaitForSeconds(waitTime);
		character.CombatComponent.RequestFreez(freezTime);
	}

	public void RequestFlyAway(float flyAwayTime)
	{
		if (FlyAwayTimer.IsRunning)
			FlyAwayTimer.AddTime(flyAwayTime);
		else 
			FlyAwayTime = flyAwayTime;
		gameCharacter.StateMachine.RemoveLazyState(EGameCharacterState.Freez);
		gameCharacter.StateMachine.ForceStateChange(EGameCharacterState.FlyAway, true);
	}

	public void SuccsessfullDodge(GameCharacter damageInitiator, float damage)
	{
		var ps = gameCharacter.SuccsessfullDodgeParticlePool.GetValue();
		ps.transform.position = gameCharacter.MovementComponent.CharacterCenter;
		ps.transform.rotation = Quaternion.identity;
	}

	void OnDodgeRecoveryTimerFinished()
	{
		dodgeRecoveryTimer.onTimerFinished -= OnDodgeRecoveryTimerFinished;
		DodgesLeft++;
	}

	public float GetDodgeRecovertTimerProgress()
	{
		if (dodgeRecoveryTimer.IsRunning)
			return Mathf.Lerp(0, dodgeRecoveryTimer.Time, dodgeRecoveryTimer.CurrentTime);
		else 
			return 1;
	}
}
