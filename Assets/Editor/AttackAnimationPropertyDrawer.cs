using System;
using System.Collections;
using System.Collections.Generic;
using Ultra;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(AttackAnimationHitDetectionData))]
public class AttackAnimationPropertyDrawer : PropertyDrawer
{
	public static float singleLineHeight => 22f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		bool showProperties = property.FindPropertyRelative("showProperties").boolValue;
		Rect FoldDrawer = new Rect(position.x, position.y, position.width, singleLineHeight);
		showProperties = EditorGUI.Foldout(FoldDrawer, showProperties, "Hit Detection");
		property.FindPropertyRelative("showProperties").boolValue = showProperties;
		if (showProperties)
		{
			GUIContent tempLabel = label;
			

			EHitDetectionType type = (EHitDetectionType)property.FindPropertyRelative("hitDetectionType").enumValueIndex;

			switch (type)
			{
				case EHitDetectionType.Mesh:
					{
						tempLabel.text = "Mesh";
						tempLabel.tooltip = "the mesh that will be used to detect collision";
						Rect meshRect = new Rect(position.x, position.y + singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(meshRect, property.FindPropertyRelative("mesh"), tempLabel, true);
						tempLabel.text = "Offset";
						tempLabel.tooltip = "the offset on the hit detection mesh";
						Rect offsetRect = new Rect(position.x, position.y + singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(offsetRect, property.FindPropertyRelative("offset"), tempLabel, true);
						tempLabel.text = "Scale";
						tempLabel.tooltip = "the scale of the mesh";
						Rect scaleRect = new Rect(position.x, position.y + singleLineHeight * 4, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(scaleRect, property.FindPropertyRelative("scale"), tempLabel, true);
						//tempLabel.text = "Debug Material";
						//tempLabel.tooltip = "the material wich shows the mesh for debug";
						//Rect materialRect = new Rect(position.x, position.y + singleLineHeight * 5, position.width, EditorGUIUtility.singleLineHeight);
						//EditorGUI.PropertyField(materialRect, property.FindPropertyRelative("material"), tempLabel, true);
					}
					break;
				case EHitDetectionType.Sphere:
					{
						tempLabel.text = "Radius";
						tempLabel.tooltip = "the radius of the Sphere";
						Rect radiusRect = new Rect(position.x, position.y + singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(radiusRect, property.FindPropertyRelative("radius"), tempLabel, true);
						tempLabel.text = "Offset";
						tempLabel.tooltip = "the offset on the hit detection Sphere";
						Rect offsetRect = new Rect(position.x, position.y + singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(offsetRect, property.FindPropertyRelative("offset"), tempLabel, true);
					}
					break;
				case EHitDetectionType.Box:
					{
						tempLabel.text = "BoxDimensions";
						tempLabel.tooltip = "the Dimensions of the Box";
						Rect boxDimensionsRect = new Rect(position.x, position.y + singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(boxDimensionsRect, property.FindPropertyRelative("boxDimensions"), tempLabel, true);
						tempLabel.text = "Offset";
						tempLabel.tooltip = "the offset on the hit detection Box";
						Rect offsetRect = new Rect(position.x, position.y + singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(offsetRect, property.FindPropertyRelative("offset"), tempLabel, true);
					}
					break;
				case EHitDetectionType.Capsul:
					{
						tempLabel.text = "Radius";
						tempLabel.tooltip = "the radius of the Capsul";
						Rect radiusRect = new Rect(position.x, position.y + singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(radiusRect, property.FindPropertyRelative("radius"), tempLabel, true);
						tempLabel.text = "Height";
						tempLabel.tooltip = "the height of the Capsul";
						Rect capsulHeightRect = new Rect(position.x, position.y + singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(capsulHeightRect, property.FindPropertyRelative("capsulHeight"), tempLabel, true);
						tempLabel.text = "Offset";
						tempLabel.tooltip = "the offset on the hit detection Capsul";
						Rect offsetRect = new Rect(position.x, position.y + singleLineHeight * 4, position.width, EditorGUIUtility.singleLineHeight);
						EditorGUI.PropertyField(offsetRect, property.FindPropertyRelative("offset"), tempLabel, true);
					}
					break;
				default: break;
			}

			tempLabel.text = "EHitDectectionType";
			tempLabel.tooltip = "The Hit Detection Type";
			Rect enumRect = new Rect(position.x, position.y + singleLineHeight, position.width, singleLineHeight);
			//EditorGUI.PropertyField(enumRect, property.FindPropertyRelative("hitDetectionType"), tempLabel, true);
			//enumRect = new Rect(position.x, position.y + singleLineHeight, position.width, singleLineHeight);
			int index = EditorGUI.Popup(enumRect, "EHitDetectionType", (int)type, Enum.GetNames(typeof(EHitDetectionType)));
			property.FindPropertyRelative("hitDetectionType").enumValueIndex = index;
			EHitDetectionType newType = (EHitDetectionType)property.FindPropertyRelative("hitDetectionType").enumValueIndex;
			if (type != newType)
			{
				Ultra.Hope.Instance.SwitchAsset(Selection.activeObject);
			}
		}
		else
		{

		}

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		EHitDetectionType type = (EHitDetectionType)property.FindPropertyRelative("hitDetectionType").enumValueIndex;
		bool showProperties = property.FindPropertyRelative("showProperties").boolValue;

		float height = 1;
		height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("hitDetectionType"), label, true);
		if (showProperties)
		{
			height += EditorGUIUtility.singleLineHeight;
			height += EditorGUIUtility.singleLineHeight;
			switch (type)
			{
				case EHitDetectionType.Mesh:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("mesh"), label, true);
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("offset"), label, true);
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("scale"), label, true);
					//height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("material"), label, true);
					break;
				case EHitDetectionType.Sphere:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("radius"), label, true);
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("offset"), label, true);
					break;
				case EHitDetectionType.Box:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("boxDimensions"), label, true);
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("offset"), label, true);
					break;
				case EHitDetectionType.Capsul:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("radius"), label, true);
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("capsulHeight"), label, true);
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("scale"), label, true);
					break;
				default: break;
			}
		}
		else
		{

		}
		return height;
	}
}
