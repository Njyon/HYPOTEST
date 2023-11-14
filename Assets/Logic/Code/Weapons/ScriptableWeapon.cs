using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeaponType
{
	Melee,
	Ranged,
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Assets/Weapons/Weapon")]
public class ScriptableWeapon : ScriptableObject
{
	public string WeaponName = "new Weapon";
	public ClassInstance<WeaponBase> WeaponBase;
	public SerializableCharacterDictionary<string, ScriptableWeaponAnimationData> AnimationData;
	//[HideInInspector] public string WeaponClassName;
	//[HideInInspector] public WeaponBase Weapon;
	public ScriptableWeaponMeshData WeaponMeshData;
	public EWeaponType WeaponType;
	public float MaxChargeAmount = 1000f;
	public float DefaultChargeAmount = 700f;
	public float TimeAfterEqupingMaxChargedWeapon = 3f;
	public GameObject UIElement;
	public Sprite WeaponImage;
	public List<SoundEffect> defaultAttackSounds;
	public List<SoundEffect> defaultHitSounds;

	private WeaponBase weaponCopy = null;
	public WeaponBase Weapon { get { return weaponCopy; } }
	public void CreateWeapon(GameCharacter gameCharacter)
	{
		weaponCopy = WeaponBase?.instance?.CreateCopy(gameCharacter, this);
	}
}
