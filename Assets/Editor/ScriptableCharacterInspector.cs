using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;

[CustomEditor(typeof(ScriptableCharacter))]
public class ScriptableCharacterInspector : Editor
{
	int index = 0;
	int currentIndex = 0;
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		ScriptableCharacter characterData = (ScriptableCharacter)target;


		ControllerBase[] controllers = Ultra.Utilities.GetAll<ControllerBase>().ToArray();
		if (characterData.ControllerName != null)
		{
			for (int i = 0; i < controllers.Length; i++)
			{
				if (controllers[i].GetType().Name == characterData.ControllerName) currentIndex = i;
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

		index = EditorGUILayout.Popup("Label", currentIndex, controllerNames);
		if (index != currentIndex)
		{
			currentIndex = index;
			characterData.ControllerName = controllerNames[currentIndex];

			EditorUtility.SetDirty(characterData);
			AssetDatabase.SaveAssetIfDirty(characterData);
		}
	}
}
