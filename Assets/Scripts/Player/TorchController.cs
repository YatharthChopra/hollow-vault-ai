using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TorchController : MonoBehaviour
{
    public Light torchLight;
    public CryptSentinel sentinel;
    public float visionBoostAmount = 4f;
    public InputActionReference torchToggleInput;

    public static bool IsLit { get; private set; }

    bool boostedSentinel = false;

    private void Awake()
    {
        IsLit = false;
        if (torchLight != null) torchLight.enabled = false;
    }

    private void OnEnable()
    {
        if (torchToggleInput != null)
            torchToggleInput.action.performed += OnToggle;
    }

    private void OnDisable()
    {
        if (torchToggleInput != null)
            torchToggleInput.action.performed -= OnToggle;
    }

    void OnToggle(InputAction.CallbackContext ctx)
    {
        IsLit = !IsLit;
        if (torchLight != null) torchLight.enabled = IsLit;

        // boost sentinel vision when torch is on (tradeoff for repelling the shade)
        if (sentinel != null && sentinel.vision != null)
        {
            if (IsLit && !boostedSentinel)
            {
                sentinel.vision.viewRadius += visionBoostAmount;
                boostedSentinel = true;
            }
            else if (!IsLit && boostedSentinel)
            {
                sentinel.vision.viewRadius -= visionBoostAmount;
                boostedSentinel = false;
            }
        }
    }
}
