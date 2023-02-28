using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Database", menuName = "Assets/Character Database")]
public class CharacterDataBase : ScriptableObject
{
	public List<ScriptableCharacter> characters;
}
