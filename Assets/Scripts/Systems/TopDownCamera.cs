using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// simple top-down camera that follows the player
public class TopDownCamera : MonoBehaviour
{
    public Transform target;
    public float height = 20f;

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x, height, target.position.z);
    }
}
