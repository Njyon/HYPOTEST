using System;
using System.Collections;
using System.Collections.Generic;
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
				actionCopie = action.instance.CreateCopy();
			}
			return actionCopie;
		}
	}
	public AttackAnimationHitDetectionData data;
	public SerializableDictionary<EExplicitAttackType, int> combatBranches;
	public SerializableDictionary<EExplicitAttackType, int> timedCombatBrenches;
	public List<GameObject> particleList;
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

	//bool isValidtating = false;

	public void OnValidate()
	{
//#if UNITY_EDITOR
//		if (EditorApplication.isUpdating) return;
//#endif
//		if (isValidtating) return;
//		isValidtating = true;
//		foreach (AttackAnimationData AnimData in GroundAttacks)
//		{
//			AnimData.attackDataHolder.SetAttackRef();
//		}
//		foreach (AttackAnimationData AnimData in GroundUpAttacks)
//		{
//			AnimData.attackDataHolder.SetAttackRef();
//		}
//		foreach (AttackAnimationData AnimData in GroundDownAttacks)
//		{
//			AnimData.attackDataHolder.SetAttackRef();
//		}
//		foreach (AttackAnimationData AnimData in GroundDirectionAttacks)
//		{
//			AnimData.attackDataHolder.SetAttackRef();
//		}
//		
//		foreach (AttackAnimationData AnimData in AirAttacks)
//		{
//			AnimData.attackDataHolder.SetAttackRef();
//		}
//		foreach (AttackAnimationData AnimData in AirUpAttacks)
//		{
//			AnimData.attackDataHolder.SetAttackRef();
//		}
//		foreach (AttackAnimationData AnimData in AirDownAttacks)
//		{
//			AnimData.attackDataHolder.SetAttackRef();
//		}
//		foreach (AttackAnimationData AnimData in AirDirectionAttacks)
//		{
//			AnimData.attackDataHolder.SetAttackRef();
//		}
//		
//		foreach (AttackAnimationData AnimData in DefensiveAction)
//		{
//			AnimData.attackDataHolder.SetAttackRef();
//		}
//
//#if UNITY_EDITOR
//		EditorUtility.SetDirty(this);
//		if (!EditorApplication.isUpdating)
//		{
//			//AssetDatabase.SaveAssetIfDirty(this);
//		} 
//#endif
//		isValidtating = false;
	}
}
