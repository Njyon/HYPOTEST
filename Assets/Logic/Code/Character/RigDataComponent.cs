using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigDataComponent : MonoBehaviour
{
	[SerializeField] SerializableDictionary<string, Transform> bones = new SerializableDictionary<string, Transform>();
	public SerializableDictionary<string, Transform> Bones { get { return bones; } }

	[SerializeField] List<Rigidbody> regdollRigidBodys = new List<Rigidbody>();
	public List<Rigidbody> RegdollRigidBodys { get { return regdollRigidBodys; } }
	[SerializeField] List<Collider> colliders = new List<Collider>();
	public List<Collider> Colliders {get { return colliders; } }

	void Awake()
	{
		bones.Clear();
		regdollRigidBodys.Clear();

		Transform root = gameObject.transform.Find("root");
		bones.Add(root.name, root);
		Rigidbody rootRigidBody = root.GetComponent<Rigidbody>();
		Collider rootCollider = root.GetComponent<Collider>();
		if (rootRigidBody != null)
			regdollRigidBodys.Add(rootRigidBody);	
		if (rootCollider != null)
			colliders.Add(rootCollider);
		FindBones(root);
	}

	private void FindBones(Transform parentBone)
	{
		for (int i = 0; i < parentBone.transform.childCount; i++)
		{
			Transform bone = parentBone.GetChild(i);
			bones.Add(bone.name, bone);
			Rigidbody boneRigitBody = bone.GetComponent<Rigidbody>();
			Collider boneCollider = bone.GetComponent<Collider>();
			if (boneRigitBody != null)
				regdollRigidBodys.Add(boneRigitBody);
			if (boneCollider != null)
				colliders.Add(boneCollider);
			FindBones(bone);
		}
	}
}
