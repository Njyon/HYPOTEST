using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HighlightSelectedObject
{
	static EditorWindow hierarchy;
	static EditorWindow Hierarchy
	{
		get
		{
			if (hierarchy == null)
			{
				var hierarchyWindow = Resources.FindObjectsOfTypeAll<EditorWindow>();

				foreach (EditorWindow window in hierarchyWindow)
				{
					if (window.titleContent.text == "Hierarchy")
					{
						// Fokussiere das Hierarchie-Fenster
						hierarchy = window;
						break;
					}
				}
			}
			return hierarchy;
		}
	}


	static HighlightSelectedObject()
	{
		// Registriere eine Callback-Funktion, die bei einer Änderung in der Hierarchie oder Szene aufgerufen wird
		//Selection.selectionChanged += UpdateHighlight;
	}

	[MenuItem("MyTools/Select Hierarchy")]
	static void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			Hierarchy?.Focus();
		}
	}
}
