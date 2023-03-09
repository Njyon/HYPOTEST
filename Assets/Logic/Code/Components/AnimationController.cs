using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController
{
	GameCharacter gameCharacter;

	int walkRunBlendID;
	int strideBlendID;
	int standingPlayRateID;
	int isGroundedID;
	int IsMovingID;

	float minMalkSpeed;
	bool startWalkRunBlendInterp = true;

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
				gameCharacter.Animator.SetBool(IsMovingID, isMoving);
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
		IsMovingID = Animator.StringToHash("IsMoving");

		minMalkSpeed = gameCharacter.CharacterData.MaxMovementSpeed * gameCharacter.CharacterData.WalkFactor;
	}

	public void Update(float deltaTime)
	{
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Moving) IsMoving = true; else IsMoving = false;
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.InAir) IsGrounded = false; else IsGrounded = true;

		switch (gameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Moving: CalculateMovementValues(); break;
			default:
				// Set True everytime when case not Moving (Remember for new Cases!!!)
				startWalkRunBlendInterp = true;
				break;
		}
	}

	private void CalculateMovementValues()
	{
		bool dontInterp = startWalkRunBlendInterp;
		if (startWalkRunBlendInterp) startWalkRunBlendInterp = !startWalkRunBlendInterp;

		// Walk Run
		float movementSpeed = gameCharacter.MovementSpeed;
		float walkrunblendTarget = Unity.Mathematics.math.remap(minMalkSpeed, gameCharacter.CharacterData.MaxMovementSpeed, 0.2f, 1, movementSpeed);
		WalkRunBlend = dontInterp ? walkrunblendTarget : Mathf.Lerp(WalkRunBlend, walkrunblendTarget, Time.deltaTime * gameCharacter.CharacterData.WalkRunInterp);

		// Stride
		float walkStride = gameCharacter.CharacterData.StrideBlendWalk.Evaluate(Unity.Mathematics.math.remap(0, minMalkSpeed, 0.2f, 1, movementSpeed));
		float runStride = gameCharacter.CharacterData.StrideBlendRun.Evaluate(Unity.Mathematics.math.remap(0, gameCharacter.CharacterData.MaxMovementSpeed, 0.2f, 1, movementSpeed));
		float strideTarget = Mathf.Lerp(walkStride, runStride, walkRunBlend);
		StrideBlend = dontInterp ? strideTarget : Mathf.Lerp(StrideBlend, strideTarget, Time.deltaTime * gameCharacter.CharacterData.StrideInterp);

		// Movement PlaySpeed
		float walkPlayRate = movementSpeed / minMalkSpeed;
		float runPlayRate = movementSpeed / gameCharacter.CharacterData.MaxMovementSpeed;
		float playbackSpeed = Mathf.Lerp(walkPlayRate, runPlayRate, WalkRunBlend);
		playbackSpeed = playbackSpeed / StrideBlend;
		playbackSpeed = playbackSpeed / gameCharacter.transform.localScale.z;
		playbackSpeed = Mathf.Clamp(playbackSpeed, 0.1f, 3);
		playbackSpeed = dontInterp ? playbackSpeed : Mathf.Lerp(StandingPlayRate, playbackSpeed, Time.deltaTime * gameCharacter.CharacterData.PlaybackInterp);
		StandingPlayRate = playbackSpeed;
	}

	public void LateUpdate()
	{

	}
}
