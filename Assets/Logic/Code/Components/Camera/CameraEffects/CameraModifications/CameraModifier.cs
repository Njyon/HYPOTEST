using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModifier : MonoBehaviour
{
    public List<ClassInstance<ACameraModification>> modification = new List<ClassInstance<ACameraModification>>();

    public void ApplyModifications(PlayerGameCharacter obj)
    {
        foreach (var mod in modification)
        {
            mod.instance.Init(obj, CameraController.Instance);
            mod.instance.DoOperation();
        }
    }
}
