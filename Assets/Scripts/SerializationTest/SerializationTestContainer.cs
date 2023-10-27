using System;
using System.Collections.Generic;
using UnityEngine;

public class SerializationTestContainer : MonoBehaviour
{
    [Header("Single")]
    public ClassInstance<MyCoolSerializeBase> myCoolSingleInstance = new ClassInstance<MyCoolSerializeBase>();
    
    [Header("Multi")]
    public List<ClassInstance<MyCoolSerializeBase>> myCoolListInstance = new List<ClassInstance<MyCoolSerializeBase>>();
}
