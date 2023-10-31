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
	private ActionBase actionCopie;

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

		copy.action.instance = action.instance;
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

	public ScriptableWeaponAnimationData Copy()
	{
		ScriptableWeaponAnimationData copy = new ScriptableWeaponAnimationData();

		copy.GroundAttacks = GroundAttacks;
		CopyDataInList(ref copy.GroundAttacks, ref GroundAttacks);
		copy.GroundUpAttacks = GroundUpAttacks;
		CopyDataInList(ref copy.GroundUpAttacks, ref GroundUpAttacks);
		copy.GroundDownAttacks = GroundDownAttacks;
		CopyDataInList(ref copy.GroundDownAttacks, ref GroundDownAttacks);
		copy.GroundDirectionAttacks = GroundDirectionAttacks;
		CopyDataInList(ref copy.GroundDirectionAttacks, ref GroundDirectionAttacks);

		copy.AirAttacks = AirAttacks;
		CopyDataInList(ref copy.AirAttacks, ref AirAttacks);
		copy.AirUpAttacks = AirUpAttacks;
		CopyDataInList(ref copy.AirUpAttacks, ref AirUpAttacks);
		copy.AirDownAttacks = AirDownAttacks;
		CopyDataInList(ref copy.AirDownAttacks, ref AirDownAttacks);
		copy.AirDirectionAttacks = AirDirectionAttacks;
		CopyDataInList(ref copy.AirDirectionAttacks, ref AirDirectionAttacks);

		copy.DefensiveAction = DefensiveAction;
		CopyDataInList(ref copy.DefensiveAction, ref DefensiveAction);

		copy.HandType = HandType;
		copy.WeaponReadyPose = WeaponReadyPose;
		copy.WeaponReadyWeight = WeaponReadyWeight;
		copy.WeaponReadyInterpSpeed = WeaponReadyInterpSpeed;
		copy.HeadSpineLayerMovingWeight = HeadSpineLayerMovingWeight;
		copy.ArmRMovingWeight = ArmRMovingWeight;
		copy.ArmLMovingWeight = ArmLMovingWeight;

		return copy;
	}

	void CopyDataInList(ref List<AttackAnimationData> copy, ref List<AttackAnimationData> origin)
	{
		for (int i = 0; i < origin.Count; i++)
		{
			copy[i] = origin[i].Copy();
		}
	}
}
