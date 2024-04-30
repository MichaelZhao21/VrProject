using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burner : MonoBehaviour
{
    public bool isBurning = false;

    private Color defaultColor;

    void Start()
    {
        // Save the default color of the burner
        defaultColor = GetComponent<MeshRenderer>().material.color;
        
    }

    // Update is called once per frame
    public void ToggleBurning()
    {
        isBurning = !isBurning;
        GetComponent<Renderer>().material.color = isBurning ? Color.red : defaultColor;
    }
}
