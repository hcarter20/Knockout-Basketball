using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meter : MonoBehaviour
{
    [SerializeField] private Image marker;
    private float minThrowPower, maxThrowPower;
    private float throwPower;

    private void Start()
    {
        // Set the min and max expected power
        minThrowPower = PlayerControl.player.minThrowSpeed;
        maxThrowPower = PlayerControl.player.maxThrowSpeed;
    }

    // this is supposed to access the throw speed variable T-T
    // Update is called once per frame
    void Update()
    {
        setMarker();
    }

    void setMarker()
    {
        throwPower = PlayerControl.player.throwSpeed;

        //float targetZ = PlayerControl.player.lower ? 16.0f : -16.0f;
        // Quaternion newAngle = Quaternion.LerpUnclamped(marker.transform.rotation, Quaternion.Euler(0.0f, 0.0f, targetZ), Time.deltaTime);

        // Just eyeballed it: should clamp between 16 and -16 degrees z-rotation for min/max.
        Debug.Log("Min " + minThrowPower + ", Max " + maxThrowPower + ", current " + throwPower);
        float t = Mathf.InverseLerp(minThrowPower, maxThrowPower, throwPower);
        Debug.Log(t);
        Quaternion newAngle = Quaternion.Lerp(Quaternion.Euler(0.0f, 0.0f, 16.0f), Quaternion.Euler(0.0f, 0.0f, -16.0f), t);

        marker.transform.rotation = newAngle;
    }
}
