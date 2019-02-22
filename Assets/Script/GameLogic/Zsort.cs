using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zsort : MonoBehaviour
{
    Vector3 position;
    public float smallAmount = 0.0001f;
    void LateUpdate()
    {
        position = transform.position;
        transform.position = new Vector3(position.x, position.y, position.y * smallAmount);
    }
}