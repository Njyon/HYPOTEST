using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(ScriptableWeapon))]
public class ScriptableWeaponInspector : Editor
{
	int index = 0;
	int currentIndex = 0;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		ScriptableWeapon weaponData = (ScriptableWeapon)target;
		WeaponBase[] controllers = Ultra.Utilities.GetAll<WeaponBase>().ToArray();
		if (weaponData.WeaponClassName != null)
		{
			for (int i = 0; i < controllers.Length; i++)
			{
				if (controllers[i].GetType().Name == weaponData.WeaponClassName) currentIndex = i;
			}
		}
		else
		{
			currentIndex = 0;
		}
		string[] controllerNames = new string[controllers.Length];
		for (int i = 0; i < controllers.Length; i++)
		{
			controllerNames[i] = controllers[i].GetType().Name;
		}

		index = EditorGUILayout.Popup("WeaponClass", currentIndex, controllerNames);
		if (index != currentIndex)
		{
			currentIndex = index;
			weaponData.WeaponClassName = controllerNames[currentIndex];

			EditorUtility.SetDirty(weaponData);
			AssetDatabase.SaveAssetIfDirty(weaponData);
		}
	}
}
