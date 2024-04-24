using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ContainerPickup : MonoBehaviour
{
    private bool grabbing = false;

    private readonly HashSet<GameObject> onBoard = new();

    public void Grab()
    {
        Debug.Log("GRABBING");
        RemoveNulls();
        foreach (var g in onBoard)
        {
            Debug.Log("g - " + g.name);
            g.transform.SetParent(transform);
        }
        grabbing = true;
    }

    public void Ungrab()
    {
        foreach (var g in onBoard)
        {
            g.transform.parent = null;
        }
        grabbing = false;
    }

    private void RemoveNulls()
    {
        onBoard.RemoveWhere(g => g == null);
    }

    public void OnTriggerEnter(Collider c)
    {
        if (!c.gameObject.CompareTag("ingredient")) return;
        if (grabbing) c.transform.SetParent(transform);
        onBoard.Add(c.gameObject);
    }

    public void OnTriggerExit(Collider c)
    {
        if (!c.gameObject.CompareTag("ingredient")) return;
        if (grabbing) c.transform.parent = null;
        onBoard.Remove(c.gameObject);
    }
}
