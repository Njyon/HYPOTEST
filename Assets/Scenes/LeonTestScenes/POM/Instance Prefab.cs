using UnityEngine;
using System.Collections;

public class Instantiate_example : MonoBehaviour
{
    public Transform prefab;
    void Start()
    {
        Instantiate(prefab, new Vector3(2.0F, 0, 0), Quaternion.identity);
    }
}