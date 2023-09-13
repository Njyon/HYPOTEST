using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipFistWeaponScript : MonoBehaviour
{
	[SerializeField] GameObject ankleL;
	[SerializeField] GameObject ankleR;
	[SerializeField] GameObject gaunteltL;
	[SerializeField] GameObject gaunteltR;
	[SerializeField] GameObject lowerLegL;
	[SerializeField] GameObject lowerLegR;
	[SerializeField] GameObject midFeetL;
	[SerializeField] GameObject midFeetR;
	[SerializeField] GameObject toeL;
	[SerializeField] GameObject toeR;

	GameCharacter gameCharacter;

	void Awake()
	{
		Transform parent = transform.parent;
		if (parent == null)
		{
			Debug.LogError("Parent was null!");
			return;
		}

		gameCharacter = parent.gameObject.GetComponent<GameCharacter>();
		if (gameCharacter == null)
		{
			Debug.LogError("GameCharacter was null!");
			return;
		}

		switch (gameCharacter.GameCharacterData.RigType)
		{
			case RigType.Dummy:
				EquitWeaponOnDummyRig();
				break;
			default:
				Debug.LogError("Unknown Rig!");
				return;
		}
	}

	void OnDestroy()
	{
		Destroy(ankleL);
		Destroy(ankleR);
		Destroy(gaunteltL);
		Destroy(gaunteltR);
		Destroy(lowerLegL);
		Destroy(lowerLegR);
		Destroy(midFeetL);
		Destroy(midFeetR);
		Destroy(toeL);
		Destroy(toeR);
	}

	void EquitWeaponOnDummyRig()
	{
		SetUpObjectWithBone(ankleL, DummyCharacterBones.leg_twistL);
		SetUpObjectWithBone(ankleR, DummyCharacterBones.leg_twistR);
		SetUpObjectWithBone(gaunteltL, DummyCharacterBones.forearm_twistL);
		SetUpObjectWithBone(gaunteltR, DummyCharacterBones.forearm_twistR);
		SetUpObjectWithBone(lowerLegL, DummyCharacterBones.leg_stretchL);
		SetUpObjectWithBone(lowerLegR, DummyCharacterBones.leg_stretchR);
		SetUpObjectWithBone(midFeetL, DummyCharacterBones.footL);
		SetUpObjectWithBone(midFeetR, DummyCharacterBones.footR);
		SetUpObjectWithBone(toeL, DummyCharacterBones.toes_01L);
		SetUpObjectWithBone(toeR, DummyCharacterBones.toes_01R);
	}

	private void SetUpObjectWithBone(GameObject obj, string boneName)
	{
		Transform newParent = null;
		if (gameCharacter.RigDataComponent.Bones.TryGetValue(boneName, out newParent))
		{
			obj.transform.parent = newParent;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
		}
		else
		{
			Debug.LogError("BoneNotFound");
		}
	}
}
