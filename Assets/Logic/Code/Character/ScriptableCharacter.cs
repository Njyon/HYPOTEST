using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Assets/Character")]
public class ScriptableCharacter : ScriptableObject
{
	public string Name = "New Character";
	public GameObject CharacterPrefab;
	[HideInInspector] public string ControllerName;
	public CharacterAnimationData CharacterAnimationData;
}
