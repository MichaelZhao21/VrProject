using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CuttingBoardPickup : MonoBehaviour
{
    private bool grabbing = false;

    private bool isFixed = false;

    private float initialRotationX;

    private readonly HashSet<GameObject> onBoard = new();

    public void Grab()
    {
        grabbing = true;
    }

    public void Ungrab()
    {
        grabbing = false;
    }

    public void Start()
    {
        initialRotationX = transform.rotation.eulerAngles.x;
    }

    public void Update()
    {
        // If not grabbing and is fixed, unfix it
        // Otherwise ignore the board
        if (!grabbing)
        {
            if (isFixed) Unfix();
            return;
        }

        if (IsRotated())
        {
            // Ignore if is rotated and not fixed
            if (!isFixed) return;

            // Otherwise, unfix it
            Unfix();
        }
        else if (!isFixed)
        {
            // If not rotated and not fixed, affix it
            Affix();
        }
    }

    private void Unfix()
    {
        RemoveNulls();
        foreach (var g in onBoard)
        {
            if (!g.TryGetComponent<FixedJoint>(out var joint)) continue;
            Destroy(joint);
            g.GetComponent<Rigidbody>().AddForce(Vector3.up * 3, ForceMode.Impulse);
        }
        isFixed = false;
    }

    private void Affix()
    {
        RemoveNulls();
        foreach (var g in onBoard)
        {
            FixedJoint joint = g.AddComponent<FixedJoint>();
            joint.connectedBody = gameObject.GetComponent<Rigidbody>();
        }
        isFixed = true;
    }

    private bool IsRotated()
    {
        return Math.Abs(initialRotationX - transform.rotation.eulerAngles.x) > 30;
    }

    private void RemoveNulls()
    {
        onBoard.RemoveWhere(g => g == null);
    }

    public void OnTriggerEnter(Collider c)
    {
        if (!c.gameObject.CompareTag("ingredient")) return;
        onBoard.Add(c.gameObject);
    }

    public void OnTriggerExit(Collider c)
    {
        if (!c.gameObject.CompareTag("ingredient")) return;
        onBoard.Remove(c.gameObject);
    }
}
