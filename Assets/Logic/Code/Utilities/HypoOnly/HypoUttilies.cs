using System.Collections;
using System.Collections.Generic;
using Ultra;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ultra
{
	public class HypoUttilies : Singelton<HypoUttilies>
	{
		public static List<ScriptableCharacter> GetAllCharacters()
		{
#if UNITY_EDITOR

			List<ScriptableCharacter> dataList = new List<ScriptableCharacter>();
			string[] guids = AssetDatabase.FindAssets("t:" + typeof(ScriptableCharacter).Name);
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				ScriptableCharacter data = AssetDatabase.LoadAssetAtPath<ScriptableCharacter>(path);

				if (data != null)
				{
					dataList.Add(data);
				}
			}
			return dataList;
#endif
			return new List<ScriptableCharacter>();
		}
		public static GameCharacter FindCharactereNearestToDirection(Vector3 fromPosition, Vector3 direction, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = list[0];
			float smallestAngle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (mostPointingObject.transform.position - fromPosition).normalized);

			foreach (GameCharacter character in list)
			{
				float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.transform.position - fromPosition).normalized);
				if (angle < smallestAngle)
				{
					mostPointingObject = character;
					smallestAngle = angle;
				}
			}

			return mostPointingObject;
		}
		public static GameCharacter FindCHaracterNearestToDirectionWithMinAngel(Vector3 fromPosition, Vector3 direction, float minAngelAngle, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = minAngelAngle;

			foreach (GameCharacter character in list)
			{
				float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.transform.position - fromPosition).normalized);
				if (angle < smallestAngle)
				{
					mostPointingObject = character;
					smallestAngle = angle;
				}
			}

			return mostPointingObject;
		}
	}
}
