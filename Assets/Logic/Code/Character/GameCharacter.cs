using EasyButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour , IDamage
{
	public delegate void OnGameCharacterDied(GameCharacter gameCharacter);
	public OnGameCharacterDied onGameCharacterDied;
	public OnGameCharacterDied onGameCharacterDestroyed;

	GameCharacterStateMachine stateMachine;
	GameCharacterPluginStateMachine pluginStateMachine;
	GameCharacterData gameCharacterData;
	EventComponent eventComponent;
	Animator animator;
	AnimationController animController;
	ScriptableCharacter characterData;
	CombatComponent combatComponent;
	GameCharacterMovementComponent movementComponent;
	RigDataComponent rigDataComponent;
	Rigidbody rigidbody;
	Vector2 movementInput;
	int currentJumpAmount = 0;
	bool isPlayerCharacter = false;
	RecourceBase health;
	StaggerComponent staggerComponent;
	Ultra.Timer freezTimer;
	float freezTime = 1f;
	float freezTimeOverride = 0f;
	float characterRadiusTarget;
	float characterHeightTarget;
	CharacterDetection characterDetection;
	bool isGameCharacterDead = false;
	HyppoliteTeam team;
	Quaternion rotationTarget;
	Vector3 lastDir;

	bool isInitialized = false;
	public bool IsInitialized { get { return isInitialized; } }

	public GameCharacterStateMachine StateMachine { get { return stateMachine; } }
	public Vector2 MovementInput { get { return movementInput; } }
	public GameCharacterData GameCharacterData { get { return gameCharacterData; } }
	public int CurrentJumpAmount { get { return currentJumpAmount; } set { currentJumpAmount = value; } }
	public Animator Animator { get { return animator; } }
	public ScriptableCharacter CharacterData { get { return characterData; } set { characterData = value; } }
	public AnimationController AnimController { get { return animController; } }
	public CombatComponent CombatComponent { get { return combatComponent; } }
	public GameCharacterPluginStateMachine PluginStateMachine { get { return pluginStateMachine; } }
	public EventComponent EventComponent { get { return eventComponent; } }
	public GameCharacterMovementComponent MovementComponent { get { return movementComponent; } }
	public Rigidbody Rigidbody { get { return rigidbody; } }
	public bool IsPlayerCharacter { get {  return isPlayerCharacter; } set { isPlayerCharacter = value; } }
	public RecourceBase Health { get { return health; } }
	public StaggerComponent StaggerComponent { get { return staggerComponent; } }
	public Ultra.Timer FreezTimer { get { return freezTimer; } }
	public float FreezTime { get { return freezTime; } }	
	public float FreezTimeOverride { get { return freezTimeOverride; } set { freezTimeOverride = value; } }	
	public CharacterDetection CharacterDetection { get { return characterDetection; } }
	public RigDataComponent RigDataComponent { get { return rigDataComponent; } }
	public Quaternion RotationTarget { get { return rotationTarget; } set { rotationTarget = value; } }
	public Vector3 LastDir { get { return lastDir; } set { lastDir = value; } }


	public HyppoliteTeam Team { get { return team; } set { team = value; } }
	public bool IsGameCharacterDead 
	{ 
		get { return isGameCharacterDead; } 
		private set { isGameCharacterDead = value; }
	}
	public float CharacterRadiusTarget
	{
		get { return characterRadiusTarget; }
		set { characterRadiusTarget = value; }
	}
	public float CharacterHeightTarget
	{
		get { return characterHeightTarget; }
		set { characterHeightTarget = value; }
	}

	public void HorizontalMovementInput(float Haxis)
	{
		movementInput.x = Haxis;
	}
	public void VerticalMovmentInput(float Vaxis)
	{
		movementInput.y = Vaxis;
	}
	public Vector2 GetHorizontalMovementInputDir()
	{
		return new Vector2(MovementInput.x, 0f);
	}
	private void KillZOffset()
	{
		//Vector3 pos = transform.position;
		///// Kill Z so character is always on Z 0
		//pos.z = 0;
		//transform.position = pos;
	}

	protected virtual void Awake()
	{
		animator = gameObject.GetComponent<Animator>();
		if (animator == null) Debug.LogError("GameObject: " + name + " Does not have an Animator Attached!");
		rigidbody = gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null) Debug.LogError("GameObject: " + name + " Does not have an Rigibody Attached!");

		freezTimer = new Ultra.Timer(freezTime, true);

	}

	public virtual void CustomAwake()
	{
		rigDataComponent = gameObject.AddComponent<RigDataComponent>();
		movementComponent = gameObject.GetComponent<GameCharacterMovementComponent>();
		if (movementComponent == null) Debug.LogError("GameObject: " + name + " Does not have an GameCharacterMovementComponent Attached!");
		eventComponent = new EventComponent();
		gameCharacterData = gameObject.GetComponent<GameCharacterData>();
		if (!gameCharacterData) gameObject.AddComponent<GameCharacterData>();
		stateMachine = gameObject.AddComponent<GameCharacterStateMachine>();
		pluginStateMachine = gameObject.AddComponent<GameCharacterPluginStateMachine>();
		pluginStateMachine.Init(this);
		animController = new AnimationController(this);
		combatComponent = new CombatComponent(this);

		GameObject characterDetectionObject = GameObject.Instantiate(GameAssets.Instance.characterDetection, transform);
		characterDetection = characterDetectionObject.GetComponent<CharacterDetection>();
		characterDetection.onOverlapEnter += OnCharacterDetectionOverlapEnter;
		characterDetection.Collider.radius = gameCharacterData.CharacterDetectionRange;

		animController.Start();
		combatComponent.StartComponent();

		health = new RecourceBase(gameCharacterData.Health, gameCharacterData.Health);
		health.onCurrentValueChange += OnHealthValueChanged;

		staggerComponent = new StaggerComponent(this, gameCharacterData.StaggerTime, gameCharacterData.MaxStaggerValue, gameCharacterData.MaxStaggerValue);

		PluginStateMachine.AddPluginState(EPluginCharacterState.LookInVelocityDirection);

		isInitialized = true;
	}

	protected void OnDestroy()
	{
		if (health != null)
		{
			health.onCurrentValueChange -= OnHealthValueChanged;
		}

		if (characterDetection != null)
		{
			characterDetection.onOverlapEnter -= OnCharacterDetectionOverlapEnter;
		}

		if (onGameCharacterDestroyed != null) onGameCharacterDestroyed(this);
	}

	protected void Update()
	{
		if (!IsInitialized) return;
		freezTimer.Update(Time.deltaTime);

		//movementInput.x = 1;
		EventComponent.Update(Time.deltaTime);
		MovementComponent.CalculateVelocity();
		MovementComponent.AddGravityOnMovementVelocity();
		MovementComponent.CheckIfCharacterIsGrounded();
		CombatComponent.UpdateComponent(Time.deltaTime);
		MovementComponent.MoveCharacter();

		animController.Update(Time.deltaTime);
		if (Health != null) Health.Update(Time.deltaTime);
		if (StaggerComponent != null) StaggerComponent.Update(Time.deltaTime);


		
		// Debug
		if (IsPlayerCharacter)
		{
			foreach (var item in PluginStateMachine.DictionaryOfPluginStates)
			{
				if (item.Value.IsActive())
					Ultra.Utilities.Instance.DebugLogOnScreen("GameCharacter" + item.Value.GetStateType() + "PluginState is Active", 100, DebugAreas.Misc);
			}
		}
		if (MovementComponent.PossibleGround != null)
		{
			Ultra.Utilities.DrawWireSphere(MovementComponent.PossibleGround.hit.point, 0.2f, Color.blue, 0.0f, 100, DebugAreas.Movement);
		}
		if (IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("CurrentCharacterState: " + StateMachine.GetCurrentStateType().ToString(), 0f, StringColor.Teal, 100, DebugAreas.Movement);
		if (IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("Current Ground Angle: " + MovementComponent.GetPossibleGroundAngle(), 0f, StringColor.Teal, 200, DebugAreas.Misc);
		if (!IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("AICurrentCharacterState: " + StateMachine.GetCurrentStateType().ToString(), 0f, StringColor.Brown, 200, DebugAreas.AI);
	}

	

	private void LateUpdate()
	{
		if (!isInitialized) return;
		KillZOffset();
		if (animController != null) animController.LateUpdate();
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		
	}

	#region AnimEvents

	public void HitDetectionEventStart(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon?.HitDetectionStart();
	}

	public void HitDetectionEventEnd(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon?.HitDetectionEnd();
	}

	public void AttackRecoveryEvent(AnimationEvent evt)
	{
		StateMachine?.RequestStateChange(EGameCharacterState.AttackRecovery);
	}

	public void StartParticleEffect(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon?.StartParticelEffect(evt.intParameter);
	}

	public void StartCameraShake(AnimationEvent evt)
	{
		CameraController.Instance.ShakeCamerea(evt.intParameter);
	}

	public void DefensiveActionStart(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon.DefensiveActionStart();
	}

	public void DefensiveActionEnd(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon.DefensiveActionEnd();
	}

	#endregion

	public void DoDamage(GameCharacter damageInitiator, float damage)
	{
		if (damageInitiator != null)
		{
			if (CheckForSameTeam(damageInitiator.Team)) return;
		}
		Ultra.Utilities.Instance.DebugLogOnScreen(name + " got Damaged by: " + damageInitiator.name + ", Damage = " + damage, 2f, StringColor.Red, 200, DebugAreas.Combat);
		Health.AddCurrentValue(-damage);
		StaggerComponent.AddCurrentValue(-damage);
		damageInitiator?.combatComponent.AddCombo();
		CombatComponent.ComboBreak();

		if (CombatComponent.CanRequestFreez())
		{
			if (StateMachine.GetCurrentStateType() == EGameCharacterState.Freez)
			{
				freezTimer.Start(freezTime);
				AnimController.FreezA = !AnimController.FreezA;
			}else if (StateMachine.GetCurrentStateType() == EGameCharacterState.InAir)
			{
				StateMachine.RequestStateChange(EGameCharacterState.Freez);
			}
		}
		
		animController.TriggerAdditiveHit();
		OnDamaged(damageInitiator, damage);
		damageInitiator?.AddRatingOnHit(damage);
	}

	public bool CheckForSameTeam(HyppoliteTeam team)
	{
		return Team == team;
	}

	protected virtual void OnDamaged(GameCharacter damageInitiator, float damage)
	{

	}

	protected virtual void AddRatingOnHit(float damage)
	{

	}

	public bool CheckIfCharacterIsInAir()
	{
		return !MovementComponent.IsGrounded || MovementComponent.IsInJump;
	}

	public bool CheckIfCharacterIsOnSteepGround()
	{
		return MovementComponent.GetPossibleGroundAngle() > MovementComponent.SlopeLimit;
	}

	public bool CheckIfCharacterIsMoving()
	{
		return MovementComponent.Velocity.magnitude > 0 || GetHorizontalMovementInputDir().magnitude > 0;
	}

	public bool CheckIfCharacterIsStanding()
	{
		return MovementComponent.Velocity.magnitude <= 0 && GetHorizontalMovementInputDir().magnitude <= 0;
	}

	public void RequestBestCharacterState()
	{
		if (CheckIfCharacterIsInAir())
		{
			StateMachine.RequestStateChange(EGameCharacterState.InAir);
			return;
		}

		if (CheckIfCharacterIsOnSteepGround())
		{
			StateMachine.RequestStateChange(EGameCharacterState.InAir);
			return;
		}

		if (CheckIfCharacterIsMoving())
		{
			StateMachine.RequestStateChange(EGameCharacterState.Moving);
			return;
		}

		if (CheckIfCharacterIsStanding())
		{
			StateMachine.RequestStateChange(EGameCharacterState.Standing);
			return;
		}
	}

	public EGameCharacterState GetBestCharacterState()
	{
		if (CheckIfCharacterIsInAir())
		{
			return EGameCharacterState.InAir;
		}

		if (CheckIfCharacterIsOnSteepGround())
		{
			return EGameCharacterState.InAir;
		}

		if (CheckIfCharacterIsMoving())
		{
			return EGameCharacterState.Moving;
		}

		if (CheckIfCharacterIsStanding())
		{
			return EGameCharacterState.Standing;
		}

		return StateMachine.GetCurrentStateType();
	}

	void OnHealthValueChanged(float newHealthAmount, float oldHealthAmount)
	{
		if (newHealthAmount <= 0)
		{
			// Character Dead
			GameCharacterDied();
		}
	}

	public void AddFreezTime(float freezTime)
	{
		if (FreezTimer.CurrentTime + freezTime > FreezTime)
		{
			FreezTimer.Start(freezTime);
		}
		else
		{
			FreezTimer.AddTime(freezTime);
		}
	}

	public void CharacterMoveToPositionStateCharacterOnDestination(GameCharacter movedCharacter)
	{
		CombatComponent.CurrentWeapon.CharacterArrivedAtRequestedLocation(movedCharacter);
	}

	public void CharacterMoveToPositionStateAbort(GameCharacter movedCharacter)
	{
		CombatComponent.CurrentWeapon.CharacterMoveToAbort(movedCharacter);
	}

	public void CharacterMoveToPositionStateEnd(GameCharacter movedCharacter)
	{
		CombatComponent.CurrentWeapon.CharacterMoveToEnd(movedCharacter);
	}

	public void RotateToDir(Vector3 dir)
	{
		transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
	}

	void GameCharacterDied()
	{
		Ultra.Utilities.Instance.DebugLogOnScreen(name + " Is Dead", 2f, StringColor.White, 200, DebugAreas.Combat);
		IsGameCharacterDead = true;
		if (onGameCharacterDied != null) onGameCharacterDied(this);
		Animator.enabled = false;
		MovementComponent.CapsuleCollider.enabled = false;
		MovementComponent.UnityMovementController.enabled = false;

		foreach (Rigidbody rBody in rigDataComponent.RegdollRigidBodys)
		{
			rBody.useGravity = true;
			rBody.isKinematic = false;
		}
		foreach (Collider collider in rigDataComponent.Colliders)
		{
			collider.enabled = true;
		}
	}

	[Button("Die")]
	protected virtual void DieButton()
	{
		health.AddCurrentValue(-health.CurrentValue);
	}

	protected virtual void OnCharacterDetectionOverlapEnter(GameCharacter other)
	{
		
	}
}