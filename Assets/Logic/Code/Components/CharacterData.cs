using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

	[Header("JumpData")]
	[SerializeField] int maxJumps = 3;
	[SerializeField] float jumpForce = 100f;
	[SerializeField] float gravityMultiplier = 1f;

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

	[Header("Health")]
	[SerializeField] float health = 100f;

	[Header("Bones")]
	[SerializeField] Transform root;

	[Header("Combat")]
	[SerializeField] float characterDetectionRange = 30f;

	public float MaxMovementSpeed { get { return maxMovementSpeed; } }
	public float MovmentGravity { get { return movmentGravity; } }
	public float Acceleration { get { return acceleration; } }	
	public float Drag { get { return drag; } }	
	public float InAirControll { get { return inAirControll; } }
	public float SlopStrenghIncrease {  get { return slopStrenghIncrease; } }
	public float SlopStrenghDecrease { get { return slopStrenghDecrease; } }
	public float MaxSlopAngle { get { return maxSlopAngle; } }
	public float RoationSpeed { get { return roationSpeed; } }

	public int MaxJumps { get { return maxJumps; } }
	public float JumpForce { get { return jumpForce; } }
	public float GravityMultiplier { get { return gravityMultiplier; } }

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

	public float Health { get { return health; } }

	public Transform Root { get { return root; } }

	public float CharacterDetectionRange { get { return characterDetectionRange; } }
}
