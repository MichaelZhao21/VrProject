using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public bool grabbed = false;
    
    public float minVolume = 0.5f;

    public float CookingPercentage = 0;
    public bool isCooked = false;
    public bool isCooking = false;

    [Tooltip("Material for inside of mesh when cut")]
    public Material innerMaterial;

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
