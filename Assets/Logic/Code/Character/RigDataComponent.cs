using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigDataComponent : MonoBehaviour
{
	[SerializeField] SerializableDictionary<string, Transform> bones = new SerializableDictionary<string, Transform>();
	public SerializableDictionary<string, Transform> Bones { get { return bones; } }

	void Awake()
	{
		Transform root = gameObject.transform.Find("root");
		bones.Add(root.name, root);
		FindBones(root);
	}

	private void FindBones(Transform parentBone)
	{
		for (int i = 0; i < parentBone.transform.childCount; i++)
		{
			Transform bone = parentBone.GetChild(i);
			bones.Add(bone.name, bone);
			FindBones(bone);
		}
	}
}
