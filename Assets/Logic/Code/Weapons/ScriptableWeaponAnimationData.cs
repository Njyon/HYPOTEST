using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
	public ClassInstance<ActionBase> action = new ClassInstance<ActionBase>();
	private ActionBase actionCopie = null;

	public ActionBase Action
	{
		get
		{
			if (actionCopie == null)
			{
				actionCopie = action?.instance?.CreateCopy();
			}
			return actionCopie;
		}
	}
	public AttackAnimationHitDetectionData data;
	public SerializableDictionary<EExplicitAttackType, int> combatBranches;
	public SerializableDictionary<EExplicitAttackType, int> timedCombatBrenches;
	public List<GameObject> particleList;

	public AttackAnimationData Copy()
	{
		var copy = new AttackAnimationData();

		copy.action = action;
		copy.actionCopie = null;
		copy.data = data;
		copy.combatBranches = combatBranches;
		copy.timedCombatBrenches = timedCombatBrenches;
		copy.particleList = particleList;

		return copy;
	}
}

[Serializable]
public class AttackAnimationExtraData
{
	AttackAnimationExtraData()
	{
		Damage = 10f;
		Rating = 10f;
		discharge = 100f;
		flyAwayTime = 1f;
		freezTime = 1f;
		stopMovingRange = 1f;
	}

	public float Damage = 10;
	public float Rating = 10;
	public float discharge = 100;
	public float maxVerticalMovement;
	public float maxHorizontalMovement;
	public Vector3 flyAwayDirection;
	public float flyAwayStrengh;
	public float rangeValue;
	public float speedValue;
	public float timeValue;
	public float flyAwayTime = 1f;
	public float freezTime = 1f;
	public float stopMovingRange = 1f;
}

[Serializable]
public class AimBlendTypes
{
	public AimBlendAnimations blendAnimations;
	public AimBlendAnimations blendHoldAnimations;
	public AimBlendAnimations blendTriggerAnimations;
}

[Serializable]
public class AimBlendAnimations
{
	public AnimationClip upAnimation;
	public AnimationClip midAnimation;
	public AnimationClip downAnimation;
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

	public void Copy(ScriptableWeaponAnimationData origin)
	{
		GroundAttacks = new List<AttackAnimationData>();
		GroundUpAttacks = new List<AttackAnimationData>();
		GroundDownAttacks = new List<AttackAnimationData>();
		GroundDirectionAttacks = new List<AttackAnimationData>();

		AirAttacks = new List<AttackAnimationData>();
		AirUpAttacks = new List<AttackAnimationData>();
		AirDownAttacks = new List<AttackAnimationData>();
		AirDirectionAttacks = new List<AttackAnimationData>();

		DefensiveAction = new List<AttackAnimationData>();

		CopyDataInList(ref GroundAttacks, ref origin.GroundAttacks);
		CopyDataInList(ref GroundUpAttacks, ref origin.GroundUpAttacks);
		CopyDataInList(ref GroundDownAttacks, ref origin.GroundDownAttacks);
		CopyDataInList(ref GroundDirectionAttacks, ref origin.GroundDirectionAttacks);

		CopyDataInList(ref AirAttacks, ref origin.AirAttacks);
		CopyDataInList(ref AirUpAttacks, ref origin.AirUpAttacks);
		CopyDataInList(ref AirDownAttacks, ref origin.AirDownAttacks);
		CopyDataInList(ref AirDirectionAttacks, ref origin.AirDirectionAttacks);

		CopyDataInList(ref DefensiveAction, ref origin.DefensiveAction);

		HandType = origin.HandType;
		WeaponReadyPose = origin.WeaponReadyPose;
		WeaponReadyWeight = origin.WeaponReadyWeight;
		WeaponReadyInterpSpeed = origin.WeaponReadyInterpSpeed;
		HeadSpineLayerMovingWeight = origin.HeadSpineLayerMovingWeight;
		ArmRMovingWeight = origin.ArmRMovingWeight;
		ArmLMovingWeight = origin.ArmLMovingWeight;
	}

	void CopyDataInList(ref List<AttackAnimationData> copy, ref List<AttackAnimationData> origin)
	{
		for (int i = 0; i < origin.Count; i++)
		{
			copy.Add(origin[i].Copy());
		}
	}
}
