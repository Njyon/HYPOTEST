using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum RigType
{
	Unknown,
	Dummy,
}

public enum EPhysicsMaterial
{
	Unknown,
	Metal,
	Stone,
}

public class GameCharacterData : MonoBehaviour
{
	[Header("MovementData")]
	[SerializeField] float maxMovementSpeed = 5f;
	[SerializeField] float movmentGravity = 9.81f;
	[SerializeField] float acceleration = 5f;
	[SerializeField] float drag = 1f;
	[SerializeField] float inAirControll = 1f;
	[SerializeField] float slopStrenghIncrease = 1f;
	[SerializeField] float slopStrenghDecrease = 1f;
	[SerializeField] float maxSlopAngle = 75f;
	[SerializeField] float roationSpeed = 5f;
	[SerializeField] AnimationCurve gravityInterpCurve;
	[SerializeField] float gravityMultiplierInAttack = 0.2f;
	[SerializeField] float maxGravityPull = 20f;

	[Header("JumpData")]
	[SerializeField] int maxJumps = 3;
	[SerializeField] float jumpForce = 100f;
	[SerializeField] float gravityMultiplier = 1f;
	[SerializeField] EnemyStepColliderScript enemyStepColliderScript;

	[Header("Sliding")]
	[SerializeField] float maxSlidingSpeed = 10f;
	[SerializeField] float slidingAcceleration = 5f;

	[Header("AnimationData")]
	[SerializeField] AnimationCurve strideBlendWalk;
	[SerializeField] AnimationCurve strideBlendRun;
	[SerializeField] float walkFactor = 0.2f;
	[SerializeField] float walkRunInterp = 1f;
	[SerializeField] float strideInterp = 1f;
	[SerializeField] float playbackInterp = 1f;
	[SerializeField] float roationBlendInterp = 1f;
	[SerializeField] float maxFallingVelocityAnim = 200f;

	[Header("WeaponData")]
	[SerializeField] List<ScriptableWeapon> weapons;
	[SerializeField] Transform handRObjectPoint;
	[SerializeField] Transform handLObjectPoint;
	[SerializeField] float airToZeroVelYInAttackSpeed = 5f;
	[SerializeField] float airToZeroVelXInAttackSpeed = 5f;
	[SerializeField] float groundToZeroVelYInAttackSpeed = 5f;
	[SerializeField] float groundToZeroVelXInAttackSpeed = 5f;
	[SerializeField] AnimationCurve gravitationOverTime;

	[Header("Health")]
	[SerializeField] float health = 100f;

	[Header("Stagger")]
	[SerializeField] float maxStaggerValue;
	[SerializeField] float staggerTime = 3f;

	[Header("Bones")]
	[SerializeField] Transform root;
	[SerializeField] string headBoneName = "head.x";
	[SerializeField] string rootBoneName = "root";
	[SerializeField] Rig baseRig;
	[SerializeField] ChainIKConstraint chestCorrectionIK;
	[SerializeField] TwoBoneIKConstraint forcePosition_FootIK_R;
	[SerializeField] TwoBoneIKConstraint forcePosition_FootIK_L;

	[Header("Combat")]
	[SerializeField] float characterDetectionRange = 30f;
	[SerializeField] int combatAttackListLenght = 10;
	[SerializeField] float iFramesTime = 0.2f;
	[SerializeField] ParticleSystem succsessfullDodgeParticleEffect;
	[SerializeField] ParticleSystem dodgeParticleEffect;

	[Header("Data")]
	[SerializeField] SkinnedMeshRenderer meshRenderer;

	[Header("Rig")]
	[SerializeField] RigType rigType = RigType.Unknown;

	[Header("Enemy")]
	[SerializeField] Vector3 enemyInfoOffset = new Vector3(0, 5f, 0);
	[SerializeField] float minCharacterDistance = 1f;

	[Header("Feedback")]
	[SerializeField] Vector3 attackFeedbackOffset = Vector3.zero;

	[Header("Dodge")]
	[SerializeField] int maxDodgeAmount = 3;
	[SerializeField] float dodgeDistance = 10f;
	[SerializeField] float dodgeSpeed = 10f;
	[SerializeField] float dodgeRecoveryTime = 1f;

	[Header("SoundEffects")]
	public BetterSerializableDictionary<EPhysicsMaterial, SoundEffectWrapper> FootSoundEffects = new();
	public List<SoundEffect> DodgeSoundEffects = new List<SoundEffect>();

	public float MaxMovementSpeed { get { return maxMovementSpeed; } }
	public float MovmentGravity { get { return movmentGravity; } }
	public float Acceleration { get { return acceleration; } }
	public float Drag { get { return drag; } }
	public float InAirControll { get { return inAirControll; } }
	public float SlopStrenghIncrease { get { return slopStrenghIncrease; } }
	public float SlopStrenghDecrease { get { return slopStrenghDecrease; } }
	public float MaxSlopAngle { get { return maxSlopAngle; } }
	public float RoationSpeed { get { return roationSpeed; } }
	public AnimationCurve GravityInterpCurve { get { return gravityInterpCurve; } }
	public float GravityMultiplierInAttack { get { return gravityMultiplierInAttack; } }
	public float MaxGravityPull { get { return maxGravityPull; } }

	public int MaxJumps { get { return maxJumps; } }
	public float JumpForce { get { return jumpForce; } }
	public float GravityMultiplier { get { return gravityMultiplier; } }
	public EnemyStepColliderScript EnemyStepColliderScript { get { return enemyStepColliderScript; } }

	public float MaxSlidingSpeed { get { return maxSlidingSpeed; } }
	public float SlidingAcceleration { get { return slidingAcceleration; } }

	public AnimationCurve StrideBlendWalk { get { return strideBlendWalk; } }
	public AnimationCurve StrideBlendRun { get { return strideBlendRun; } }
	public float WalkFactor { get { return walkFactor; } }
	public float WalkRunInterp { get { return walkRunInterp; } }
	public float StrideInterp { get { return strideInterp; } }
	public float PlaybackInterp { get { return playbackInterp; } }
	public float RoationBlendInterp { get { return roationBlendInterp; } }
	public float MaxFallingVelocityAnim { get { return maxFallingVelocityAnim; } }

	public List<ScriptableWeapon> Weapons { get { return weapons; } }
	public Transform HandROnjectPoint { get { return handRObjectPoint; } }
	public Transform HandLOnjectPoint { get { return handLObjectPoint; } }
	public float AirToZeroVelYInAttackSpeed { get { return airToZeroVelYInAttackSpeed; } }
	public float AirToZeroVelXInAttackSpeed { get { return airToZeroVelXInAttackSpeed; } }
	public float GroundToZeroVelYInAttackSpeed { get { return groundToZeroVelYInAttackSpeed; } }
	public float GroundToZeroVelXInAttackSpeed { get { return groundToZeroVelXInAttackSpeed; } }
	public AnimationCurve GravitationOverTime { get { return  gravitationOverTime; } }

	public float Health { get { return health; } }

	public float MaxStaggerValue { get { return maxStaggerValue; } }
	public float StaggerTime { get { return staggerTime; } }

	public Transform Root { get { return root; } }
	public string HeadBoneName { get { return headBoneName; } }
	public string RootBoneName { get { return rootBoneName; } }

	public Rig BaseRig { get { return baseRig; } }
	public ChainIKConstraint ChestCorrectionIK { get { return chestCorrectionIK; } }
	public TwoBoneIKConstraint ForcePosition_FootIK_R { get { return forcePosition_FootIK_R; } }
	public TwoBoneIKConstraint ForcePosition_FootIK_L { get { return forcePosition_FootIK_L; } }

	public float CharacterDetectionRange { get { return characterDetectionRange; } }
	public int CombatAttackListLenght { get { return combatAttackListLenght; } }
	public float IFrameTime { get { return iFramesTime; } }
	public ParticleSystem SuccsessfullDodgeParticleEffect { get { return succsessfullDodgeParticleEffect; } }
	public ParticleSystem DodgeParticleEffect { get { return dodgeParticleEffect; } }

	public SkinnedMeshRenderer MeshRenderer { get { return meshRenderer; } }

	public RigType RigType { get { return rigType; } }

	public Vector3 EnemyInfoOffset { get { return enemyInfoOffset; } }
	public float MinCharacterDistance { get { return minCharacterDistance; } }

	public Vector3 AttackFeedbackOffset { get { return attackFeedbackOffset; } }

	public int MaxDodgeAmount { get { return maxDodgeAmount; } }
	public float DodgeDistance { get { return dodgeDistance; } }
	public float DodgeSpeed { get { return dodgeSpeed; } }
	public float DodgeRecoveryTime { get { return dodgeRecoveryTime; } }
}

[Serializable]
public class SoundEffectWrapper
{
	public List<SoundEffect> SoundEffects = new();
}
