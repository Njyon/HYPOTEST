using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class GameCharacter : MonoBehaviour , IDamage
{
	GameCharacterStateMachine stateMachine;
	GameCharacterPluginStateMachine pluginStateMachine;
	GameCharacterData gameCharacterData;
	EventComponent eventComponent;
	Animator animator;
	AnimationController animController;
	ScriptableCharacter characterData;
	CombatComponent combatComponent;
	GameCharacterMovementComponent movementComponent;
	Rigidbody rigidbody;
	Vector2 movementInput;
	int currentJumpAmount = 0;
	Vector3 lastDir;
	bool isPlayerCharacter = false;
	RecourceBase health;
	Quaternion rotationTarget;
	Ultra.Timer freezTimer;
	float freezTime = 1f;

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
	public Quaternion RotationTarget { get { return rotationTarget; } }
	public Ultra.Timer FreezTimer { get { return freezTimer; } }
	public float FreezTime { get { return freezTime; } }	

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

	void Awake()
	{
		animator = gameObject.GetComponent<Animator>();
		if (animator == null) Debug.LogError("GameObject: " + name + " Does not have an Animator Attached!");
		rigidbody = gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null) Debug.LogError("GameObject: " + name + " Does not have an Rigibody Attached!");

		freezTimer = new Ultra.Timer(freezTime, true);
	}

	public void CustomAwake()
	{
	
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

		// Set Default Data
		lastDir = transform.right;

		animController.Start();
		combatComponent.StartComponent();

		health = new RecourceBase(gameCharacterData.Health, gameCharacterData.Health);
		health.onCurrentValueChange += OnHealthValueChanged;

		isInitialized = true;
	}

	void OnDestroy()
	{
		if (health != null)
		{
			health.onCurrentValueChange -= OnHealthValueChanged;
		}
	}

	private void Update()
	{
		if (!isInitialized) return;
		freezTimer.Update(Time.deltaTime);

		//movementInput.x = 1;
		EventComponent.Update(Time.deltaTime);
		MovementComponent.CalculateVelocity();
		MovementComponent.AddGravityOnMovementVelocity();
		MovementComponent.CheckIfCharacterIsGrounded();
		CombatComponent.UpdateComponent(Time.deltaTime);
		MovementComponent.MoveCharacter();
		RotateCharacterInVelocityDirection();

		animController.Update(Time.deltaTime);
		if (Health != null) Health.Update(Time.deltaTime);

		if (MovementComponent.PossibleGround != null)
		{
			Ultra.Utilities.DrawWireSphere(MovementComponent.PossibleGround.hit.point, 0.2f, Color.blue, 0.0f, 100, DebugAreas.Movement);
		}
		if (IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("CurrentCharacterState: " + StateMachine.GetCurrentStateType().ToString(), 0f, StringColor.Teal, 100, DebugAreas.Movement);
		if (IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("Current Ground Angle: " + MovementComponent.GetPossibleGroundAngle(), 0f, StringColor.Teal, 200, DebugAreas.Misc);
	}

	private void RotateCharacterInVelocityDirection()
	{
		if (StateMachine.GetCurrentStateType() == EGameCharacterState.Attack || StateMachine.GetCurrentStateType() == EGameCharacterState.AttackRecovery) return;

		if (MovementComponent.MovementVelocity.normalized.x != 0) lastDir = new Vector3(MovementComponent.MovementVelocity.x, 0, 0);
		if (lastDir == Vector3.zero) return;
		rotationTarget = Quaternion.LookRotation(lastDir.normalized, Vector3.up);
		Quaternion targetRot = Quaternion.Slerp(transform.rotation, rotationTarget, Time.deltaTime * gameCharacterData.RoationSpeed);
		Vector3 dir = transform.rotation * Vector3.forward;
		Vector3 cross = Vector3.Cross(lastDir.normalized, dir);
		float sign = Mathf.Sign(cross.y);
		if (Ultra.Utilities.IsNearlyEqual(cross, Vector3.zero, 0.2f))
		{
			sign = 0f;
		}
		// invert sign because of lerping rotation
		animController.RotationTrarget = sign *-1;
		transform.rotation = targetRot;
		//Ultra.Utilities.DrawArrow(transform.position, targetRot * Vector3.forward, 10, Color.green);
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

	public void HitDetectionEventStart()
	{
		CombatComponent?.CurrentWeapon?.HitDetectionStart();
	}

	public void HitDetectionEventEnd()
	{
		CombatComponent?.CurrentWeapon?.HitDetectionEnd();
	}

	public void AttackRecoveryEvent()
	{
		StateMachine?.RequestStateChange(EGameCharacterState.AttackRecovery);
	}

	public void DoDamage(GameCharacter damageInitiator, float damage)
	{
		Ultra.Utilities.Instance.DebugLogOnScreen(name + " got Damaged by: " + damageInitiator.name + ", Damage = " + damage, 2f, StringColor.Red, 200, DebugAreas.Combat);
		Health.AddCurrentValue(-damage);
		if (StateMachine.GetCurrentStateType() == EGameCharacterState.Freez || StateMachine.GetCurrentStateType() == EGameCharacterState.InAir)
		{
			freezTimer.Start(freezTime);
			AnimController.FreezA = !AnimController.FreezA;
		}
		animController.TriggerAdditiveHit();
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
		return MovementComponent.Veloctiy.magnitude > 0 || GetHorizontalMovementInputDir().magnitude > 0;
	}

	public bool CheckIfCharacterIsStanding()
	{
		return MovementComponent.Veloctiy.magnitude <= 0 && GetHorizontalMovementInputDir().magnitude <= 0;
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

	void OnHealthValueChanged(float newHealthAmount, float oldHealthAmount)
	{
		if (newHealthAmount <= 0)
		{
			// Character Dead
			Ultra.Utilities.Instance.DebugLogOnScreen(name + " Is Dead", 2f, StringColor.White, 200, DebugAreas.Combat);
		}
	}
}