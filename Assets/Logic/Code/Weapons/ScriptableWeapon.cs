using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
	public float MaxUltChargeAmount = 100f;
	public GameObject UIElement;
	public Sprite WeaponImage;
	public List<SoundEffect> defaultAttackSounds;
	public List<SoundEffect> defaultHitSounds;

	private WeaponBase weaponCopy = null;
	public WeaponBase Weapon { get { return weaponCopy; } }
	public virtual void CreateWeapon(GameCharacter gameCharacter)
	{
		weaponCopy = WeaponBase?.instance?.CreateCopy(gameCharacter, this);
	}

	public virtual ScriptableWeapon CreateCopy()
	{
		ScriptableWeapon instance = ScriptableObject.CreateInstance<ScriptableWeapon>();
		CopyData(ref instance);

		return instance;
	}

	protected void CopyData(ref ScriptableWeapon instance)
	{
		instance.WeaponName = WeaponName;
		instance.WeaponBase = WeaponBase;
		instance.AnimationData = AnimationData;
		instance.WeaponMeshData = WeaponMeshData;
		instance.WeaponType = WeaponType;
		instance.MaxChargeAmount = MaxChargeAmount;
		instance.DefaultChargeAmount = DefaultChargeAmount;
		instance.TimeAfterEqupingMaxChargedWeapon = TimeAfterEqupingMaxChargedWeapon;
		instance.UIElement = UIElement;
		instance.WeaponImage = WeaponImage;
		instance.defaultAttackSounds = defaultAttackSounds;
		instance.defaultHitSounds = defaultHitSounds;
	}
	protected void CopyData(ref ScriptableWeaponWrapper instance)
	{
		instance.WeaponName = WeaponName;
		instance.WeaponBase = WeaponBase;
		instance.AnimationData = AnimationData;
		instance.WeaponMeshData = WeaponMeshData;
		instance.WeaponType = WeaponType;
		instance.MaxChargeAmount = MaxChargeAmount;
		instance.DefaultChargeAmount = DefaultChargeAmount;
		instance.TimeAfterEqupingMaxChargedWeapon = TimeAfterEqupingMaxChargedWeapon;
		instance.UIElement = UIElement;
		instance.WeaponImage = WeaponImage;
		instance.defaultAttackSounds = defaultAttackSounds;
		instance.defaultHitSounds = defaultHitSounds;
	}
}
