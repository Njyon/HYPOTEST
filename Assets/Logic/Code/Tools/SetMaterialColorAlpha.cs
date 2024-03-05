using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterialColorAlpha : MonoBehaviour
{
    [SerializeField] float alphaTarget = 0f;

    void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        foreach (Material mat in mr.materials)
        {
			mat.color = mat.color.WithAlpha(alphaTarget);
        }
    }
}
