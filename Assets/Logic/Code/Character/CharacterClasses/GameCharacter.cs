using EasyButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using Ultra;
using UnityEngine;
using UnityEngine.Profiling;

public class GameCharacter : MonoBehaviour, IDamage
{
	public delegate void GameCharacterEventWithGameCharacter(GameCharacter gameCharacter);
	public delegate void OnGameCharacterEvent();
	public GameCharacterEventWithGameCharacter onGameCharacterDied;
	public GameCharacterEventWithGameCharacter onGameCharacterDestroyed;
	public OnGameCharacterEvent onGameCharacterAggroChanged;
	public GameCharacterEventWithGameCharacter onGameCharacterGotArroged;
	public GameCharacterEventWithGameCharacter onGameCharacterStoppedBeingArroged;

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
	BuffComponent buffComponent;
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
	GameCharacterDetection characterDetection;
	bool isGameCharacterDead = false;
	HyppoliteTeam team;
	Quaternion rotationTarget;
	Vector3 lastDir;
	int ignoreCharacterLayer;
	int characterLayer;
	int defaultLayer;
	ParticleSystemPool dodgeParticleSystemPool;
	ParticleSystemPool succsessfullDodgeParticlePool;
	GameObject dataWorldHolder;
	List<GameCharacter> aggroedCharacters = new List<GameCharacter>();

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
	public bool IsPlayerCharacter { get { return isPlayerCharacter; } set { isPlayerCharacter = value; } }
	public RecourceBase Health { get { return health; } }
	public StaggerComponent StaggerComponent { get { return staggerComponent; } }
	public Ultra.Timer FreezTimer { get { return freezTimer; } }
	public float FreezTime { get { return freezTime; } }
	public float FreezTimeOverride { get { return freezTimeOverride; } set { freezTimeOverride = value; } }
	public GameCharacterDetection CharacterDetection { get { return characterDetection; } }
	public RigDataComponent RigDataComponent { get { return rigDataComponent; } }
	public Quaternion RotationTarget { get { return rotationTarget; } set { rotationTarget = value; } }
	public Vector3 LastDir { get { return lastDir; } set { lastDir = value; } }
	public int IgnoreCharacterLayer { get { return ignoreCharacterLayer; } }
	public int CharacterLayer { get { return characterLayer; } }
	public int DefaultLayer { get { return defaultLayer; } }
	public ParticleSystemPool SuccsessfullDodgeParticlePool { get { return succsessfullDodgeParticlePool; } }
	public ParticleSystemPool DodgeParticleSystemPool { get { return dodgeParticleSystemPool; } }
	public GameObject DataWorldHolder { get { return dataWorldHolder; } }
	public BuffComponent BuffComponent { get { return buffComponent; } }
	public bool CharacterHasAggro => aggroedCharacters.Count > 0;

#if UNITY_EDITOR
	//[DebugGUIGraph(min: 0, max: 9, r: 0, g: 1, b: 1, autoScale: true)]
	//public float MovementSpeed { get { return MovementComponent != null ? MovementComponent.MovementSpeed : 0; } }
	//
	//[DebugGUIPrint, DebugGUIGraph(min: -10, max: 10, group: 1, r: 1, g: 0.3f, b: 0.3f)]
	//public float MovementVelx { get { return MovementComponent != null ? MovementComponent.Velocity.x : 0; } }
	//[DebugGUIPrint, DebugGUIGraph(min: -10, max: 10, group: 1, r: 0, g: 1, b: 0)]
	//public float MovementVely { get { return MovementComponent != null ? MovementComponent.Velocity.y : 0; } }
	//[DebugGUIPrint, DebugGUIGraph(min: -10, max: 10, group: 1, r: 0, g: 1, b: 1)]
	//public float MovementVelz { get { return MovementComponent != null ? MovementComponent.Velocity.z : 0; } }
#endif

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

		ignoreCharacterLayer = LayerMask.GetMask("IgnoreCharacter");
		characterLayer = LayerMask.GetMask("Character");
		defaultLayer =  LayerMask.GetMask("Default");
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
		buffComponent = new BuffComponent(this);

		AnimController.SetChestCorrectionWeight(0);
		AnimController.SetFPFootIKLWeight(0);
		AnimController.SetFPFootIKRWeight(0);

		GameObject characterDetectionObject = GameObject.Instantiate(GameAssets.Instance.characterDetection, transform);
		characterDetection = characterDetectionObject.GetComponent<GameCharacterDetection>();
		characterDetection.onOverlapEnter += OnCharacterDetectionOverlapEnter;
		SphereCollider sphereCollider = characterDetection.Collider as SphereCollider;
		if (sphereCollider != null) sphereCollider.radius = gameCharacterData.CharacterDetectionRange;

		if (animController != null) animController.Start();
		if (combatComponent != null) combatComponent.StartComponent();

		health = new RecourceBase(gameCharacterData.Health, gameCharacterData.Health);
		health.onCurrentValueChange += OnHealthValueChanged;

		staggerComponent = new StaggerComponent(this, gameCharacterData.StaggerTime, gameCharacterData.MaxStaggerValue, gameCharacterData.MaxStaggerValue);

		if (dataWorldHolder == null)
		{
			dataWorldHolder = new GameObject(">> " + this.name + " World Data Holder");
		}

		SetupPartilcePools();

		PluginStateMachine.AddPluginState(EPluginCharacterState.LookInVelocityDirection);

		isInitialized = true;
	}

	private void SetupPartilcePools()
	{
		succsessfullDodgeParticlePool = new ParticleSystemPool(gameCharacterData.SuccsessfullDodgeParticleEffect, CreateHolderChild("SuccsessfullDodgeEffect Holder"), 2);
		dodgeParticleSystemPool = new ParticleSystemPool(gameCharacterData.DodgeParticleEffect, CreateHolderChild("DodgeEffect Holder"), 2);
	}

	public GameObject CreateHolderChild(string name)
	{
		var succsessfullDodgeParticlePoolHolder = new GameObject(">> " + this.name + " " + name);
		succsessfullDodgeParticlePoolHolder.transform.parent = dataWorldHolder.transform;
		return succsessfullDodgeParticlePoolHolder;
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
		if (!IsInitialized || !LoadingChecker.Instance.FinishLoading) return;
		if (!isPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("Enemy MovementInput: " + movementInput, 0f, StringColor.Green, 200, DebugAreas.AI);
		float deltaTime = Time.deltaTime;
		freezTimer.Update(deltaTime);
		//movementInput.x = 1;
		EventComponent.Update(deltaTime);
		MovementComponent.CalculateVelocity();
		MovementComponent.AddGravityOnMovementVelocity();
		MovementComponent.CheckIfCharacterIsGrounded();
		CombatComponent?.UpdateComponent(deltaTime);
		MovementComponent.MoveCharacter();
		buffComponent.Update(deltaTime);

		animController.Update(deltaTime);
		if (Health != null) Health.Update(deltaTime);
		if (StaggerComponent != null) StaggerComponent.Update(deltaTime);


#if UNITY_EDITOR
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
#endif
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
		CameraController.Instance?.ShakeCamerea(evt.intParameter);
	}

	public void DefensiveActionStart(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon.DefensiveActionStart();
	}

	public void DefensiveActionEnd(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon.DefensiveActionEnd();
	}

	public void StartIFrameWindow(AnimationEvent evt)
	{
		PluginStateMachine?.AddPluginState(EPluginCharacterState.IFrame);
	}

	public void EndIFrameWindow(AnimationEvent evt)
	{
		PluginStateMachine?.RemovePluginState(EPluginCharacterState.IFrame);
	}

	public void AllowEarlyLeaveAttackRecovery(AnimationEvent evt)
	{
		CombatComponent.AllowEarlyLeaveAttackRecovery = true;
	}

	public void PlayAttackSound(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon?.PlayAttackSound(evt.intParameter);
	}

	public void SwitchAttackAnimEvent(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon?.CurrentAction?.Action?.SwitchAnimationEvent();
	}

	#endregion

	public void DoDamage(GameCharacter damageInitiator, float damage, bool shouldStagger = true, bool removeCharge = true)
	{
		if (damageInitiator != null)
		{
			if (CheckForSameTeam(damageInitiator.Team)) return;
		}
		if (PluginStateMachine.ContainsPluginState(EPluginCharacterState.IFrame))
		{

			SuccsessfullDodge(damageInitiator, damage);
			damage = 0;
			return;
		}
		else if (PluginStateMachine.ContainsPluginState(EPluginCharacterState.Parry))
		{
			SuccessfullParry(damageInitiator, damage);
			damage = 0;
			return;
		}
		else if (PluginStateMachine.ContainsPluginState(EPluginCharacterState.Block))
		{
			if (damage > 0)
			{
				SuccessfullBlock(damageInitiator, damage);
				damage = damage / 2f;
			}
		}

		if (HypoUttilies.gameModeBase == null || HypoUttilies.gameModeBase.AllowDamage()) 
			Health.AddCurrentValue(-damage);

		StaggerComponent.AddCurrentValue(-damage);
		Ultra.Utilities.Instance.DebugLogOnScreen(name + " got Damaged by: " + damageInitiator.name + ", Damage = " + damage, 2f, StringColor.Red, 200, DebugAreas.Combat);

		if (damage > 0)
		{
			damageInitiator?.CombatComponent.AddCombo();
			CombatComponent.ComboBreak();
			damageInitiator?.CombatComponent.CharacterDidDamageTo(damageInitiator, damage);

			if (!isPlayerCharacter && shouldStagger) CombatComponent.RequestFreez();
			//if (CombatComponent.CanRequestFreez())
			//{
			//	if (StateMachine.GetCurrentStateType() == EGameCharacterState.Freez)
			//	{
			//		freezTimer.Start(freezTime);
			//		AnimController.FreezA = !AnimController.FreezA;
			//	}
			//	else if (StateMachine.GetCurrentStateType() == EGameCharacterState.InAir)
			//	{
			//		StateMachine.RequestStateChange(EGameCharacterState.Freez);
			//	}
			//}

			animController.TriggerAdditiveHit();
			OnDamaged(damageInitiator, damage, removeCharge);
			damageInitiator?.AddRatingOnHit(damage);
		}
	}

	public bool CheckForSameTeam(HyppoliteTeam team)
	{
		return Team == team;
	}

	protected virtual void OnDamaged(GameCharacter damageInitiator, float damage, bool removeCharge)
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

	public void RequestBestCharacterState(bool force = false)
	{
		StateMachine.RequestStateChange(GetBestCharacterState(), force);
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
		Animator.enabled = false;
		MovementComponent.CapsuleCollider.enabled = false;
		MovementComponent.UnityMovementController.enabled = false;
		// Request a dead state so unsubsribing from specific state delegates is easier by just calling CurrentState.End(NewState Dead)
		StateMachine.RequestStateChange(EGameCharacterState.Dead, true);

		GameObject.Destroy(gameObject, 3f);

		foreach (Rigidbody rBody in rigDataComponent.RegdollRigidBodys)
		{
			rBody.useGravity = true;
			rBody.isKinematic = false;
		}
		foreach (Collider collider in rigDataComponent.Colliders)
		{
			collider.enabled = true;
		}

		GameObject.Destroy(dataWorldHolder);

		if (onGameCharacterDied != null) onGameCharacterDied(this);
	}

	[Button("Die")]
	protected virtual void DieButton()
	{
		health.AddCurrentValue(-health.CurrentValue);
	}

	protected virtual void OnCharacterDetectionOverlapEnter(GameCharacter other)
	{

	}

	public virtual void ShowAttackFeedback()
	{

	}

	public void AddCharacterToAggroedCharacters(GameCharacter aggroedCharacter)
	{
		aggroedCharacters.Add(aggroedCharacter);
		aggroedCharacter.onGameCharacterDied += OnAggroedCharactersDies;
		aggroedCharacter.onGameCharacterDestroyed += OnAggroedCharactersDies;

		if (onGameCharacterAggroChanged != null) onGameCharacterAggroChanged();
		if (onGameCharacterGotArroged != null) onGameCharacterGotArroged(aggroedCharacter);
	}

	void OnAggroedCharactersDies(GameCharacter diedCharacter)
	{
		RemoveCharacterFromAggroedCharacter(diedCharacter);
	}

	public void RemoveCharacterFromAggroedCharacter(GameCharacter aggroedCharacter)
	{
		if (aggroedCharacters.Contains(aggroedCharacter))
		{
			aggroedCharacter.onGameCharacterDied -= OnAggroedCharactersDies;
			aggroedCharacter.onGameCharacterDestroyed -= OnAggroedCharactersDies;
			aggroedCharacters.Remove(aggroedCharacter);
		}else
		{
			Ultra.Utilities.Instance.DebugErrorString("GameCharacter", "RemoveCharacterFromAggroedCharacter", "Character Died but was not in Aggroed Characters, something went wrong!");
		}

		RemoveNullsFromAggroedCharacters();

		if (onGameCharacterAggroChanged != null) onGameCharacterAggroChanged();
		if (onGameCharacterStoppedBeingArroged != null) onGameCharacterStoppedBeingArroged(aggroedCharacter);
	}

	void RemoveNullsFromAggroedCharacters()
	{
		List<GameCharacter> nullCharacters = new List<GameCharacter>();
		foreach (var character in aggroedCharacters)
		{
			if (character == null) nullCharacters.Add(character);
		}
		foreach (var character in nullCharacters)
		{
			aggroedCharacters.Remove(character);
		}
	}

	protected virtual void SuccsessfullDodge(GameCharacter damageInitiator, float damage)
	{
		CombatComponent.SuccsessfullDodge(damageInitiator, damage);
	}

	protected virtual void SuccessfullParry(GameCharacter damageInitiator, float damage)
	{
		CombatComponent.CurrentWeapon.CurrentAction.Action.SuccessfullParry(damageInitiator, damage);
	}

	protected virtual void SuccessfullBlock(GameCharacter damageInitiator, float damage)
	{
		CombatComponent.CurrentWeapon.CurrentAction.Action.SuccessfullBlock(damageInitiator, damage);
	}

	[Button("ReInitGraphs")]
	protected void Test()
	{
		DebugGUI.ForceReinitializeAttributes();
	}

	public HyppoliteTeam GetTeam()
	{
		return Team;
	}
}