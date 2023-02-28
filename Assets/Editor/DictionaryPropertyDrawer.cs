using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class IntStringDictionaryDrawer : PropertyDrawer
{
    private const float buttonWidth = 18f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.indentLevel++;
        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        SerializedProperty valuesProperty = property.FindPropertyRelative("values");

        float widthSize = position.width / 3;
        float offsetSize = 2;

        for (int i = 0; i < keysProperty.arraySize; i++)
        {
            Rect pos1 = new Rect(position.x, position.y, widthSize - offsetSize, position.height * (i + 1));
            Rect pos2 = new Rect(position.x + widthSize * 1, position.y, widthSize - offsetSize, position.height * (i + 1));
            Rect pos3 = new Rect(position.x + widthSize * 2, position.y, widthSize, position.height * (i + 1));

            Rect keyPosition = new Rect(position.x, position.y + (i * EditorGUIUtility.singleLineHeight), position.width - 100f, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(keyPosition, keysProperty.GetArrayElementAtIndex(i), GUIContent.none);

            Rect valuePosition = new Rect(position.x + keyPosition.width + EditorGUIUtility.standardVerticalSpacing, keyPosition.y, position.width - keyPosition.width - buttonWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(valuePosition, valuesProperty.GetArrayElementAtIndex(i), GUIContent.none);

            Rect buttonPosition = new Rect(valuePosition.x + valuePosition.width + EditorGUIUtility.standardVerticalSpacing, valuePosition.y, buttonWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonPosition, "-"))
            {
                keysProperty.DeleteArrayElementAtIndex(i);
                valuesProperty.DeleteArrayElementAtIndex(i);
            }
        }

        EditorGUI.indentLevel--;

        Rect addButtonPosition = new Rect(position.x, position.y + (keysProperty.arraySize * EditorGUIUtility.singleLineHeight), position.width, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(addButtonPosition, "Add"))
        {
            keysProperty.arraySize++;
            valuesProperty.arraySize++;
            WriteSerialzedProperty(keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1));
            WriteSerialzedProperty(valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1));
            keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1).serializedObject.ApplyModifiedProperties();
            valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1).serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
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

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        return (keysProperty.arraySize + 1) * EditorGUIUtility.singleLineHeight;
    }
}
