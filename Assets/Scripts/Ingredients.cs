using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredients : MonoBehaviour
{
    public float CookingPercentage = 0;
    public bool isCooked = false;
    public bool isCooking = false;
    // Start is called before the first frame update

    public void freeMe()
    {
        // If object already has a fixed joint, don't add another one
        if (GetComponent<FixedJoint>() != null)
        {
            Destroy(GetComponent<FixedJoint>());
        }
    }

    
}
