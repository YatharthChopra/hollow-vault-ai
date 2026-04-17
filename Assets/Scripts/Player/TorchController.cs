using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// handles torch toggle — F key turns it on/off
// when lit: repels The Shade but boosts Sentinel vision range (the core tradeoff)
public class TorchController : MonoBehaviour
{
    public Light torchLight;
    public CryptSentinel sentinel;
    public float visionBoostAmount = 4f;

    public static bool IsLit { get; private set; }

    bool boosted = false;

    void Awake()
    {
        IsLit = false;
        if (torchLight != null) torchLight.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ToggleTorch();
    }

    void ToggleTorch()
    {
        IsLit = !IsLit;
        if (torchLight != null) torchLight.enabled = IsLit;

        // boost Sentinel vision when torch is on
        if (sentinel != null && sentinel.vision != null)
        {
            if (IsLit && !boosted)
            {
                sentinel.vision.viewRadius += visionBoostAmount;
                boosted = true;
            }
            else if (!IsLit && boosted)
            {
                sentinel.vision.viewRadius -= visionBoostAmount;
                boosted = false;
            }
        }
    }
}
