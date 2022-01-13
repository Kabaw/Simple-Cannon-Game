
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float rotation_Z;

    void Update()
    {
        Vector3 rotation = Vector3.zero;

        rotation.x = 0;
        rotation.y = 0;
        rotation.z = rotation_Z;

        transform.Rotate(rotation);
    }
}
