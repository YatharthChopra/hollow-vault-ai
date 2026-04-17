using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundEventBroadcaster
{
    static List<HearingSensor> listeners = new List<HearingSensor>();

    public static void Register(HearingSensor sensor)
    {
        if (!listeners.Contains(sensor))
            listeners.Add(sensor);
    }

    public static void Unregister(HearingSensor sensor)
    {
        listeners.Remove(sensor);
    }

    // call this whenever something makes noise in the world
    public static void Broadcast(Vector3 position, float radius)
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            if (listeners[i] != null)
                listeners[i].CanHear(position, radius);
        }
    }
}
