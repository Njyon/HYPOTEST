using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EGameCharacterAnimatiorLayers
{
	BasisLayer,
	SpineLayer,
	LegLayer,
	HeadLayer,
	ArmRLayer,
	ArmLLayer,
	UpperBodyLayer,
	RotationLayer,
	UpperBodyAddativeLayer,
	SecondaryMotionLayer,
	HitLayer,
}

[Serializable]
public class ToggableDebugFloat
{
	public ToggableDebugFloat() { }

	public bool useDebugValue = false;
	[ConditionalField("useDebugValue")] [Range(0f, 1f)]
	public float debugFloat = 0.0f;	
}

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
	int attackBStateIndex;
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
	int inAimBlendTreeBStateIndex;
	int aimBlendIndex;
	int inUpperBodyLayerAIndex;
	int upperBodyLayerIndex;
	int flyAwayIndex;
	int flyAwayDirIndex;
	int inCombat3BlendIndex;
	int combat3BlendDirIndex;
	int inCombat3BlendStateAIndex;
	int inCombat3BlendStateBIndex;
	int combat3BlendTreeAStateIndex;
	int combat3BlendTreeBStateIndex;
	int inDodgeIndex;
	int attackTriggerStateIndex;
	int attackHoldStateIndex;
	int dodgeStateIndex;
	int addativeLayerIndex;
	int additiveAStateIndex;
	int additiveBStateIndex;
	int inAddativeStateIndex;
	int inAddativeAStateIndex;
	int upperBodyAddativeLayerIndex;
	int inUpperBodyAddativeStateIndex;
	int upperBodyAddativeAStateIndex;
	int inUpperBodyAddativeAIndex;
	int moveBackwardsIndex;

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
	float addativeLayerInterpTarget;
	float addativeLayerInterpSpeed;
	float chestCorrectionTargetInterpSpeed;
	float forcePositionFootIKRTargetInterpSpeed;
	float forcePositionFootIKLTargetInterpSpeed;

	int holdStateHash;
	public int HoldStateHash
	{
		get
		{
			if (holdStateHash == 0)
			{
				holdStateHash = Animator.StringToHash("BasisLayer.Combat.CombatLoseStates.Hold");
			}
			return holdStateHash;
		}
	}
	int dodgeStateHash;
	public int DodgeStateHash
	{
		get
		{
			if (dodgeStateHash == 0)
			{
				dodgeStateHash = Animator.StringToHash("BasisLayer.Combat.CombatLoseStates.Dodge");
			}
			return dodgeStateHash;
		}
	}
	int triggerStateHash;
	public int TriggerStateHash
	{
		get
		{
			if (triggerStateHash == 0)
			{
				triggerStateHash = Animator.StringToHash("BasisLayer.Combat.CombatLoseStates.Trigger");
			}
			return triggerStateHash;
		}
	}
	int defensiveActionStateHash;
	public int DefensiveActionStateHash
	{
		get
		{
			if (defensiveActionStateHash == 0)
			{
				defensiveActionStateHash = Animator.StringToHash("BasisLayer.Combat.CombatLoseStates.DefensiveAction");
			}
			return defensiveActionStateHash;
		}
	}
	int standingStateHash;
	public int StandingStateHash
	{
		get
		{
			if (standingStateHash == 0)
			{
				standingStateHash = Animator.StringToHash("BasisLayer.OnGround.Standing");
			}
			return standingStateHash;
		}
	}

	float walkRunBlend;
	public float WalkRunBlend
	{
		get { return walkRunBlend; }
		private set
		{
			value = Mathf.Clamp01(value);
			if (walkRunBlend != value)
			{
				walkRunBlend = value;
				gameCharacter.Animator?.SetFloat(walkRunBlendID, walkRunBlend);
			}
		}
	}
	float strideBlend;
	public float StrideBlend
	{
		get { return strideBlend; }
		private set
		{
			value = Mathf.Clamp01(value);
			if (strideBlend != value)
			{
				strideBlend = value;
				gameCharacter.Animator?.SetFloat(strideBlendID, strideBlend);
			}
		}
	}
	float standingPlayRate;
	public float StandingPlayRate
	{
		get { return standingPlayRate; }
		private set
		{
			if (standingPlayRate != value)
			{
				standingPlayRate = value;
				gameCharacter.Animator?.SetFloat(standingPlayRateID, standingPlayRate);
			}
		}
	}
	bool isGrounded;
	public bool IsGrounded
	{
		get { return isGrounded; }
		private set
		{
			if (isGrounded != value)
			{
				isGrounded = value;
				gameCharacter.Animator?.SetBool(isGroundedID, isGrounded);
			}
		}
	}
	bool isMoving;
	public bool IsMoving
	{
		get { return isMoving; }
		private set
		{
			if (isMoving != value)
			{
				isMoving = value;
				gameCharacter.Animator?.SetBool(isMovingID, isMoving);
			}
		}
	}
	float rotationTarget;
	public float RotationTrarget { get { return rotationTarget; } set { rotationTarget = value; } }
	float rotationBlend;
	public float RotationBlend
	{
		get { return rotationBlend; }
		private set
		{
			if (rotationBlend != value)
			{
				//if (Ultra.Utilities.IsNearlyEqual(value, 0, 0.001f)) value = 0;
				//rotationBlend = value;
				//gameCharacter.Animator.SetFloat(rotationBlendID, rotationBlend);
			}
		}
	}
	float fallingBlend;
	public float FallingBlend
	{
		get { return fallingBlend; }
		private set
		{
			if (fallingBlend != value)
			{
				fallingBlend = value;
				gameCharacter.Animator?.SetFloat(fallingBlendID, fallingBlend);
			}
		}
	}
	bool layerA = true;
	public bool HeadSpineArmIsA
	{
		get { return layerA; }
		private set
		{
			if (layerA != value)
			{
				layerA = value;
				gameCharacter.Animator?.SetBool(layerAID, layerA);
			}
		}
	}
	public float SpineLayerWeight
	{
		get { return gameCharacter.Animator.GetLayerWeight(spineLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(spineLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(spineLayerIndex, value);
			}
		}
	}
	public float LegLayerWeight
	{
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
	public float AddativeLayerWeight
	{
		get { return gameCharacter.Animator.GetLayerWeight(addativeLayerIndex); }
		private set
		{
			if (gameCharacter.Animator.GetLayerWeight(addativeLayerIndex) != value)
			{
				gameCharacter.Animator.SetLayerWeight(addativeLayerIndex, value);
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
	public bool InAttack
	{
		get { return inAttack; }
		set
		{
			if (inAttack != value)
			{
				inAttack = value;
				gameCharacter.Animator?.SetBool(inAttackIndex, inAttack);
			}
		}
	}
	bool attackA = true;
	public bool AttackA
	{
		get { return attackA; }
		set
		{
			if (attackA != value)
			{
				attackA = value;
				gameCharacter.Animator?.SetBool(attackAIndex, attackA);
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
				gameCharacter.Animator?.SetBool(inFreezIndex, inFreez);
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
				gameCharacter.Animator?.SetBool(freezAIndex, freezA);
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
				gameCharacter.Animator?.SetBool(holdAttackIndex, holdAttack);
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
				gameCharacter.Animator?.SetBool(triggerAttackIndex, triggerAttack);
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
				gameCharacter.Animator?.SetBool(inDefensiveActionIndex, inDefensiveAction);
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
				gameCharacter.Animator?.SetBool(inAimBlendTreeIndex, inAimBlendTree);
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
				gameCharacter.Animator?.SetBool(inAimBlendTreeAIndex, inAimBlendTreeA);
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
				gameCharacter.Animator?.SetFloat(aimBlendIndex, aimBlend);
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
				gameCharacter.Animator?.SetBool(inUpperBodyLayerAIndex, inUpperBodyLayerA);
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
				gameCharacter.Animator?.SetBool(flyAwayIndex, flyAway);
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
				gameCharacter.Animator?.SetFloat(flyAwayDirIndex, flyAwayDir);
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
				gameCharacter.Animator?.SetBool(inCombat3BlendIndex, inCombat3Blend);
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
				gameCharacter.Animator?.SetFloat(combat3BlendDirIndex, combat3BlendDir);
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
				gameCharacter.Animator?.SetBool(inCombat3BlendStateAIndex, inCombat3BlendStateA);
			}
		}
	}
	bool inDodge = false;
	public bool InDodge
	{
		get { return inDodge; }
		set
		{
			if (inDodge != value)
			{
				inDodge = value;
				gameCharacter.Animator?.SetBool(inDodgeIndex, inDodge);
			}
		}
	}
	bool inAddativeState = false;
	public bool InAddativeState
	{
		get { return inAddativeState; }
		set
		{
			if (inAddativeState != value)
			{
				inAddativeState = value;
				gameCharacter.Animator?.SetBool(inAddativeStateIndex, inAddativeState);
			}
		}
	}
	bool inAddativeAState = false;
	public bool InAddativeAState
	{
		get {return inAddativeAState; }
		set
		{
			if (inAddativeAState != value)
			{
				inAddativeAState = value;
				gameCharacter.Animator?.SetBool(inAddativeAStateIndex, inAddativeAState);
			}
		}
	}
	bool inUpperBodyAddativeState = false;
	public bool InUpperBodyAddativeState
	{
		get { return inUpperBodyAddativeState; }
		set
		{
			if (inUpperBodyAddativeState != value)
			{
				inUpperBodyAddativeState = value;
				gameCharacter.Animator?.SetBool(inUpperBodyAddativeStateIndex, inUpperBodyAddativeState);
			}
		}
	}
	bool inUpperBodyAddativeAState = false;
	public bool InUpperBodyAddativeAState
	{
		get { return inUpperBodyAddativeAState; }
		set
		{
			if (inUpperBodyAddativeAState != value)
			{
				inUpperBodyAddativeAState = value;
				gameCharacter.Animator?.SetBool(inUpperBodyAddativeAIndex, inUpperBodyAddativeAState);
			}
		}
	}
	bool moveBackwards = false;
	public bool MoveBackwards
	{
		get { return moveBackwards; }
		set
		{
			if (moveBackwards != value)
			{
				moveBackwards = value;
				gameCharacter.Animator?.SetBool(moveBackwardsIndex, moveBackwards);
			}
		}
	}

	float chestCorrectionIKTarget;
	public float ChestCorrectionIKTarget { get { return chestCorrectionIKTarget; } }
	float forcePositionFootIKRTarget;
	public float ForcePositionFootIKRTarget { get { return forcePositionFootIKRTarget; } }
	float forcePositionFootIKLTarget;
	public float ForcePositionFootIKLTarget { get { return forcePositionFootIKLTarget; } }

	public float ChestCorrectionIK { get { return gameCharacter.GameCharacterData.ChestCorrectionIK != null ? gameCharacter.GameCharacterData.ChestCorrectionIK.weight : -1; } set { if (gameCharacter.GameCharacterData.ChestCorrectionIK != null) gameCharacter.GameCharacterData.ChestCorrectionIK.weight = value; } }
	public float ForcePositionFootIKR { get { return gameCharacter.GameCharacterData.ForcePosition_FootIK_R != null ? gameCharacter.GameCharacterData.ForcePosition_FootIK_R.weight : -1; } set { if (gameCharacter.GameCharacterData.ForcePosition_FootIK_R != null) gameCharacter.GameCharacterData.ForcePosition_FootIK_R.weight = value; } }
	public float ForcePositionFootIKL { get { return gameCharacter.GameCharacterData.ForcePosition_FootIK_L != null ? gameCharacter.GameCharacterData.ForcePosition_FootIK_L.weight : -1; } set { if (gameCharacter.GameCharacterData.ForcePosition_FootIK_L != null) gameCharacter.GameCharacterData.ForcePosition_FootIK_L.weight = value; } }

	bool blockRotation = false;
	public bool BlockRotation { get { return blockRotation; } set { blockRotation = value; } }

	public float GetUpMovementCurve { get { return gameCharacter.Animator.GetFloat(upMovementCurveIndex); } }
	Ultra.Timer attackTriggerTimer = new Ultra.Timer();
	public Ultra.Timer AttackTriggerTimer { get { return attackTriggerTimer; } }

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
		attackBStateIndex = Animator.StringToHash("AttackBState");
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
		inAimBlendTreeBStateIndex = Animator.StringToHash("AimBlendTreeB");
		inUpperBodyLayerAIndex = Animator.StringToHash("InUpperBodyLayerA");
		upperBodyLayerIndex = gameCharacter.Animator.GetLayerIndex("UpperBodyLayer");
		flyAwayIndex = Animator.StringToHash("FlyAway");
		flyAwayDirIndex = Animator.StringToHash("FlyAwayDir");
		inCombat3BlendIndex = Animator.StringToHash("InCombat3Blend");
		combat3BlendDirIndex = Animator.StringToHash("Combat3BlendDir");
		inCombat3BlendStateAIndex = Animator.StringToHash("InCombat3BlendAState");
		inCombat3BlendStateBIndex = Animator.StringToHash("InCombat3BlendBState");
		combat3BlendTreeAStateIndex = Animator.StringToHash("Combat3BlendTreeA");
		combat3BlendTreeBStateIndex = Animator.StringToHash("Combat3BlendTreeB");
		inDodgeIndex = Animator.StringToHash("inDodge");
		attackTriggerStateIndex = Animator.StringToHash("TriggerState");
		attackHoldStateIndex = Animator.StringToHash("HoldState");
		dodgeStateIndex = Animator.StringToHash("DodgeState");
		addativeLayerIndex = gameCharacter.Animator.GetLayerIndex("AdditiveLayer");
		additiveAStateIndex = Animator.StringToHash("AdditiveAState");
		additiveBStateIndex = Animator.StringToHash("AdditiveBState");
		inAddativeStateIndex = Animator.StringToHash("InAddativeState");
		inAddativeAStateIndex = Animator.StringToHash("InAddativeAState");
		upperBodyAddativeLayerIndex = gameCharacter.Animator.GetLayerIndex("UpperBodyAddativeLayer");
		inUpperBodyAddativeStateIndex = Animator.StringToHash("InUpperBodyAddativeState");
		upperBodyAddativeAStateIndex = Animator.StringToHash("UpperBodyAdditiveAState");
		inUpperBodyAddativeAIndex = Animator.StringToHash("InUpperBodyAddativeA");
		moveBackwardsIndex = Animator.StringToHash("MoveBackwards");

		if (gameCharacter.Animator.runtimeAnimatorController is AnimatorOverrideController)
		{
			overrideController = (AnimatorOverrideController)gameCharacter.Animator.runtimeAnimatorController;
		}
		else
		{
			overrideController = new AnimatorOverrideController(gameCharacter.Animator.runtimeAnimatorController);
		}

		
		minMalkSpeed = gameCharacter.GameCharacterData.MaxMovementSpeed * gameCharacter.GameCharacterData.WalkFactor;

		addativeLayerInterpTarget = AddativeLayerWeight;
		addativeLayerInterpSpeed = 5f;

		AttackTriggerTimer.onTimerFinished += OnAttackTriggerTimerFinished;
	}

	public void Update(float deltaTime)
	{
		AttackTriggerTimer.Update(deltaTime);

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

		SpineLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(spineLayerIndex), gameCharacter.spineLayerDebugFloat.useDebugValue ? gameCharacter.spineLayerDebugFloat.debugFloat : spineLayerInterpTarget, deltaTime * spineLayerInterpSpeed);
		LegLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(legLayerIndex), gameCharacter.legLayerDebugFloat.useDebugValue ? gameCharacter.legLayerDebugFloat.debugFloat : legLayerInterpTarget, deltaTime * legLayerInterpSpeed);
		HeadLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(headLayerIndex), gameCharacter.headLayerDebugFloat.useDebugValue ? gameCharacter.headLayerDebugFloat.debugFloat :  headLayerInterpTarget, deltaTime * headLayerInterpSpeed);
		ArmRLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(armRLayerIndex), gameCharacter.armRLayerDebugFloat.useDebugValue ? gameCharacter.armRLayerDebugFloat.debugFloat : armRLayerInterpTarget, deltaTime * armRLayerInterpSpeed);
		ArmLLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(armLLayerIndex), gameCharacter.armLLayerDebugFloat.useDebugValue ? gameCharacter.armLLayerDebugFloat.debugFloat : armLLayerInterpTarget, deltaTime * armLLayerInterpSpeed);
		SecondaryMotionLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(secondaryMotionLayerIndex), gameCharacter.secondaryLayerDebugFloat.useDebugValue ? gameCharacter.secondaryLayerDebugFloat.debugFloat : secondaryMotionLayerInterpTarget, deltaTime * secondaryMotionLayerInterpSpeed);
		UpperBodyLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(upperBodyLayerIndex), gameCharacter.upperBodyLayerDebugFloat.useDebugValue ? gameCharacter.upperBodyLayerDebugFloat.debugFloat : upperBodyLayerInterpTarget, deltaTime * upperBodyLayerInterpSpeed);
		AddativeLayerWeight = Mathf.Lerp(gameCharacter.Animator.GetLayerWeight(addativeLayerIndex), gameCharacter.addativeLayerDebugFloat.useDebugValue ? gameCharacter.addativeLayerDebugFloat.debugFloat : addativeLayerInterpTarget, deltaTime * addativeLayerInterpSpeed);
		ChestCorrectionIK = Mathf.Lerp(ChestCorrectionIK, gameCharacter.chestCorrectionDebugFloat.useDebugValue ? gameCharacter.chestCorrectionDebugFloat.debugFloat : ChestCorrectionIKTarget, deltaTime * chestCorrectionTargetInterpSpeed);
		ForcePositionFootIKR = Mathf.Lerp(ForcePositionFootIKR, gameCharacter.footIKRCorrectionDebugFloat.useDebugValue ? gameCharacter.footIKRCorrectionDebugFloat.debugFloat : ForcePositionFootIKRTarget, deltaTime * forcePositionFootIKRTargetInterpSpeed);
		ForcePositionFootIKL = Mathf.Lerp(ForcePositionFootIKL, gameCharacter.footIKLCorrectionDebugFloat.useDebugValue ? gameCharacter.footIKLCorrectionDebugFloat.debugFloat : ForcePositionFootIKLTarget, deltaTime * forcePositionFootIKLTargetInterpSpeed);
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
		float movementSpeed = gameCharacter.MovementComponent.MovementVelocity.magnitude;
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
		}
		else
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
		}
		else
		{
			overrideController["AttackA"] = attackClip;
		}
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
		AttackA = !isAttackAState;
	}

	public void ApplyBlendTree(AimBlendAnimations anims)
	{
		bool isBlendAState = IsInState(inAimBlendTreeAStateIndex, upperBodyLayerIndex);
		if (isBlendAState)
		{
			if (overrideController["AimBlendSpaceMidA"].GetInstanceID() == anims.midAnimation.GetInstanceID()) return;

			overrideController["AimBlendSpaceUpB"] = anims.upAnimation;
			overrideController["AimBlendSpaceMidB"] = anims.midAnimation;
			overrideController["AimBlendSpaceDownB"] = anims.downAnimation;
		}
		else
		{
			if (overrideController["AimBlendSpaceMidB"].GetInstanceID() == anims.midAnimation.GetInstanceID()) return;

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
	public void ApplyAddativeAnimationToAddativeState(AnimationClip addativeAnimation)
	{
		bool inAddativeAState = IsInState(additiveAStateIndex, addativeLayerIndex);
		if (inAddativeAState)
		{
			overrideController["AddativeB"] = addativeAnimation;
		}
		else
		{
			overrideController["AddativeA"] = addativeAnimation;
		}
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
		InAddativeAState = !inAddativeAState;
	}

	public void ApplyUpperBodyAddativeAnimationToState(AnimationClip addativeAnimation)
	{
		bool inUpperBodyAddativeAState = IsInState(upperBodyAddativeAStateIndex, upperBodyAddativeLayerIndex);
		Ultra.Utilities.Instance.DebugLogOnScreen("In Upper Body A State:" + inUpperBodyAddativeAState.ToString(), 1f, StringColor.Darkblue);
		if (inUpperBodyAddativeAState)
		{
			overrideController["UpperBodyAddativeB"] = addativeAnimation;
		}
		else
		{
			overrideController["UpperBodyAddativeA"] = addativeAnimation;
		}
		gameCharacter.Animator.runtimeAnimatorController = overrideController;
		InUpperBodyAddativeAState = !inUpperBodyAddativeAState;
	}

	private bool IsInState(int stateIndex, int layerIndex = 0)
	{
		AnimatorStateInfo animatorState = gameCharacter.Animator.GetCurrentAnimatorStateInfo(layerIndex);
		bool state = false;

		if (animatorState.shortNameHash == stateIndex) state = true;
		return state;
	}

	public bool IsInValidAttackState()
	{
		bool isAttackAState = IsInState(attackAStateIndex);
		bool isAttackBState = IsInState(attackBStateIndex);

		return isAttackAState || isAttackBState;
	}
	public bool IsInValid3BlendAttackState()
	{
		bool isAttackAState = IsInState(combat3BlendTreeAStateIndex);
		bool isAttackBState = IsInState(combat3BlendTreeBStateIndex);

		return isAttackAState || isAttackBState;
	}
	public bool IsInValid3BlendAimState()
	{
		bool isAttackAState = IsInState(inAimBlendTreeAStateIndex, upperBodyLayerIndex);
		bool isAttackBState = IsInState(inAimBlendTreeBStateIndex, upperBodyLayerIndex);

		return isAttackAState || isAttackBState;
	}
	public bool IsInValidAttackHoldState()
	{
		return IsInState(attackHoldStateIndex);
	}
	public bool IsInValidAttackTriggerState()
	{
		return IsInState(attackTriggerStateIndex);
	}
	//
	//public bool IsValidAttackAnimationData(bool isAState)
	//{
	//	var infos = gameCharacter.Animator.GetCurrentAnimatorClipInfo(0);
	//	if (infos == null) return false;
	//	if (infos[0].clip == null) return false;
	//	Dictionary<string, AnimationClip> runtimeAnimations = GetAnimationsFromController(defaultController);
	//	Dictionary<string, AnimationClip> overrideAnimations = GetAnimationsFromController(overrideController.runtimeAnimatorController as RuntimeAnimatorController);
	//
	//	string stateNameToCompare = isAState ? "AttackA" : "AttackB";
	//
	//
	//	if (runtimeAnimations.ContainsKey(stateNameToCompare) && overrideAnimations.ContainsKey(stateNameToCompare))
	//	{
	//		AnimationClip runtimeClip = runtimeAnimations[stateNameToCompare];
	//		AnimationClip overrideClip = overrideAnimations[stateNameToCompare];
	//	}
	//
	//}

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
	public void InterpAddativeLayerWeight(float target, float speed = 5f)
	{
		addativeLayerInterpTarget = target;
		addativeLayerInterpSpeed = speed;
	}
	public void SetAddativeLayerWeight(float weight)
	{
		addativeLayerInterpTarget = weight;
		AddativeLayerWeight = weight;
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
			// Check if we already in correct Body Anim. Check Needs to be reverse B Check A && A Checks B
			// Needed for unesesary Interpolations between anims States
			if (overrideController["BodyLayerA"].GetInstanceID() == clip.GetInstanceID())
				return;
			overrideController["BodyLayerB"] = clip;
		}
		else
		{
			// Check if we already in correct Body Anim. Check Needs to be reverse B Check A && A Checks B
			// Needed for unesesary Interpolations between anims States
			if (overrideController["BodyLayerB"].GetInstanceID() == clip.GetInstanceID())
				return;
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

	void OnAttackTriggerTimerFinished()
	{
		TriggerAttack = false;
	}

	public void CrossFadeToNextState(int nextStateHash, float transitionDuration, EGameCharacterAnimatiorLayers layerType = EGameCharacterAnimatiorLayers.BasisLayer)
	{
		int layer = GetLayerIndex(layerType);
		gameCharacter.Animator.CrossFade(nextStateHash, transitionDuration, layer);
	}

	public int GetLayerIndex(EGameCharacterAnimatiorLayers layer)
	{
		return (int)layer;
		// Was i high? wtf is this!
		//switch (layer)
		//{
		//	case EGameCharacterAnimatiorLayers.BasisLayer: return 0;
		//	case EGameCharacterAnimatiorLayers.SpineLayer: return 1;
		//	case EGameCharacterAnimatiorLayers.LegLayer: return 2;
		//	case EGameCharacterAnimatiorLayers.HeadLayer: return 3;
		//	case EGameCharacterAnimatiorLayers.ArmRLayer: return 4;
		//	case EGameCharacterAnimatiorLayers.ArmLLayer: return 5;
		//	case EGameCharacterAnimatiorLayers.UpperBodyLayer: return 6;
		//	case EGameCharacterAnimatiorLayers.RotationLayer: return 7;
		//	case EGameCharacterAnimatiorLayers.UpperBodyAddativeLayer: return 8;
		//	case EGameCharacterAnimatiorLayers.SecondaryMotionLayer: return 9;
		//	case EGameCharacterAnimatiorLayers.HitLayer: return 10;
		//	default:
		//		Ultra.Utilities.Instance.DebugErrorString("AnimationController", "GetLayerIndex", "Layer was not defined!");
		//		return -1;
		//}
	}

	public void ResetAnimStatesHARD()
	{
		gameCharacter.AnimController.InAttack = false;
		gameCharacter.AnimController.InAimBlendTree = false;
		gameCharacter.AnimController.InFreez = false;
		gameCharacter.AnimController.HoldAttack = false;
		gameCharacter.AnimController.TriggerAttack = false;
		gameCharacter.AnimController.InDefensiveAction = false;
		gameCharacter.AnimController.InCombat3Blend = false;
		gameCharacter.AnimController.FlyAway = false;
	}

	public void ResetAnimIK_HARD()
	{
		SetChestCorrectionWeight(0);
		SetFPFootIKLWeight(0);
		SetFPFootIKRWeight(0);
	}

	public void ForceStandingState()
	{
		gameCharacter.Animator.CrossFade(StandingStateHash, 0f);
	}

	public void InterpChectCorrectionWeight(float target, float speed = 5f)
	{
		chestCorrectionIKTarget = target;
		chestCorrectionTargetInterpSpeed = speed;
	}
	public void SetChestCorrectionWeight(float weight)
	{
		chestCorrectionIKTarget = weight;
		ChestCorrectionIK = weight;
	}
	public void InterpFPFootIKRWeight(float target, float speed = 5f)
	{
		forcePositionFootIKRTarget = target;
		forcePositionFootIKRTargetInterpSpeed = speed;
	}
	public void SetFPFootIKRWeight(float weight)
	{
		forcePositionFootIKRTarget = weight;
		ForcePositionFootIKR = weight;
	}
	public void InterpFPFootIKLWeight(float target, float speed = 5f)
	{
		forcePositionFootIKLTarget = target;
		forcePositionFootIKLTargetInterpSpeed = speed;
	}
	public void SetFPFootIKLWeight(float weight)
	{
		forcePositionFootIKLTarget = weight;
		ForcePositionFootIKL = weight;
	}
}
