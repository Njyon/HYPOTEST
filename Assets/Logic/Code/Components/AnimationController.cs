using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationController
{
	GameCharacter gameCharacter;

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
	int jumpStateBID;
	int upperBodyIsAID;
	int lowerBodyIsAID;
	int upperBodyLayerIndex;
	int lowerBodyLayerIndex;
	int upperBodyAStateID;
	int upperBodyBStateID;
	int lowerBodyAStateID;
	int lowerBodyBStateID;

	int rotationLayerIndex;

	float minMalkSpeed;
	bool startWalkRunBlendInterp = true;
	float upperBodyInterpTarget;
	float upperBodyInterpSpeed;
	float lowerBodyInterpTarget;
	float lowerBodyInterpSpeed;

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
	bool upperBodyIsA;
	public bool UpperBodyIsA { 
		get { return upperBodyIsA; } 
		private set
		{
			if (upperBodyIsA != value)
			{
				upperBodyIsA = value;
				gameCharacter.Animator.SetBool(upperBodyIsAID, upperBodyIsA);
			}
		}
	}
	bool lowerBodyIsA;
	public bool LowerBodyIsA { 
		get { return lowerBodyIsA; } 
		private set
		{
			if (lowerBodyIsA != value)
			{
				lowerBodyIsA = value;
				gameCharacter.Animator.SetBool(lowerBodyIsAID, lowerBodyIsA);
			}
		}
	}
	public float UpperBodyLayerWeight {
		get { return gameCharacter.Animator.GetLayerWeight(upperBodyLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(upperBodyLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(upperBodyLayerIndex, value);
			}
		}
	}
	public float LowerBodyLayerWeight {
		get { return gameCharacter.Animator.GetLayerWeight(lowerBodyLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(lowerBodyLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(lowerBodyLayerIndex, value);
			}
		}
	}

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
		jumpStateBID = Animator.StringToHash("JumpStateB");
		upperBodyIsAID = Animator.StringToHash("UpperBodyIsA");
		lowerBodyIsAID = Animator.StringToHash("LowerBodyIsA");
		upperBodyLayerIndex = gameCharacter.Animator.GetLayerIndex("UpperBodyLayer");
		lowerBodyLayerIndex = gameCharacter.Animator.GetLayerIndex("LegLayer");
		upperBodyAStateID = Animator.StringToHash("UpperBodyStateA");
		upperBodyBStateID = Animator.StringToHash("UpperBodyStateB");
		lowerBodyAStateID = Animator.StringToHash("LegLayerStateA");
		lowerBodyBStateID = Animator.StringToHash("LegLayerStateB");

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
		UpperBodyLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(upperBodyLayerIndex), upperBodyInterpTarget, deltaTime * upperBodyInterpSpeed);
		LowerBodyLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(lowerBodyLayerIndex), lowerBodyInterpTarget, deltaTime * lowerBodyInterpSpeed);
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
		FallingBlend = Unity.Mathematics.math.remap(0, gameCharacter.GameCharacterData.MaxFallingVelocityAnim, 0, 1, gameCharacter.Veloctiy.y);
	}

	void CalculateMovementValues()
	{
		bool dontInterp = startWalkRunBlendInterp;
		if (startWalkRunBlendInterp) startWalkRunBlendInterp = !startWalkRunBlendInterp;

		// Walk Run
		float movementSpeed = gameCharacter.MovementSpeed;
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
		AnimatorOverrideController overrideController = new AnimatorOverrideController(gameCharacter.Animator.runtimeAnimatorController);
		if (isJumpStateA)
		{
			overrideController["Jump2 PlaceHolder"] = gameCharacter.CharacterData.CharacterAnimationData.Jumps[jumpIndex];
			gameCharacter.Animator.SetTrigger(jumpBTriggerID);
			
		} else
		{
			overrideController["Jump1 PlaceHolder"] = gameCharacter.CharacterData.CharacterAnimationData.Jumps[jumpIndex];
			gameCharacter.Animator.SetTrigger(jumpATriggerID);
		}
		gameCharacter.Animator.runtimeAnimatorController = overrideController;

	}
    
	public void LateUpdate()
	{

	}

	public void InterpUpperBodyLayerWeight(float target, float speed = 5f)
	{
		upperBodyInterpTarget = target;
		upperBodyInterpSpeed = speed;
	}
	public void SetUpperBodyLayerWeight(float weight)
	{
		upperBodyInterpTarget = weight;
		UpperBodyLayerWeight = weight;
	}
	public void InterpLowerBodyLayerWeight(float target, float speed = 5f)
	{
		lowerBodyInterpTarget = target;
		lowerBodyInterpSpeed = speed;
	}
	public void SetLowerBodyLayerWeight(float weight)
	{
		lowerBodyInterpTarget = weight;
		LowerBodyLayerWeight = weight;
	}
	public void SetUpperBodyAnimClip(AnimationClip clip)
	{
		AnimatorStateInfo animatorState = gameCharacter.Animator.GetCurrentAnimatorStateInfo(upperBodyLayerIndex);
		bool isAState = false;

		if (animatorState.shortNameHash == upperBodyAStateID) isAState = true;
		AnimatorOverrideController overrideController = new AnimatorOverrideController(gameCharacter.Animator.runtimeAnimatorController);
		if (!isAState)
		{
			overrideController["UpperBodyLayerPlaceHolderA"] = clip;
		}else
		{
			overrideController["UpperBodyLayerPlaceHolderB"] = clip;
		}
		UpperBodyIsA = isAState;
	}
	public void SetLowerBodyAnimClip(AnimationClip clip)
	{
		AnimatorStateInfo animatorState = gameCharacter.Animator.GetCurrentAnimatorStateInfo(lowerBodyLayerIndex);
		bool isAState = false;

		if (animatorState.shortNameHash == lowerBodyAStateID) isAState = true;
		AnimatorOverrideController overrideController = new AnimatorOverrideController(gameCharacter.Animator.runtimeAnimatorController);
		if (!isAState)
		{
			overrideController["LegLayerPlaceHolderA"] = clip;
		}
		else
		{
			overrideController["LegLayerPlaceHolderB"] = clip;
		}
		LowerBodyIsA = isAState;
	}
}
