using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player's handheld torch.
///
/// Mechanics:
///   - Toggle with the designated input action (default: F key or Fire2).
///   - When lit: forces Shade into RETREAT (LightSensor reads IsLit).
///   - When lit: increases Sentinel vision radius by visionBoostAmount (makes Sentinel
///     more dangerous — the key player trade-off in Hollow Vault).
///   - Exposes a static bool IsLit for cross-script access without coupling.
/// </summary>
public class TorchController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The torch light source (point light or spot light).")]
    public Light torchLight;

    [Tooltip("The Sentinel — needed to boost its vision when torch is lit.")]
    public CryptSentinel sentinel;

    [Header("Config")]
    [Tooltip("Extra vision radius added to Sentinel while torch is active.")]
    public float visionBoostAmount = 4f;

    [Header("Input")]
    public InputActionReference torchToggleInput;

    // Static accessor for LightSensor — avoids requiring a direct reference
    public static bool IsLit { get; private set; }

    private bool boostedSentinel = false;

    private void Awake()
    {
        IsLit = false;
        if (torchLight != null) torchLight.enabled = false;
    }

    private void OnEnable()
    {
        if (torchToggleInput != null)
            torchToggleInput.action.performed += OnToggleTorch;
    }

    private void OnDisable()
    {
        if (torchToggleInput != null)
            torchToggleInput.action.performed -= OnToggleTorch;
    }

    private void OnToggleTorch(InputAction.CallbackContext ctx)
    {
        IsLit = !IsLit;
        if (torchLight != null) torchLight.enabled = IsLit;

        // Boost/restore Sentinel vision radius
        if (sentinel != null && sentinel.vision != null)
        {
            if (IsLit && !boostedSentinel)
            {
                sentinel.vision.viewRadius += visionBoostAmount;
                boostedSentinel = true;
                Debug.Log("[Torch] Lit — Sentinel vision boosted.");
            }
            else if (!IsLit && boostedSentinel)
            {
                sentinel.vision.viewRadius -= visionBoostAmount;
                boostedSentinel = false;
                Debug.Log("[Torch] Extinguished — Sentinel vision restored.");
            }
        }
    }
}
