using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public bool grabbed;

    public float CookingPercentage = 0;
    public bool isCooked = false;
    public bool isCooking = false;

    public void Grab() {
        // Ignore if no fixed joint
        if (!gameObject.TryGetComponent<FixedJoint>(out var joint)) return;

        Destroy(joint);
        grabbed = true;
    }
    
    public void Ungrab() {
        grabbed = false;
    }
}
