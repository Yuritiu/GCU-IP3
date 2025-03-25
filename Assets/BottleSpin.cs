using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSpin : MonoBehaviour
{
    float spinSpeed = 360f;
    Rigidbody rb;
    bool isSpinning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ThrowBottle();
    }

    void ThrowBottle()
    {
        if (rb != null)
        {
            rb.angularVelocity = new Vector3(spinSpeed, 0, 0);
            isSpinning = true;
        }
    }
}
