using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterAnimationData", menuName = "Assets/CharacterAnimationData")]
public class CharacterAnimationData : ScriptableObject
{
	public List<AnimationClip> Jumps;

}
