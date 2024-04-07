using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public Boolean grabbed;

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
