using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DummyRigOffsets
{
	public TransformOffsets ankleL;
	public TransformOffsets ankleR;
	public TransformOffsets gaunteltL;
	public TransformOffsets gaunteltR;
	public TransformOffsets lowerLegL;
	public TransformOffsets lowerLegR;
	public TransformOffsets midFeetL;
	public TransformOffsets midFeetR;
	public TransformOffsets toeL;
	public TransformOffsets toeR;
}

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

	[SerializeField] DummyRigOffsets dummyOffsets;

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
		SetUpObjectWithBone(ankleL, DummyCharacterBones.footL, dummyOffsets.ankleL);
		SetUpObjectWithBone(ankleR, DummyCharacterBones.footR, dummyOffsets.ankleR);
		SetUpObjectWithBone(gaunteltL, DummyCharacterBones.forearm_twistL, dummyOffsets.gaunteltL);
		SetUpObjectWithBone(gaunteltR, DummyCharacterBones.forearm_twistR, dummyOffsets.gaunteltR);
		SetUpObjectWithBone(lowerLegL, DummyCharacterBones.leg_stretchL, dummyOffsets.lowerLegL);
		SetUpObjectWithBone(lowerLegR, DummyCharacterBones.leg_stretchR, dummyOffsets.lowerLegR);
		SetUpObjectWithBone(midFeetL, DummyCharacterBones.footL, dummyOffsets.midFeetL);
		SetUpObjectWithBone(midFeetR, DummyCharacterBones.footR, dummyOffsets.midFeetR);
		SetUpObjectWithBone(toeL, DummyCharacterBones.toes_01L, dummyOffsets.toeL);
		SetUpObjectWithBone(toeR, DummyCharacterBones.toes_01R, dummyOffsets.toeR);
	}

	private void SetUpObjectWithBone(GameObject obj, string boneName, TransformOffsets offset)
	{
		Transform newParent = null;
		if (gameCharacter.RigDataComponent.Bones.TryGetValue(boneName, out newParent))
		{
			obj.transform.parent = newParent;
			obj.transform.localPosition = offset.offset;
			obj.transform.localEulerAngles = offset.eulerRotationOffset;
		}
		else
		{
			Debug.LogError("BoneNotFound");
		}
	}
}
