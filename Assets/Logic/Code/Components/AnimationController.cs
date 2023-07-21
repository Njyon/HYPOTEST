using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AnimationController
{
	GameCharacter gameCharacter;
	AnimatorOverrideController overrideController;

	int walkRunBlendID;
	int strideBlendID;
	int standingPlayRateID;
	int isGroundedID;
	int isMovingID;
	int rotationBlendID;
	int fallingBlendID;
	int jumpATriggerID;
	int jumpBTriggerID;
	int jumpStateAID;
	int layerAID;
	int spineLayerIndex;
	int legLayerIndex;
	int headLayerIndex;
	int armRLayerIndex;
	int armLLayerIndex;
	int inAttackIndex;
	int attackAIndex;
	int attackAStateIndex;
	int secondaryMotionLayerIndex;
	int rotationLayerIndex;
	int upMovementCurveIndex;
	int hitTriggerIndex;
	int hitLayerIndex;
	int inFreezIndex;
	int freezAIndex;

	float minMalkSpeed;
	bool startWalkRunBlendInterp = true;
	float spineLayerInterpTarget;
	float spineLayerInterpSpeed;
	float legLayerInterpTarget;
	float legLayerInterpSpeed;
	float headLayerInterpTarget;
	float headLayerInterpSpeed;
	float armRLayerInterpTarget;
	float armRLayerInterpSpeed;
	float armLLayerInterpTarget;
	float armLLayerInterpSpeed;
	float secondaryMotionLayerInterpTarget;
	float secondaryMotionLayerInterpSpeed;

	float walkRunBlend;
	public float WalkRunBlend { 
		get { return walkRunBlend; } 
		private set {
			if (walkRunBlend != value)
			{
				walkRunBlend = value;
				gameCharacter.Animator.SetFloat(walkRunBlendID, walkRunBlend);
			}
		}
	}
	float strideBlend;
	public float StrideBlend { 
		get { return strideBlend; } 
		private set {
			if (strideBlend != value)
			{
				strideBlend = value;
				gameCharacter.Animator.SetFloat(strideBlendID, strideBlend);
			}
		}
	}
	float standingPlayRate;
	public float StandingPlayRate {
		get { return standingPlayRate; }
		private set {
			if (standingPlayRate != value)
			{
				standingPlayRate = value;
				gameCharacter.Animator.SetFloat(standingPlayRateID, standingPlayRate);
			}
		}
	}
	bool isGrounded;
	public bool IsGrounded { 
		get { return isGrounded; } 
		private set {
			if (isGrounded != value)
			{
				isGrounded = value;
				gameCharacter.Animator.SetBool(isGroundedID, isGrounded);
			}
		}
	}
	bool isMoving;
	public bool IsMoving { 
		get { return isMoving; } 
		private set {
			if (isMoving != value)
			{
				isMoving = value;
				gameCharacter.Animator.SetBool(isMovingID, isMoving);
			}
		}
	}
	float rotationTarget;
	public float RotationTrarget { get { return rotationTarget; } set { rotationTarget = value; } }
	float rotationBlend;
	public float RotationBlend { 
		get { return rotationBlend; }
		private set {
			if (rotationBlend != value)
			{
				if (Ultra.Utilities.IsNearlyEqual(value, 0, 0.001f)) value = 0;
				rotationBlend = value;
				gameCharacter.Animator.SetFloat(rotationBlendID, rotationBlend);
			}
		}
	}
	float fallingBlend;
	public float FallingBlend { 
		get { return fallingBlend; } 
		private set { 
			if (fallingBlend != value)
			{
				fallingBlend = value;
				gameCharacter.Animator.SetFloat(fallingBlendID, fallingBlend);
			}
		}
	}
	bool layerA = true;
	public bool UpperBodyIsA { 
		get { return layerA; } 
		private set
		{
			if (layerA != value)
			{
				layerA = value;
				gameCharacter.Animator.SetBool(layerAID, layerA);
			}
		}
	}
	public float SpineLayerWeight {
		get { return gameCharacter.Animator.GetLayerWeight(spineLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(spineLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(spineLayerIndex, value);
			}
		}
	}
	public float LegLayerWeight {
		get { return gameCharacter.Animator.GetLayerWeight(legLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(legLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(legLayerIndex, value);
			}
		}
	}
	public float HeadLayerWeight
	{
		get { return gameCharacter.Animator.GetLayerWeight(headLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(headLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(headLayerIndex, value);
			}
		}
	}
	public float ArmRLayerWeight
	{
		get { return gameCharacter.Animator.GetLayerWeight(armRLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(armRLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(armRLayerIndex, value);
			}
		}
	}
	public float ArmLLayerWeight
	{
		get { return gameCharacter.Animator.GetLayerWeight(armLLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(armLLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(armLLayerIndex, value);
			}
		}
	}
	public float SecondaryMotionLayerWeight
	{
		get { return gameCharacter.Animator.GetLayerWeight(secondaryMotionLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(secondaryMotionLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(secondaryMotionLayerIndex, value);
			}
		}
	}
	public float HitLayerWeight
	{
		get { return gameCharacter.Animator.GetLayerWeight(hitLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(hitLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(hitLayerIndex, value);
			}
		}
	}
	bool inAttack = false;
	public bool InAttack { 
		get { return inAttack; } 
		set {
			if (inAttack != value)
			{
				inAttack = value; 
				gameCharacter.Animator.SetBool(inAttackIndex, inAttack);
			}
		} 
	}
	bool attackA = true;
	public bool AttackA { 
		get { return attackA; } 
		set {
			if (attackA != value) 
			{ 
				attackA = value;
				gameCharacter.Animator.SetBool(attackAIndex, attackA);
			}
		}
	}
	bool inFreez = false;
	public bool InFreez
	{
		get { return inFreez; }
		set
		{
			if (inFreez != value)
			{
				inFreez = value;
				gameCharacter.Animator.SetBool(inFreezIndex, inFreez);
			}
		}
	}
	bool freezA = true;
	public bool FreezA
	{
		get { return freezA; }
		set
		{
			if (freezA != value)
			{
				freezA = value;
				gameCharacter.Animator.SetBool(freezAIndex, freezA);
			}
		}
	}

	public float GetUpMovementCurve { get { return gameCharacter.Animator.GetFloat(upMovementCurveIndex); } }

	public AnimationController(GameCharacter character)
	{
		gameCharacter = character;
	}

	public void Start()
	{
		walkRunBlendID = Animator.StringToHash("WalkRunBlend");
		strideBlendID = Animator.StringToHash("StrideBlend");
		standingPlayRateID = Animator.StringToHash("StandingPlayRate");
		isGroundedID = Animator.StringToHash("IsGrounded");
		isMovingID = Animator.StringToHash("IsMoving");
		rotationBlendID = Animator.StringToHash("RotationBlend");
		rotationLayerIndex = gameCharacter.Animator.GetLayerIndex("RotationLayer");
		fallingBlendID = Animator.StringToHash("FallingBlend");
		jumpATriggerID = Animator.StringToHash("JumpA");
		jumpBTriggerID = Animator.StringToHash("JumpB");
		jumpStateAID = Animator.StringToHash("JumpStateA");
		layerAID = Animator.StringToHash("LayerA");
		spineLayerIndex = gameCharacter.Animator.GetLayerIndex("SpineLayer");
		legLayerIndex = gameCharacter.Animator.GetLayerIndex("LegLayer");
		headLayerIndex = gameCharacter.Animator.GetLayerIndex("HeadLayer");
		armLLayerIndex = gameCharacter.Animator.GetLayerIndex("ArmLLayer");
		armRLayerIndex = gameCharacter.Animator.GetLayerIndex("ArmRLayer");
		inAttackIndex = Animator.StringToHash("InAttack");
		attackAIndex = Animator.StringToHash("AttackA");
		attackAStateIndex = Animator.StringToHash("AttackAState");
		secondaryMotionLayerIndex = gameCharacter.Animator.GetLayerIndex("SecondaryMotion");
		upMovementCurveIndex = Animator.StringToHash("UpMovement");
		hitTriggerIndex = Animator.StringToHash("HitTrigger");
		hitLayerIndex = gameCharacter.Animator.GetLayerIndex("HitLayer");
		inFreezIndex = Animator.StringToHash("InFreez");
		freezAIndex = Animator.StringToHash("FreezA");

		overrideController = new AnimatorOverrideController(gameCharacter.Animator.runtimeAnimatorController);

		minMalkSpeed = gameCharacter.GameCharacterData.MaxMovementSpeed * gameCharacter.GameCharacterData.WalkFactor;
	}

	public void Update(float deltaTime)
	{
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Moving) IsMoving = true; else IsMoving = false;
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.InAir) IsGrounded = false; else IsGrounded = true;

		switch (gameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.InAir:
				startWalkRunBlendInterp = true;
				CalculateFallingValues();
				break;
			case EGameCharacterState.Moving: CalculateMovementValues(); break;
			default:
				// Set True everytime when case not Moving (Remember for new Cases!!!)
				startWalkRunBlendInterp = true;
				break;
		}

		RotationLayer(deltaTime);
		SpineLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(spineLayerIndex), spineLayerInterpTarget, deltaTime * spineLayerInterpSpeed);
		LegLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(legLayerIndex), legLayerInterpTarget, deltaTime * legLayerInterpSpeed);
		HeadLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(headLayerIndex), headLayerInterpTarget, deltaTime * headLayerInterpSpeed);
		ArmRLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(armRLayerIndex), armRLayerInterpTarget, deltaTime * armRLayerInterpSpeed);
		ArmLLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(armLLayerIndex), armLLayerInterpTarget, deltaTime * armLLayerInterpSpeed);
		SecondaryMotionLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(secondaryMotionLayerIndex), secondaryMotionLayerInterpTarget, deltaTime * secondaryMotionLayerInterpSpeed);
	}

	private void RotationLayer(float deltaTime)
	{
		// Rotation Blend
		if (RotationBlend != RotationTrarget)
		{
			if (Mathf.Abs(RotationBlend) < Mathf.Abs(RotationTrarget)) RotationBlend = RotationTrarget;
			else RotationBlend = Mathf.Lerp(RotationBlend, RotationTrarget, deltaTime * gameCharacter.GameCharacterData.RoationBlendInterp);
		}

		// Rotation Layer
		if (RotationBlend != 0) gameCharacter.Animator.SetLayerWeight(rotationLayerIndex, 1);
		else gameCharacter.Animator.SetLayerWeight(rotationLayerIndex, 0);
	}

	void CalculateFallingValues()
	{
		FallingBlend = Unity.Mathematics.math.remap(0, gameCharacter.GameCharacterData.MaxFallingVelocityAnim, 0, 1, gameCharacter.MovementComponent.Veloctiy.y);
	}

	void CalculateMovementValues()
	{
		bool dontInterp = startWalkRunBlendInterp;
		if (startWalkRunBlendInterp) startWalkRunBlendInterp = !startWalkRunBlendInterp;

		// Walk Run
		float movementSpeed = gameCharacter.MovementComponent.MovementSpeed;
		float walkrunblendTarget = Unity.Mathematics.math.remap(minMalkSpeed, gameCharacter.GameCharacterData.MaxMovementSpeed, 0.2f, 1, movementSpeed);
		WalkRunBlend = dontInterp ? walkrunblendTarget : Mathf.Lerp(WalkRunBlend, walkrunblendTarget, Time.deltaTime * gameCharacter.GameCharacterData.WalkRunInterp);

		// Stride
		float walkStride = gameCharacter.GameCharacterData.StrideBlendWalk.Evaluate(Unity.Mathematics.math.remap(0, minMalkSpeed, 0.2f, 1, movementSpeed));
		float runStride = gameCharacter.GameCharacterData.StrideBlendRun.Evaluate(Unity.Mathematics.math.remap(0, gameCharacter.GameCharacterData.MaxMovementSpeed, 0.2f, 1, movementSpeed));
		float strideTarget = Mathf.Lerp(walkStride, runStride, walkRunBlend);
		StrideBlend = dontInterp ? strideTarget : Mathf.Lerp(StrideBlend, strideTarget, Time.deltaTime * gameCharacter.GameCharacterData.StrideInterp);

		// Movement PlaySpeed
		float walkPlayRate = movementSpeed / minMalkSpeed;
		float runPlayRate = movementSpeed / gameCharacter.GameCharacterData.MaxMovementSpeed;
		float playbackSpeed = Mathf.Lerp(walkPlayRate, runPlayRate, WalkRunBlend);
		playbackSpeed = playbackSpeed / StrideBlend;
		playbackSpeed = playbackSpeed / gameCharacter.transform.localScale.z;
		playbackSpeed = Mathf.Clamp(playbackSpeed, 0.1f, 3);
		playbackSpeed = dontInterp ? playbackSpeed : Mathf.Lerp(StandingPlayRate, playbackSpeed, Time.deltaTime * gameCharacter.GameCharacterData.PlaybackInterp);
		StandingPlayRate = playbackSpeed;
	}

	public void Jump()
	{
		AnimatorStateInfo animatorState = gameCharacter.Animator.GetCurrentAnimatorStateInfo(0);
		bool isJumpStateA = false;
		//bool isJumpStateB = false;

		if (animatorState.shortNameHash == jumpStateAID) isJumpStateA = true;
		//if (animatorState.shortNameHash == jumpStateBID) isJumpStateB = true;

		int jumpIndex = gameCharacter.CurrentJumpAmount - 1;
		if (gameCharacter.CharacterData.CharacterAnimationData.Jumps.Count <= 0) return;
		jumpIndex = jumpIndex % gameCharacter.CharacterData.CharacterAnimationData.Jumps.Count;
		if (isJumpStateA)
		{
			overrideController["Jump2 PlaceHolder"] = gameCharacter.CharacterData.CharacterAnimationData.Jumps[jumpIndex];
			gameCharacter.Animator.runtimeAnimatorController = overrideController;
			gameCharacter.Animator.SetTrigger(jumpBTriggerID);
			
		} else
		{
			overrideController["Jump1 PlaceHolder"] = gameCharacter.CharacterData.CharacterAnimationData.Jumps[jumpIndex];
			gameCharacter.Animator.runtimeAnimatorController = overrideController;
			gameCharacter.Animator.SetTrigger(jumpATriggerID);
		}
	}
    
	public void Attack(AnimationClip attackClip)
	{
		AnimatorStateInfo animatorState = gameCharacter.Animator.GetCurrentAnimatorStateInfo(0);
		bool isAttackAState = false;

		if (animatorState.shortNameHash == attackAStateIndex) isAttackAState = true;
		if (isAttackAState)
		{
			overrideController["AttackB"] = attackClip;
		}else
		{
			overrideController["AttackA"] = attackClip;
		}
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
		AttackA = !isAttackAState;
	}

	public void LateUpdate()
	{

	}

	public void InterpSpineLayerWeight(float target, float speed = 5f)
	{
		spineLayerInterpTarget = target;
		spineLayerInterpSpeed = speed;
	}
	public void SetSpineLayerWeight(float weight)
	{
		spineLayerInterpTarget = weight;
		SpineLayerWeight = weight;
	}
	public void InterpLegLayerWeight(float target, float speed = 5f)
	{
		legLayerInterpTarget = target;
		legLayerInterpSpeed = speed;
	}
	public void SetLegLayerWeight(float weight)
	{
		legLayerInterpTarget = weight;
		LegLayerWeight = weight;
	}
	public void InterpHeadLayerWeight(float target, float speed = 5f)
	{
		headLayerInterpTarget = target;
		headLayerInterpSpeed = speed;
	}
	public void SetHeadLayerWeight(float weight)
	{
		headLayerInterpTarget = weight;
		HeadLayerWeight = weight;
	}
	public void InterpArmRLayerWeight(float target, float speed = 5f)
	{
		armRLayerInterpTarget = target;
		armRLayerInterpSpeed = speed;
	}
	public void SetArmRLayerWeight(float weight)
	{
		armRLayerInterpTarget = weight;
		ArmRLayerWeight = weight;
	}
	public void InterpArmLLayerWeight(float target, float speed = 5f)
	{
		armLLayerInterpTarget = target;
		armLLayerInterpSpeed = speed;
	}
	public void SetArmLLayerWeight(float weight)
	{
		armLLayerInterpTarget = weight;
		ArmLLayerWeight = weight;
	}
	public void InterpSecondaryMotionLayerWeight(float target, float speed = 5f)
	{
		secondaryMotionLayerInterpTarget = target;
		secondaryMotionLayerInterpSpeed = speed;
	}
	public void SetSecondaryMotionLayerWeight(float weight)
	{
		secondaryMotionLayerInterpTarget = weight;
		SecondaryMotionLayerWeight = weight;
	}
	public void InterpUpperBodyWeight(float target, float speed = 5f)
	{
		InterpSpineLayerWeight(target, speed);
		InterpHeadLayerWeight(target, speed);
		InterpArmRLayerWeight(target, speed);
		InterpArmLLayerWeight(target, speed);
	}
	public void SetUpperBodyWeight(float weight)
	{
		SetSpineLayerWeight(weight); 
		SetHeadLayerWeight(weight);
		SetArmLLayerWeight(weight);
		SetArmRLayerWeight(weight);
	}

	public void SetBodyLayerAnimClip(AnimationClip clip)
	{
		if (UpperBodyIsA)
		{
			overrideController["BodyLayerB"] = clip;
		}else
		{
			overrideController["BodyLayerA"] = clip;
		}
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
		UpperBodyIsA = !UpperBodyIsA;
	}

	public void TriggerAdditiveHit()
	{
		gameCharacter.Animator.SetTrigger(hitTriggerIndex);
	}
}
