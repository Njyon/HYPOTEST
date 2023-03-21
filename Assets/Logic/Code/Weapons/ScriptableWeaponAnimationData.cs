using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponAnimationData", menuName = "Assets/Weapons/WeaponAnimationData")]
public class ScriptableWeaponAnimationData : ScriptableObject
{
	public List<AnimationClip> GroundAttacks;
	public List<AnimationClip> GroundUpAttacks;
	public List<AnimationClip> GroundDownAttacks;
	public List<AnimationClip> GroundDirectionAttacks;

	public List<AnimationClip> AirAttacks;
	public List<AnimationClip> AirUpAttacks;
	public List<AnimationClip> AirDownAttacks;
	public List<AnimationClip> AirDirectionAttacks;

	public AnimationClip WeaponReadyPose;
	[Range(0f,1f)]
	public float WeaponReadyWeight = 1f;
	public float WeaponReadyInterpSpeed = 5f;
}
