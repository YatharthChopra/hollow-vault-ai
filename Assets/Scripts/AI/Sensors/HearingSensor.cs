using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingSensor : MonoBehaviour
{
    public float hearingRadius = 8f;

    [HideInInspector] public bool heardSoundThisFrame;
    [HideInInspector] public Vector3 lastHeardPosition;

    private void OnEnable()
    {
        SoundEventBroadcaster.Register(this);
    }

    private void OnDisable()
    {
        SoundEventBroadcaster.Unregister(this);
    }

    public bool CanHear(Vector3 soundPosition, float soundRadius)
    {
        float dist = Vector3.Distance(transform.position, soundPosition);
        bool heard = dist <= Mathf.Min(soundRadius, hearingRadius);

        if (heard)
        {
            heardSoundThisFrame = true;
            lastHeardPosition = soundPosition;
        }
        return heard;
    }

    // call this at the start of each Update to reset the flag
    public void ResetFrame()
    {
        heardSoundThisFrame = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
