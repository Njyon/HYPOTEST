using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponMeshData", menuName = "Assets/Weapons/WeaponMeshData")]
public class ScriptableWeaponMeshData : ScriptableObject
{
	[Header("SpawnableMeshData")]
	public Mesh WeaponMesh;
	public Vector3 WeaponOffset = Vector3.zero;
	public Vector3 WeaponRotationEuler = Vector3.zero;
	public Vector3 WeaponScale = Vector3.one;
}
