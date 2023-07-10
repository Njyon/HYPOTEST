using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;

[CustomEditor(typeof(CharacterSpawner))]
public class CharacterSpawnerInspector : Editor
{
	List<string> characters = new List<string>();
	Object spawnedCharacter;
	int index = 0;
	List<ScriptableCharacter> dataList = new List<ScriptableCharacter>();

	public override VisualElement CreateInspectorGUI()
	{
		CharacterSpawner spawner = (CharacterSpawner)target;

		dataList = Ultra.HypoUttilies.GetAllCharacters();

		for (int i = 0; i < dataList.Count; i++)
		{
			characters.Add(dataList[i].name);
		}

		if (spawner.transform.childCount > 1)
		{
			for (int i = 0; i < spawner.transform.childCount; i++)
			{
				GameObject go = spawner.transform.GetChild(i).gameObject;
				spawner.transform.GetChild(i).parent = null;
				GameObject.DestroyImmediate(go);
			}
		}

		if (spawner.transform.childCount > 0)
		{
			IterateTroughCharacters(spawner);
		}

		return base.CreateInspectorGUI();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		CharacterSpawner spawner = (CharacterSpawner)target;

		DropdownLogic(spawner);
		if (GUILayout.Button("Snap to Ground"))
		{
			spawner.SnapToGround();
		}
	}

	private void DropdownLogic(CharacterSpawner spawner)
	{
		index = EditorGUILayout.Popup("Label", spawner.index, characters.ToArray());

		if (!spawnedCharacter)
		{
			if (spawner.transform.childCount > 0)
			{
				IterateTroughCharacters(spawner);
			}
			else
			{
				spawnedCharacter = PrefabUtility.InstantiatePrefab(dataList[spawner.index].CharacterPrefab, spawner.transform);
				spawner.spawnableCharacterData = dataList[spawner.index];
			}

		}
		if (index != spawner.index)
		{
			spawner.index = index;
			if (spawnedCharacter)
			{
				GameObject.DestroyImmediate(spawnedCharacter);
			}
			spawnedCharacter = PrefabUtility.InstantiatePrefab(dataList[spawner.index].CharacterPrefab, spawner.transform);
			spawner.spawnableCharacterData = dataList[spawner.index];
		}
	}

	private void IterateTroughCharacters(CharacterSpawner spawner)
	{
		for (int i = 0; i < dataList.Count; i++)
		{
			if (spawner.transform.GetChild(0).gameObject.name == dataList[i].CharacterPrefab.name)
			{
				spawner.index = i;
				index = i;
				spawnedCharacter = spawner.transform.GetChild(0).gameObject;
				break;
			}
		}
	}
}
