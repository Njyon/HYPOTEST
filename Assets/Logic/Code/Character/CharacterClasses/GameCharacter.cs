using EasyButtons;
using MyBox;
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
	public GameCharacterEventWithGameCharacter onGameCharacterRespawnes;
	public OnGameCharacterEvent onGameCharacterAggroChanged;
	public GameCharacterEventWithGameCharacter onGameCharacterGotArroged;
	public GameCharacterEventWithGameCharacter onGameCharacterStoppedBeingArroged;

	[Header("Animation Debug")]
	public ToggableDebugFloat legLayerDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat headLayerDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat spineLayerDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat armRLayerDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat armLLayerDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat secondaryLayerDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat upperBodyLayerDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat addativeLayerDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat chestCorrectionDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat footIKRCorrectionDebugFloat = new ToggableDebugFloat();
	public ToggableDebugFloat footIKLCorrectionDebugFloat = new ToggableDebugFloat();

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
	DamagableDetection characterDetection;
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
	List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
	int materialHitColorIndex;
	int materialHitIntensityIndex;
	GameObject respawnObj;
	int lastFootStepSoundEffect;

	bool isInitialized = false;
	public bool IsInitialized { get { return isInitialized; } }

	public GameCharacterStateMachine StateMachine { get { return stateMachine; } }
	public Vector2 MovementInput { get { return movementInput; } }
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
	public DamagableDetection CharacterDetection { get { return characterDetection; } }
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
	public List<SkinnedMeshRenderer> SkinnedMeshRenderers { get { return skinnedMeshRenderers; } }
	public GameObject RespawnObj { get { return respawnObj; } }

	public GameCharacterData GameCharacterData 
	{ 
		get 
		{
			if (gameCharacterData == null)
				gameCharacterData = gameObject.GetComponent<GameCharacterData>();

			if (gameCharacterData == null)
				Ultra.Utilities.Instance.DebugErrorString("GameCharacter", "GameCharacterData", "GameCharacterData is not attached to GameCharacter!");

			return gameCharacterData; 
		} 
	}
	public int MaterialHitColorIndex { 
		get 
		{ 
			if (materialHitColorIndex == 0)
			{
				materialHitColorIndex = Shader.PropertyToID("_HitColor");
			}
			return materialHitColorIndex;
		} 
	}
	public int MaterialHitIntensityIndex {
		get 
		{
			if (materialHitIntensityIndex == 0)
			{
				materialHitIntensityIndex = Shader.PropertyToID("_HitIntensity");
			}
			return materialHitIntensityIndex;
		} 
	}

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
		protected set { isGameCharacterDead = value; }
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
		stateMachine = gameObject.AddComponent<GameCharacterStateMachine>();
		pluginStateMachine = gameObject.AddComponent<GameCharacterPluginStateMachine>();
		pluginStateMachine.Init(this);
		animController = new AnimationController(this);
		combatComponent = new CombatComponent(this);
		buffComponent = new BuffComponent(this);

		AnimController.ResetAnimIK_HARD();

		GameObject characterDetectionObject = GameObject.Instantiate(GameAssets.Instance.characterDetection, transform);
		characterDetection = characterDetectionObject.GetComponent<DamagableDetection>();
		characterDetection.onOverlapEnter += OnTargetDetectionOverlapEnter;
		SphereCollider sphereCollider = characterDetection.Collider as SphereCollider;
		if (sphereCollider != null) sphereCollider.radius = GameCharacterData.CharacterDetectionRange;

		if (animController != null) animController.Start();
		if (combatComponent != null) combatComponent.StartComponent();

		health = new RecourceBase(GameCharacterData.Health, GameCharacterData.Health);
		health.onCurrentValueChange += OnHealthValueChanged;

		staggerComponent = new StaggerComponent(this, GameCharacterData.StaggerTime, GameCharacterData.MaxStaggerValue, GameCharacterData.MaxStaggerValue);

		if (dataWorldHolder == null)
		{
			dataWorldHolder = new GameObject(">> " + this.name + " World Data Holder");
		}

		SetupPartilcePools();

		ApplyDefaultPluginStates();

		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent<SkinnedMeshRenderer>();

			if (skinnedMeshRenderer != null)
				skinnedMeshRenderers.Add(skinnedMeshRenderer);
		}

		if (IsPlayerCharacter)
		{
			SetNewRespawnPoint(new GameObject("PlayerSpawn"));
			respawnObj.transform.position = transform.position;
		}

		isInitialized = true;
	}

	protected void ApplyDefaultPluginStates()
	{
		PluginStateMachine.AddPluginState(EPluginCharacterState.LookInVelocityDirection);
		PluginStateMachine.AddPluginState(EPluginCharacterState.IgnoreGravityRuleState);
	}

	void SetupPartilcePools()
	{
		succsessfullDodgeParticlePool = new ParticleSystemPool(GameCharacterData.SuccsessfullDodgeParticleEffect, CreateHolderChild("SuccsessfullDodgeEffect Holder"), 2);
		dodgeParticleSystemPool = new ParticleSystemPool(GameCharacterData.DodgeParticleEffect, CreateHolderChild("DodgeEffect Holder"), 2);
	}

	/// <summary>
	/// Create Holder Object for Pools or other GameObject Collection
	/// </summary>
	/// <param name="name"> Name of the Holder </param>
	/// <param name="parent"> Parent of the Holder, if NULL default GameCharacter DataWolrdHolder is used </param>
	/// <returns></returns>
	public GameObject CreateHolderChild(string name, GameObject parent = null)
	{
		var succsessfullDodgeParticlePoolHolder = new GameObject(">> " + this.name + " " + name);
		succsessfullDodgeParticlePoolHolder.transform.parent = parent != null ? parent.transform : dataWorldHolder.transform;
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
			characterDetection.onOverlapEnter -= OnTargetDetectionOverlapEnter;
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
		if (MovementComponent.enabled)
		{
			MovementComponent.CalculateVelocity();
			MovementComponent.AddGravityOnMovementVelocity();
			MovementComponent.CheckIfCharacterIsGrounded();
		}
		
		CombatComponent?.UpdateComponent(deltaTime);
		if (MovementComponent.enabled) MovementComponent.MoveCharacter();
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
		buffComponent.LateUpdate(Time.deltaTime);
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
		CombatComponent?.CurrentWeapon?.PlayDefaultAttackSound(evt.intParameter);
	}

	public void PlaySpecialAttackSound(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon?.PlaySpecialAttackSound(evt.intParameter);
	}

	public void SwitchAttackAnimEvent(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon?.CurrentAction?.Action?.SwitchAnimationEvent();
	}

	public void TriggerAttackAnimEvent(AnimationEvent evt)
	{
		CombatComponent?.CurrentWeapon?.CurrentAction?.Action?.TriggerAnimationEvent();
	}

	public void FootStepTriggerEvent(AnimationEvent evt)
	{
		SoundEffectWrapper soundEffectWrapper;
		if (evt.animatorClipInfo.weight >= 0.5f)
		{
			switch (MovementComponent.CapsulCastGroundHit?.hit.collider?.material?.name)
			{
				case "PM_Metal (Instance)":
					if (!GameCharacterData.FootSoundEffects.ContainsKey(EPhysicsMaterial.Metal)) return;
					if (GameCharacterData.FootSoundEffects.TryGetValue(EPhysicsMaterial.Metal, out soundEffectWrapper) && soundEffectWrapper != null)
						PlayFootStepSound(-1, soundEffectWrapper.SoundEffects);
					break;
				default:
					if (!GameCharacterData.FootSoundEffects.ContainsKey(EPhysicsMaterial.Stone)) return;
					if (GameCharacterData.FootSoundEffects.TryGetValue(EPhysicsMaterial.Stone, out soundEffectWrapper) && soundEffectWrapper != null)
						PlayFootStepSound(-1, soundEffectWrapper.SoundEffects);
					break;
			}
		}
	}

	#endregion

	public virtual void PlayFootStepSound(int index, List<SoundEffect> sounds)
	{
		if (sounds.Count <= 0) return;

		index--;
		if (index < 0)
		{
			int randIndex = UnityEngine.Random.Range(0, sounds.Count);
			if (randIndex == lastFootStepSoundEffect)
			{
				randIndex++;
				randIndex %= sounds.Count;
			}
			var soundClip = sounds[randIndex];
			SoundManager.Instance.PlaySound(soundClip);
			lastFootStepSoundEffect = randIndex;
		}
		else
		{
			var soundClip = sounds[index];
			SoundManager.Instance.PlaySound(soundClip);
			lastFootStepSoundEffect = index;
		}
	}

	public bool CanBeDamaged()
	{
		return !PluginStateMachine.ContainsPluginState(EPluginCharacterState.IFrame);
	}

	public void DoDamage(GameCharacter damageInitiator, float damage, bool shouldStagger = true, bool removeCharge = true, bool shouldFreezGame = true)
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

		if (HypoUttilies.GameMode == null || HypoUttilies.GameMode.AllowDamage()) 
			Health.AddCurrentValue(-damage);

		StaggerComponent.AddCurrentValue(-damage);
		string damageInitiatorName = damageInitiator != null ? damageInitiator.name : "Enviroment";
		Ultra.Utilities.Instance.DebugLogOnScreen(name + " got Damaged by: " + damageInitiatorName + ", Damage = " + damage, 2f, StringColor.Red, 200, DebugAreas.Combat);

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
			AnimController.SwitchFreezState();

			animController.TriggerAdditiveHit();
			OnDamaged(damageInitiator, damage, removeCharge);
			damageInitiator?.AddRatingOnHit(damage);

			DefaultGameModeData data = Ultra.HypoUttilies.GameMode.GetDefaultGameModeData();
			BuffComponent.AddBuff(new OnHitEffectBuff(this, data.hitWiggleTime, data.hitWiggleHalfLenghtRange, data.hitWiggleFrequency));
			BuffComponent.AddBuff(new OnHitShaderEffect(this, data.shaderEffectTime));

			MovementComponent.IgnoreGravityForTime(data.ignoreGravityAfterHit);

			if (shouldFreezGame) GameTimeManager.Instance.AddDefaultFreezFrame();
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

	protected virtual void GameCharacterDied()
	{
		Ultra.Utilities.Instance.DebugLogOnScreen(name + " Is Dead", 2f, StringColor.White, 200, DebugAreas.Combat);
		IsGameCharacterDead = true;
		Animator.enabled = false;
		MovementComponent.CapsuleCollider.enabled = false;
		MovementComponent.UnityMovementController.enabled = false;
		// Request a dead state so unsubsribing from specific state delegates is easier by just calling CurrentState.End(NewState Dead)
		StateMachine.RequestStateChange(EGameCharacterState.Dead, true);

		if (!IsPlayerCharacter)
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

		if (!IsPlayerCharacter)
			GameObject.Destroy(dataWorldHolder);

		if (onGameCharacterDied != null) onGameCharacterDied(this);
	}

	[ButtonMethod()]
	protected virtual void DieButton()
	{
		health.AddCurrentValue(-health.CurrentValue);
	}

	protected virtual void OnTargetDetectionOverlapEnter(IDamage other)
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

	public void SetNewRespawnPoint(GameObject respawnPointObj)
	{
		respawnObj = respawnPointObj;
	}

	public virtual void RespawnCharacter()
	{
	
	}

	public bool IsGameCharacter()
	{
		return true;
	}

	public GameCharacter GetGameCharacter()
	{
		return this;
	}

	public Vector3 GetPosition()
	{
		return MovementComponent.CharacterCenter;
	}
}