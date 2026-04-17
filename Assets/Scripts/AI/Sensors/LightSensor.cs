using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSensor : MonoBehaviour
{
    public float lightDetectRadius = 5f;
    public Transform torchTransform;

    public bool DetectsTorch()
    {
        if (torchTransform == null) return false;
        if (!TorchController.IsLit) return false;

        float dist = Vector3.Distance(transform.position, torchTransform.position);
        return dist <= lightDetectRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, lightDetectRadius);
    }
}
