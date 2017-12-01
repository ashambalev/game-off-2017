using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Vector3 _original_position;

    private float _shake_amount = 0;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _original_position = transform.position;
    }

    public void Shake(float amount, float duration)
    {

        _shake_amount = amount * 0.2f;
        InvokeRepeating("CameraShake", 0, .01f);
        Invoke("StopShaking", 0.3f);
    }

    void CameraShake()
    {
        if (_shake_amount > 0)
        {
            transform.position = _original_position + new Vector3(Random.Range(-_shake_amount, _shake_amount), Random.Range(-_shake_amount, _shake_amount), 0);
        }
    }

    void StopShaking()
    {
        CancelInvoke("CameraShake");
        transform.position = _original_position;
    }
}
