using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CuttingBoardPickup : MonoBehaviour
{
    public GameObject fixArea;

    private bool grabbing = false;

    private bool isFixed = false;

    private float initialRotationX;

    private float initialRotationZ;

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
        var initialRotEuler = transform.rotation.eulerAngles;
        initialRotationX = initialRotEuler.x;
        initialRotationZ = initialRotEuler.z;
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
            if (!g.CompareTag("ingredient")) continue;
            if (!g.TryGetComponent<FixedJoint>(out var joint)) continue;
            Destroy(joint);
        }
        isFixed = false;
    }

    private void Affix()
    {
        RemoveNulls();
        foreach (var g in onBoard)
        {
            if (!g.CompareTag("ingredient")) continue;
            FixedJoint joint = g.AddComponent<FixedJoint>();
            joint.connectedBody = gameObject.GetComponent<Rigidbody>();
        }
        isFixed = true;
    }

    private bool IsRotated()
    {
        return Math.Abs(initialRotationX - transform.rotation.eulerAngles.x) > 60 ||
            Math.Abs(initialRotationZ - transform.rotation.eulerAngles.z) > 60;
    }

    private void RemoveNulls()
    {
        onBoard.RemoveWhere(g => g == null);
    }

    public void OnTriggerEnter(Collider c)
    {
        onBoard.Add(c.gameObject);
    }

    public void OnTriggerExit(Collider c)
    {
        onBoard.Remove(c.gameObject);
    }
}
