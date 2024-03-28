using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BetterSerializableCharacterDictionary<,>))]
public class BetterCharacterDictionaryPropertyDrawer : SerializableDictionaryDrawer
{
	string[] characterNameArray;
	private bool isInitialized = false;
	int[] characterNameIndexe;
	List<string> stringList;
	SerializedProperty keysProperty;

	void HeavyShit()
	{
		List<ScriptableCharacter> characters = Ultra.HypoUttilies.GetAllCharacters();
		characterNameArray = new string[characters.Count + 1];
		characterNameArray[0] = "Unknown";
		for (int i = 0; i < characters.Count; i++)
		{
			characterNameArray[i + 1] = characters[i].Name;
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (!isInitialized)
		{
			isInitialized = true;
			HeavyShit();
		}

		keysProperty = property.FindPropertyRelative("keys");
		characterNameIndexe = new int[keysProperty.arraySize];
		stringList = new List<string>(keysProperty.arraySize);

		for (int i = 0; i < keysProperty.arraySize; i++)
		{
			SerializedProperty elementProperty = keysProperty.GetArrayElementAtIndex(i);
			string stringValue = elementProperty.stringValue;
			stringList.Add(stringValue);
		}

		base.OnGUI(position, property, label);
	}

	protected override void CreateKey(Rect position, SerializedProperty property, GUIContent label, int index)
	{
		for (int j = 0; j < characterNameArray.Length; j++)
		{
			if (stringList[index] == characterNameArray[j]) characterNameIndexe[index] = j;
		}

		int popupIndex = EditorGUI.Popup(position, characterNameIndexe[index], characterNameArray);
		if (popupIndex != characterNameIndexe[index])
		{
			SerializedProperty name = keysProperty.GetArrayElementAtIndex(index);
			name.stringValue = characterNameArray[popupIndex];
			characterNameIndexe[index] = popupIndex;
		}
	}

	protected override void OnAddButtonPressed(SerializedProperty keysProperty, SerializedProperty valuesProperty)
	{
		keysProperty.arraySize++;
		valuesProperty.arraySize++;
		WriteSerialzedProperty(keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1));
		WriteSerialzedProperty(valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1));
		keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1).serializedObject.ApplyModifiedProperties();
		valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1).serializedObject.ApplyModifiedProperties();
	}

	private static bool WriteSerialzedProperty(SerializedProperty sp)
	{
		// Type the property and fill with new value
		SerializedPropertyType type = sp.propertyType; // get the property type

		if (type == SerializedPropertyType.Integer)
		{

			sp.intValue = 0;

		}
		else if (type == SerializedPropertyType.Boolean)
		{

			sp.boolValue = false;

		}
		else if (type == SerializedPropertyType.Float)
		{
			sp.floatValue = 0f;
		}
		else if (type == SerializedPropertyType.String)
		{
			sp.stringValue = "";
		}
		else if (type == SerializedPropertyType.Color)
		{
			sp.colorValue = new Color();
		}
		else if (type == SerializedPropertyType.ObjectReference)
		{
			sp.objectReferenceValue = null;
		}
		else if (type == SerializedPropertyType.LayerMask)
		{
			sp.intValue = 0;
		}
		else if (type == SerializedPropertyType.Enum)
		{
			sp.enumValueIndex = 0;
		}
		else if (type == SerializedPropertyType.Vector2)
		{
			sp.vector2Value = new Vector2();
		}
		else if (type == SerializedPropertyType.Vector3)
		{
			sp.vector3Value = new Vector3();
		}
		else if (type == SerializedPropertyType.Rect)
		{
			sp.rectValue = new Rect();
		}
		else if (type == SerializedPropertyType.ArraySize)
		{
			sp.intValue = 0;
		}
		else if (type == SerializedPropertyType.Character)
		{
			sp.intValue = 0;
		}
		else if (type == SerializedPropertyType.AnimationCurve)
		{
			sp.animationCurveValue = new AnimationCurve();
		}
		else if (type == SerializedPropertyType.Bounds)
		{
			sp.boundsValue = new Bounds();
		}
		else
		{
			Debug.Log("Unsupported SerializedPropertyType \"" + type.ToString() + " encoutered!");
			return false;
		}
		return true;
	}
}
