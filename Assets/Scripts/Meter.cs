using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meter : MonoBehaviour
{
    [SerializeField] private Image marker;
    private float minThrowPower, maxThrowPower;
    private float throwPower;

    // Keeps track of whether the meter should be visible
    private bool isActive = false;

    // The CanvasGroup which controls the transparency of the meter
    public CanvasGroup canvasGroup;

    private void Start()
    {
        // Set the min and max expected power
        minThrowPower = PlayerControl.player.minThrowSpeed;
        maxThrowPower = PlayerControl.player.maxThrowSpeed;

        // Manually set the meter to initial player variables
        SetMarker();
    }

    private void LateUpdate()
    {
        // Don't rely on call, automatically update meter if player is trying to throw
        if (PlayerControl.player != null)
        {
            if (PlayerControl.player.isThrowing)
            {
                isActive = true;
                canvasGroup.alpha = 1.0f;
                SetMarker();
            }
            else if (isActive)
            {
                // On the first update after the ball is thrown, update marker then deactivate
                SetMarker();
                canvasGroup.alpha = 0.33f;
                isActive = false;
            }
        }
    }

    public void SetMarker()
    {
        // Get the new current throwing power from the player
        throwPower = PlayerControl.player.throwSpeed;

        // Just eyeballed it: should clamp between 16 and -16 degrees z-rotation for min/max.
        // egchan: modified angle for masking/ bar fill to 26 deg
        // Max power is top and min power is bottom of meter, use lerp to place marker at current power
        float t = Mathf.InverseLerp(minThrowPower, maxThrowPower, throwPower);
        Quaternion newAngle = Quaternion.Lerp(Quaternion.Euler(0.0f, 0.0f, 20.0f), Quaternion.Euler(0.0f, 0.0f, -26.0f), t);

        // Use local rotation, since marker is a child of the meter background.
        marker.transform.localRotation = newAngle;
    }
}
