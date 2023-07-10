using System.Collections;
using System.Collections.Generic;
using Ultra;
using UnityEditor;
using UnityEngine;

namespace Ultra
{
	public class HypoUttilies : Singelton<HypoUttilies>
	{
		public static List<ScriptableCharacter> GetAllCharacters()
		{
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
		}
	}
}
