using UnityEngine;

/// <summary>
/// Light sensor — used exclusively by The Shade.
/// Detects whether a lit torch is within a given radius.
///
/// The player's TorchController sets PlayerTorch.isLit via a static reference.
/// </summary>
public class LightSensor : MonoBehaviour
{
    [Header("Light Settings")]
    [Tooltip("Radius within which a lit torch forces RETREAT.")]
    public float lightDetectRadius = 5f;

    [Tooltip("Reference to the player's torch transform (drag in Inspector).")]
    public Transform torchTransform;

    /// <summary>Returns true if the player's torch is lit and within detection range.</summary>
    public bool DetectsTorch()
    {
        if (torchTransform == null) return false;

        // TorchController exposes a static bool for easy cross-script access
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
