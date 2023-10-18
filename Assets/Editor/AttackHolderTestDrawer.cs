using System;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(AttackDataHolder))]
//public class AttackDataHolderDrawer : PropertyDrawer
//{
//	public static float singleLineHeight => 22f;
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//	{
//		EditorGUI.BeginProperty(position, label, property);
//
//		// Zeigen Sie die "attack" Eigenschaft an.
//		SerializedProperty attackProp = property.FindPropertyRelative("attack");
//
//		BaseAttack baseAttack = null;
//		switch (attackProp.type)
//		{
//			case "StandartAttack": baseAttack = new StandartAttack(); break;
//			case "NotStandartAttack": baseAttack = new NotStandartAttack(); break;
//			default: break;
//		}
//
//		// Setzen Sie "attackRef" auf "attack".
//		SerializedProperty attackRefProp = property.FindPropertyRelative("attackRef");
//		if (baseAttack != null)
//		{
//			attackRefProp.objectReferenceValue = (UnityEngine.Object)Activator.CreateInstance(baseAttack.GetAttackDataType());
//
//		}
//
//		GUIContent tempLabel = label;
//
//		GUIContent label2 = new GUIContent(label);
//		tempLabel.text = "AttackScritableObject";
//		Rect meshRect = new Rect(position.x, position.y + singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
//		EditorGUI.PropertyField(meshRect, attackProp, tempLabel, true);
//		if (attackRefProp != null)
//		{
//
//			tempLabel.text = "AttackData";
//			meshRect = new Rect(position.x, position.y + singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
//			EditorGUI.PropertyField(meshRect, attackRefProp, tempLabel, true);
//		}
//
//		EditorGUI.EndProperty();
//	}
//	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//	{
//
//		float height = 1;
//		height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("attack"), label, true);
//		height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("attackRef"), label, true);
//
//		return height;
//	}
//}
