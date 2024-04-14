using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenDoor : MonoBehaviour
{
    void Start() {
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    public void Grabbed()
    {
        GetComponent<Rigidbody>().freezeRotation = false;
    }

    public void Ungrabbed()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
    }
}
