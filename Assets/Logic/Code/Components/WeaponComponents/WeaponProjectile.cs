using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
	[SerializeField] float speed = 5f;
    Vector3 direction;

    public Vector3 Direction { get { return direction; } set { direction = value; } }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + Direction * (speed * Time.deltaTime);
    }
}
