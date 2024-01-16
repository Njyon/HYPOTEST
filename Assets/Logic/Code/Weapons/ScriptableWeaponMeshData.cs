using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SecondWeaponOffsets
{
	public Vector3 WeaponOffset = Vector3.zero;
	public Vector3 WeaponRotationEuler = Vector3.zero;
	public Vector3 WeaponScale = Vector3.one;
}

[CreateAssetMenu(fileName = "New WeaponMeshData", menuName = "Assets/Weapons/WeaponMeshData")]
public class ScriptableWeaponMeshData : ScriptableObject
{
	[Header("SpawnableMeshData")]
	public GameObject WeaponMesh;
	public Vector3 WeaponOffset = Vector3.zero;
	public Vector3 WeaponRotationEuler = Vector3.zero;
	public Vector3 WeaponScale = Vector3.one;
	public bool cascadeurSetUp = true;
	public SecondWeaponOffsets secondWeaponOffsets;
}
