using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class GameCharacter : MonoBehaviour
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

	private void Awake()
	{
		animator = gameObject.GetComponent<Animator>();
		if (animator == null) Debug.LogError("GameObject: " + name + " Does not have an Animator Attached!");
		rigidbody = gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null) Debug.LogError("GameObject: " + name + " Does not have an Rigibody Attached!");
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
	}

	private void Update()
	{
		//movementInput.x = 1;
		EventComponent.Update(Time.deltaTime);
		MovementComponent.CalculateVelocity();
		MovementComponent.AddGravityOnMovementVelocity();
		MovementComponent.CheckIfCharacterIsGrounded();
		CombatComponent.UpdateComponent(Time.deltaTime);
		MovementComponent.MoveCharacter();
		RotateCharacterInVelocityDirection();

		animController.Update(Time.deltaTime);

		if (MovementComponent.PossibleGround != null)
		{
			Ultra.Utilities.DrawWireSphere(MovementComponent.PossibleGround.hit.point, 0.2f, Color.blue, 0.0f, 100, DebugAreas.Movement);
		}
		if (IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("CurrentCharacterState: " + StateMachine.GetCurrentStateType().ToString(), 0f, StringColor.Teal, 100, DebugAreas.Movement);
		if (IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("Current Ground Angle: " + MovementComponent.GetPossibleGroundAngle(), 0f, StringColor.Teal, 200, DebugAreas.Misc);
	}

	private void RotateCharacterInVelocityDirection()
	{
		if (MovementComponent.MovementVelocity.normalized.x != 0) lastDir = new Vector3(MovementComponent.MovementVelocity.x, 0, 0);
		if (lastDir == Vector3.zero) return;
		Quaternion targetRot = Quaternion.LookRotation(lastDir.normalized, Vector3.up);
		targetRot = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * gameCharacterData.RoationSpeed);
		Vector3 dir = transform.rotation * Vector3.forward;
		Vector3 cross = Vector3.Cross(lastDir.normalized, dir);
		float sign = Mathf.Sign(cross.y);
		if (Ultra.Utilities.IsNearlyEqual(cross, Vector3.zero, 0.2f))
		{
			sign = 0f;
		}
		animController.RotationTrarget = sign;
		//// Need to rotate Character
		//targetRot.eulerAngles = new Vector3(targetRot.eulerAngles.x, targetRot.eulerAngles.y + 90, targetRot.eulerAngles.z);
		transform.rotation = targetRot;
	}
	

	private void LateUpdate()
	{
		KillZOffset();
		animController.LateUpdate();
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
}