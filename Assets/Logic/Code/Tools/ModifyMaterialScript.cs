using MoreMountains.Tools;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public enum EMaterialPropertyModificationType
{
	MPMT_Float,
	MPMT_Vector2,
	MPMT_Vector3,
	MPMT_Color,
}

[Serializable]
public class MaterialPropertyModification
{
	public string materialPropertyName;
	public EMaterialPropertyModificationType materialPropertyModificationType;

	[ConditionalField(nameof(materialPropertyModificationType), false, EMaterialPropertyModificationType.MPMT_Float)] public float floatValue;
	[ConditionalField(nameof(materialPropertyModificationType), false, EMaterialPropertyModificationType.MPMT_Vector2)] public Vector2 vector2Value;
	[ConditionalField(nameof(materialPropertyModificationType), false, EMaterialPropertyModificationType.MPMT_Vector3)] public Vector3 vector3Value;
	[ConditionalField(nameof(materialPropertyModificationType), false, EMaterialPropertyModificationType.MPMT_Color)] public Color colorValue;

	public float interpolationTime = 1f;
	public Ultra.Timer timer;

	Material[] materials;

	public void Init(ref List<Material> mats)
	{
		timer = new Ultra.Timer(interpolationTime, false);
		materials = mats.ToArray();
	}

	public delegate void CaseLogic(Material mat);

	public void Update(float deltaTime)
	{
		if (!timer.IsRunning) return;

		timer.Update(deltaTime);

		Action<CaseLogic> iterate = (CaseLogic caseLogic) =>
		{
			foreach (Material mat in materials)
			{
				if (mat != null && caseLogic != null)
					caseLogic(mat);
			}
		};

		switch (materialPropertyModificationType)
		{
			case EMaterialPropertyModificationType.MPMT_Float:
				iterate((Material mat) => 
				{
					mat.SetFloat(materialPropertyName, Mathf.Lerp(mat.GetFloat(materialPropertyName), floatValue, timer.GetProgress()));
				});
				break;
			case EMaterialPropertyModificationType.MPMT_Vector2:
				iterate((Material mat) =>
				{
					mat.SetVector(materialPropertyName, Vector2.Lerp(mat.GetVector(materialPropertyName), vector2Value, timer.GetProgress()));
				});
				break;
			case EMaterialPropertyModificationType.MPMT_Vector3:
				iterate((Material mat) =>
				{
					mat.SetVector(materialPropertyName, Vector3.Lerp(mat.GetVector(materialPropertyName), vector3Value, timer.GetProgress()));
				});
				break;
			case EMaterialPropertyModificationType.MPMT_Color:
				iterate((Material mat) =>
				{
					mat.SetVector(materialPropertyName, Color.Lerp(mat.GetColor(materialPropertyName), colorValue, timer.GetProgress()));
				});
				break;
			default:
				Ultra.Utilities.Instance.DebugErrorString("MaterialPropertyModification", "Update", materialPropertyModificationType.ToString() + " has No MaterialProperyModification Implentation!");
				break;
		}
	}
}

public class ModifyMaterialScript : MonoBehaviour
{
	public List<MaterialPropertyModification> materialPropertyModifications = new List<MaterialPropertyModification>();
	public List<MeshRenderer> meshRenderer = new List<MeshRenderer>();

	List<Material> materials = new List<Material>();

	void Awake()
	{
		if (meshRenderer.Count == 0)
		{
			MeshRenderer mr = GetComponent<MeshRenderer>();
			meshRenderer.Add(mr);
		}

		foreach(MeshRenderer mr in meshRenderer)
		{
			foreach(Material mat in mr.materials)
			{
				materials.Add(mat);
			}
		}
	}

	public void ModifyMaterials()
	{
		foreach (MaterialPropertyModification mod in materialPropertyModifications)
		{
			mod.Init(ref materials);
		}
	}

	void Update()
	{
		foreach(MaterialPropertyModification mod in materialPropertyModifications)
		{
			mod.Update(Time.deltaTime);
		}
	}
}
