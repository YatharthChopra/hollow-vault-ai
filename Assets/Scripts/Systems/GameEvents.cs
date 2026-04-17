using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// central event bus — any script can fire or listen to these without needing direct references
public static class GameEvents
{
    // fired by the player controller when they move (pos, intensity 0-1)
    public static Action<Vector3, float> OnSoundEmitted;

    // fired by CryptSentinel.TakeDamage when HP drops below rally threshold
    public static Action OnSentinelLowHP;

    // fired by SentinelRallyingState — Shade listens and checks distance
    public static Action<Vector3> OnSentinelRallyingCry;

    // fired by PlayerHealth when the player dies
    public static Action OnPlayerDied;
}
