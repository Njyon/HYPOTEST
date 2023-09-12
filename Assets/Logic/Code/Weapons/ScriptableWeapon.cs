using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Assets/Weapons/Weapon")]
public class ScriptableWeapon : ScriptableObject
{
	public string WeaponName = "new Weapon";
	public SerializableCharacterDictionary<string, ScriptableWeaponAnimationData> AnimationData;
	[HideInInspector] public string WeaponClassName;
	[HideInInspector] public WeaponBase Weapon;
	public ScriptableWeaponMeshData WeaponMeshData;
	public float MaxChargeAmount = 100;
	public float TimeAfterEqupingMaxChargedWeapon = 3f;
	public GameObject UIElement;
	public Sprite WeaponImage;


}
