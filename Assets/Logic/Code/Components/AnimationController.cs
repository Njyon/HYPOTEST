using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
	int holdAttackIndex;
	int triggerAttackIndex;
	int inDefensiveActionIndex;
	int inAimBlendTreeIndex;
	int inAimBlendTreeAIndex;
	int inAimBlendTreeAStateIndex;
	int aimBlendIndex;
	int inUpperBodyLayerAIndex;
	int upperBodyLayerIndex;
	int flyAwayIndex;
	int flyAwayDirIndex;
	int inCombat3BlendIndex;
	int combat3BlendDirIndex;
	int inCombat3BlendStateAIndex;
	int combat3BlendTreeAStateIndex;

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
	float upperBodyLayerInterpTarget;
	float upperBodyLayerInterpSpeed;

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
	public bool HeadSpineArmIsA { 
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
	public float UpperBodyLayerWeight
	{
		get { return gameCharacter.Animator.GetLayerWeight(upperBodyLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(upperBodyLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(upperBodyLayerIndex, value);
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
	bool holdAttack = false;
	public bool HoldAttack
	{
		get { return holdAttack; }
		set
		{
			if (holdAttack != value)
			{
				holdAttack = value;
				gameCharacter.Animator.SetBool(holdAttackIndex, holdAttack);
			}
		}
	}
	bool triggerAttack = false;
	public bool TriggerAttack
	{
		get { return triggerAttack; }
		set
		{
			if (triggerAttack != value)
			{
				triggerAttack = value;
				gameCharacter.Animator.SetBool(triggerAttackIndex, triggerAttack);
			}
		}
	}
	bool inDefensiveAction = false;
	public bool InDefensiveAction
	{
		get { return inDefensiveAction; }
		set
		{
			if (inDefensiveAction != value)
			{
				inDefensiveAction = value;
				gameCharacter.Animator.SetBool(inDefensiveActionIndex, inDefensiveAction);
			}
		}
	}
	bool inAimBlendTree = false;
	public bool InAimBlendTree
	{
		get { return inAimBlendTree; }
		set 
		{
			if (inAimBlendTree != value)
			{
				inAimBlendTree = value;
				gameCharacter.Animator.SetBool(inAimBlendTreeIndex, inAimBlendTree);
			}
		}
	}
	bool inAimBlendTreeA = true;
	public bool InAimBlendTreeA
	{
		get { return inAimBlendTreeA; }
		set
		{
			if (inAimBlendTreeA != value)
			{
				inAimBlendTreeA = value;
				gameCharacter.Animator.SetBool(inAimBlendTreeAIndex, inAimBlendTreeA);
			}
		}
	}
	float aimBlend = 0f;
	public float AimBlend
	{
		get { return aimBlend; }
		set
		{
			if (aimBlend != value)
			{
				aimBlend = value;
				gameCharacter.Animator.SetFloat(aimBlendIndex, aimBlend);
			}
		}
	}
	bool inUpperBodyLayerA = true;
	public bool InUpperBodyLayerA
	{
		get { return inUpperBodyLayerA; }
		set 
		{ 
			if (inUpperBodyLayerA != value)
			{
				inUpperBodyLayerA = value;
				gameCharacter.Animator.SetBool(inUpperBodyLayerAIndex, inUpperBodyLayerA);
			}
		} 
	}
	bool flyAway = false;
	public bool FlyAway
	{
		get { return flyAway; }
		set 
		{
			if (flyAway != value)
			{
				flyAway = value;
				gameCharacter.Animator.SetBool(flyAwayIndex, flyAway);
			}
		}
	}
	float flyAwayDir = 0f;
	public float FlyAwayDir
	{
		get { return flyAwayDir; }
		set
		{
			if (flyAwayDir != value)
			{
				flyAwayDir = value;
				gameCharacter.Animator.SetFloat(flyAwayDirIndex, flyAwayDir);
			}
		}
	}
	bool inCombat3Blend = false;
	public bool InCombat3Blend
	{
		get { return inCombat3Blend; }
		set
		{
			if (inCombat3Blend != value)
			{
				inCombat3Blend = value;
				gameCharacter.Animator.SetBool(inCombat3BlendIndex, inCombat3Blend);
			}
		}
	}
	float combat3BlendDir = 0f;
	public float Combat3BlendDir
	{
		get { return combat3BlendDir; }
		set
		{
			if (combat3BlendDir != value)
			{
				combat3BlendDir = value;
				gameCharacter.Animator.SetFloat(combat3BlendDirIndex, combat3BlendDir);
			}
		}
	}
	bool inCombat3BlendStateA = true;
	public bool InCombat3BlendStateA
	{
		get { return inCombat3BlendStateA; }
		set
		{
			if (inCombat3BlendStateA != value)
			{
				inCombat3BlendStateA = value;
				gameCharacter.Animator.SetBool(inCombat3BlendStateAIndex, inCombat3BlendStateA);
			}
		}
	}

	bool blockRotation = false;
	public bool BlockRotation { get { return blockRotation; } set { blockRotation = value; } }

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
		holdAttackIndex = Animator.StringToHash("HoldAttack");
		triggerAttackIndex = Animator.StringToHash("TriggerAttack");
		inDefensiveActionIndex = Animator.StringToHash("InDefensiveAction");
		inAimBlendTreeIndex = Animator.StringToHash("InAimBlendTree");
		inAimBlendTreeAIndex = Animator.StringToHash("InAimBlendTreeA");
		aimBlendIndex = Animator.StringToHash("AimBlend");
		inAimBlendTreeAStateIndex = Animator.StringToHash("AimBlendTreeA");
		inUpperBodyLayerAIndex = Animator.StringToHash("InUpperBodyLayerA");
		upperBodyLayerIndex = gameCharacter.Animator.GetLayerIndex("UpperBodyLayer");
		flyAwayIndex = Animator.StringToHash("FlyAway");
		flyAwayDirIndex = Animator.StringToHash("FlyAwayDir");
		inCombat3BlendIndex = Animator.StringToHash("InCombat3Blend");
		combat3BlendDirIndex = Animator.StringToHash("Combat3BlendDir");
		inCombat3BlendStateAIndex = Animator.StringToHash("InCombat3BlendAState");
		combat3BlendTreeAStateIndex = Animator.StringToHash("Combat3BlendTreeA");

		overrideController = new AnimatorOverrideController(gameCharacter.Animator.runtimeAnimatorController);

		minMalkSpeed = gameCharacter.GameCharacterData.MaxMovementSpeed * gameCharacter.GameCharacterData.WalkFactor;
	}

	public void Update(float deltaTime)
	{
		IsMoving = Mathf.Abs(gameCharacter.MovementComponent.Velocity.x) > 0;
		IsGrounded = gameCharacter.MovementComponent.IsGrounded;

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

		if (BlockRotation)
		{
			gameCharacter.Animator.SetLayerWeight(rotationLayerIndex, 0);
		}
		else
		{
			RotationLayer(deltaTime);
		}

		SpineLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(spineLayerIndex), spineLayerInterpTarget, deltaTime * spineLayerInterpSpeed);
		LegLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(legLayerIndex), legLayerInterpTarget, deltaTime * legLayerInterpSpeed);
		HeadLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(headLayerIndex), headLayerInterpTarget, deltaTime * headLayerInterpSpeed);
		ArmRLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(armRLayerIndex), armRLayerInterpTarget, deltaTime * armRLayerInterpSpeed);
		ArmLLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(armLLayerIndex), armLLayerInterpTarget, deltaTime * armLLayerInterpSpeed);
		SecondaryMotionLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(secondaryMotionLayerIndex), secondaryMotionLayerInterpTarget, deltaTime * secondaryMotionLayerInterpSpeed);
		UpperBodyLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(upperBodyLayerIndex), upperBodyLayerInterpTarget, deltaTime * upperBodyLayerInterpSpeed);
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
		FallingBlend = Unity.Mathematics.math.remap(0, gameCharacter.GameCharacterData.MaxFallingVelocityAnim, 0, 1, gameCharacter.MovementComponent.Velocity.y);
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
		bool isAttackAState = IsInState(attackAStateIndex);
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

	public void ApplyBlendTree(AimBlendAnimations anims)
	{
		bool isBlendAState = IsInState(inAimBlendTreeAStateIndex);
		if (isBlendAState)
		{
			overrideController["AimBlendSpaceUpB"] = anims.upAnimation;
			overrideController["AimBlendSpaceMidB"] = anims.midAnimation;
			overrideController["AimBlendSpaceDownB"] = anims.downAnimation;
		}
		else
		{
			overrideController["AimBlendSpaceUpA"] = anims.upAnimation;
			overrideController["AimBlendSpaceMidA"] = anims.midAnimation;
			overrideController["AimBlendSpaceDownA"] = anims.downAnimation;
		}
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
		InAimBlendTreeA = !isBlendAState;
	}
	public void ApplyCombat3BlendTree(AimBlendAnimations anims)
	{
		bool isBlendAState = IsInState(inCombat3BlendStateAIndex);
		if (isBlendAState)
		{
			overrideController["Combat3BlendB1"] = anims.upAnimation;
			overrideController["Combat3BlendB0"] = anims.midAnimation;
			overrideController["Combat3BlendB-1"] = anims.downAnimation;
		}
		else
		{
			overrideController["Combat3BlendA1"] = anims.upAnimation;
			overrideController["Combat3BlendA0"] = anims.midAnimation;
			overrideController["Combat3BlendA-1"] = anims.downAnimation;
		}
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
		InCombat3BlendStateA = !isBlendAState;
	}

	private bool IsInState(int stateIndex)
	{
		AnimatorStateInfo animatorState = gameCharacter.Animator.GetCurrentAnimatorStateInfo(0);
		bool state = false;

		if (animatorState.shortNameHash == stateIndex) state = true;
		return state;
	}

	public void SetHoldAttack(AnimationClip holdClip)
	{
		overrideController["Hold"] = holdClip;
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
	}

	public void SetTriggerAttack(AnimationClip triggerClip)
	{
		overrideController["Trigger"] = triggerClip;
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
	}

	public void SetDefensiveAction(AnimationClip defensiveActionClip) 
	{
		overrideController["DefensiveAction"] = defensiveActionClip;
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
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
	public void InterpHeadSpineArmWeight(float target, float speed = 5f)
	{
		InterpSpineLayerWeight(target, speed);
		InterpHeadLayerWeight(target, speed);
		InterpArmRLayerWeight(target, speed);
		InterpArmLLayerWeight(target, speed);
	}
	public void SetHeadSpineArmWeight(float weight)
	{
		SetSpineLayerWeight(weight); 
		SetHeadLayerWeight(weight);
		SetArmLLayerWeight(weight);
		SetArmRLayerWeight(weight);
	}

	public void InterpUpperBodyLayerWeight(float target, float speed = 5f)
	{
		upperBodyLayerInterpTarget = target;
		upperBodyLayerInterpSpeed = speed;
	}
	public void SetUpperBodyLayerWeight(float weight)
	{
		upperBodyLayerInterpTarget = weight;
		UpperBodyLayerWeight = weight;
	}

	public void SetBodyLayerAnimClip(AnimationClip clip)
	{
		if (HeadSpineArmIsA)
		{
			overrideController["BodyLayerB"] = clip;
		}else
		{
			overrideController["BodyLayerA"] = clip;
		}
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
		HeadSpineArmIsA = !HeadSpineArmIsA;
	}

	public void TriggerAdditiveHit()
	{
		gameCharacter.Animator.SetTrigger(hitTriggerIndex);
	}

	public void SwitchFreezState()
	{
		if (InFreez) FreezA = !FreezA;
	}
}
