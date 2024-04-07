using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenDetection : MonoBehaviour
{

    private float originalRotation = 0;
    private bool open = false;

    public void Start()
    {
        originalRotation = transform.rotation.y;
    }

    public void Update()
    {
        if (IsOpen())
        {
            if (!open)
            {
                open = true;
                GetComponent<StateChange>().Change("");
            }
        }
        else
        {
            open = false;
        }
    }

    public bool IsOpen()
    {
        return transform.rotation.y > originalRotation + 0.5f;
    }
}
