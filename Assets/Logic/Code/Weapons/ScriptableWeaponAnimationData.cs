using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum EWeaponHandType
{
	NoHands,
	RightHand,
	LeftHand,
	Both,
}

[Serializable]
public class AttackAnimationData
{
	public AnimationClip clip;
	public AttackAnimationHitDetectionData data;
	public AnimationClip holdAnimation;
	public AnimationClip triggerAnimation;
	public float maxVerticalMovement;
	public List<GameObject> particleList;
}

public enum EHitDetectionType
{
	Mesh,
	Sphere,
	Box,
	Capsul,
}

[Serializable]
public class AttackAnimationHitDetectionData
{
	public bool showProperties = true;
	public EHitDetectionType hitDetectionType;
	public Mesh mesh = null;
	public Vector3 offset = Vector3.zero;
	public Vector3 scale = Vector3.one;
	public float radius = 1f;
	public Vector3 boxDimensions = Vector3.one;
	public float capsulHeight = 1f;
}

[Serializable]
public class testlol
{
	public AttackAnimationHitDetectionData data;
}

[CreateAssetMenu(fileName = "New WeaponAnimationData", menuName = "Assets/Weapons/WeaponAnimationData")]
public class ScriptableWeaponAnimationData : ScriptableObject
{
	public List<AttackAnimationData> GroundAttacks;
	public List<AttackAnimationData> GroundUpAttacks;
	public List<AttackAnimationData> GroundDownAttacks;
	public List<AttackAnimationData> GroundDirectionAttacks;

	public List<AttackAnimationData> AirAttacks;
	public List<AttackAnimationData> AirUpAttacks;
	public List<AttackAnimationData> AirDownAttacks;
	public List<AttackAnimationData> AirDirectionAttacks;

	public List<AttackAnimationData> DefensiveAction;

	public EWeaponHandType HandType;
	public AnimationClip WeaponReadyPose;
	[Range(0f,1f)]
	public float WeaponReadyWeight = 1f;
	public float WeaponReadyInterpSpeed = 5f;
	[Range(0f, 1f)]
	public float HeadSpineLayerMovingWeight = 0.5f;
	[Range(0f, 1f)]
	public float ArmRMovingWeight = 0.5f;
	[Range(0f, 1f)]
	public float ArmLMovingWeight = 0.5f;
}
