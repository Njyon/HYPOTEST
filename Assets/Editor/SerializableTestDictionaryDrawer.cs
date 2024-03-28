#if UNITY_EDITOR
using Codice.CM.Common;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BetterSerializableDictionary<,>))]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    public float padding = 3f; // Padding zwischen den Elementen
    public float valueWidthPercentage = 0.7f; // Prozentsatz der Breite für den Wert

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		// Zeichne den Label des Feldes
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Erhalte die Listenelemente für Schlüssel und Werte
		var keysProperty = property.FindPropertyRelative("keys");
		var valuesProperty = property.FindPropertyRelative("values");
		var count = keysProperty.arraySize;

		// Berechne die Höhe für ein einzelnes Zeilenfeld
		var lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		// Breite für den "Remove Element" Button
		var buttonWidth = lineHeight;

		// Breite für den Wert
		var valueWidth = (position.width - buttonWidth - padding * 2f) * valueWidthPercentage;

		// Breite für den Schlüssel
		var keyWidth = position.width - valueWidth - buttonWidth - padding * 3f;

		// Position für den "Add Element" Button
		var addButtonRect = new Rect(position.x, position.y, position.width, lineHeight);

		// Zeichne den "Add Element" Button
		if (GUI.Button(addButtonRect, "Add Element"))
		{
			OnAddButtonPressed(keysProperty, valuesProperty);
		}

		// Berechne die Höhe des gesamten Dictionary-Bereichs
		var dictionaryHeight = count * lineHeight;

		// Position für das erste Dictionary-Element
		var elementPosition = new Rect(position.x, position.y + lineHeight, position.width, lineHeight);

		// Zeichne jedes Schlüssel-Wert-Paar
		for (int i = 0; i < count; i++)
		{
			// Erhalte die SerializedProperties für den aktuellen Schlüssel und Wert
			var keyProperty = keysProperty.GetArrayElementAtIndex(i);
			var valueProperty = valuesProperty.GetArrayElementAtIndex(i);

			// Berechne die Rechtecke für den Schlüssel und den "Remove Element" Button
			var keyRect = new Rect(elementPosition.x, elementPosition.y, keyWidth, EditorGUI.GetPropertyHeight(keyProperty));
			var buttonRect = new Rect(elementPosition.xMax - buttonWidth, elementPosition.y, buttonWidth, EditorGUI.GetPropertyHeight(keyProperty));

			// Zeichne die PropertyFields für den Schlüssel
			CreateKey(keyRect, keyProperty, GUIContent.none, i);

			// Zeichne den "Remove Element" Button
			if (GUI.Button(buttonRect, "-"))
			{
				keysProperty.DeleteArrayElementAtIndex(i);
				valuesProperty.DeleteArrayElementAtIndex(i);
				break;
			}

			var valueHeight = EditorGUI.GetPropertyHeight(valueProperty, GUIContent.none, true);
			bool simple = true;

			// Maybe needs more cases
			switch (valueProperty.propertyType)
			{
				case SerializedPropertyType.Generic:
					simple = false;
					break;
				default:
					break;
			}


			if (valueProperty.isExpanded)
			{
				// Berechne das Rechteck für den Wert
				var valueRect = new Rect(elementPosition.x + padding, elementPosition.y + lineHeight, position.width, EditorGUI.GetPropertyHeight(valueProperty));
				// Zeichne die PropertyFields für den Wert
				EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent(valueProperty.type.ToString()), true);
			}
			else if (!valueProperty.isExpanded && !simple)
			{
				// Berechne das Rechteck für den Wert
				var valueRect = new Rect(keyRect.xMax + padding, elementPosition.y, valueWidth, EditorGUI.GetPropertyHeight(valueProperty));
				// Zeichne die PropertyFields für den Wert
				EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent(valueProperty.type.ToString()), true);
			}
			else
			{
				// Berechne das Rechteck für den Wert
				var valueRect = new Rect(keyRect.xMax + padding, elementPosition.y, valueWidth, EditorGUI.GetPropertyHeight(valueProperty));
				// Zeichne die PropertyFields für den Wert
				EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none, true);
			}

		

			// Aktualisiere die Position für das nächste Dictionary-Element
			elementPosition.y += Mathf.Max(EditorGUI.GetPropertyHeight(keyProperty), EditorGUI.GetPropertyHeight(valueProperty)) + EditorGUIUtility.standardVerticalSpacing;
		}

		EditorGUI.EndProperty();
	}

	protected virtual void CreateKey(Rect position, SerializedProperty property, GUIContent label, int index)
	{
		EditorGUI.PropertyField(position, property, label); 
	}

	protected virtual void OnAddButtonPressed(SerializedProperty keysProperty, SerializedProperty valuesProperty)
	{
		keysProperty.arraySize++;
		valuesProperty.arraySize++;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		var keysProperty = property.FindPropertyRelative("keys");
		int count = keysProperty.arraySize;

		// Berechne die Höhe für ein einzelnes Zeilenfeld
		float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		// Höhe des "Add Element" Buttons und der Schlüssel-Wert-Paare
		float totalHeight = (count + 1) * EditorGUIUtility.standardVerticalSpacing;
		totalHeight += EditorGUIUtility.singleLineHeight;

		// Berücksichtige die Höhe der Werte, wenn sie mehrzeilig sind
		for (int i = 0; i < count; i++)
		{
			var valueProperty = property.FindPropertyRelative("values").GetArrayElementAtIndex(i);
			totalHeight += EditorGUI.GetPropertyHeight(valueProperty);
			totalHeight += EditorGUIUtility.standardVerticalSpacing / 2;
		}

		//totalHeight += EditorGUIUtility.standardVerticalSpacing / 2;

		return totalHeight;
	}
}
#endif
