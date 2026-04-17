using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Global sound event system (static pub/sub pattern).
///
/// Any game object can emit a sound by calling SoundEventBroadcaster.Broadcast().
/// All registered HearingSensor components receive the event and decide whether
/// they are close enough to react.
///
/// Usage:
///   SoundEventBroadcaster.Broadcast(worldPosition, radius);
///
/// HearingSensors register/unregister themselves automatically in OnEnable/OnDisable.
/// </summary>
public static class SoundEventBroadcaster
{
    private static readonly List<HearingSensor> _listeners = new();

    /// <summary>Register a HearingSensor to receive sound events.</summary>
    public static void Register(HearingSensor sensor)
    {
        if (!_listeners.Contains(sensor))
            _listeners.Add(sensor);
    }

    /// <summary>Unregister a HearingSensor.</summary>
    public static void Unregister(HearingSensor sensor)
    {
        _listeners.Remove(sensor);
    }

    /// <summary>
    /// Broadcast a sound event to all registered listeners.
    /// </summary>
    /// <param name="worldPosition">Origin of the sound in world space.</param>
    /// <param name="radius">How far the sound naturally carries (metres).</param>
    public static void Broadcast(Vector3 worldPosition, float radius)
    {
        foreach (HearingSensor sensor in _listeners)
        {
            if (sensor != null)
                sensor.CanHear(worldPosition, radius);
        }
    }
}
