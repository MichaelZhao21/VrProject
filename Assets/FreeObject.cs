using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void freeMe()
    {
        // If object already has a fixed joint, don't add another one
        if (GetComponent<FixedJoint>() != null)
        {
            Destroy(GetComponent<FixedJoint>());
        }
    }
}
