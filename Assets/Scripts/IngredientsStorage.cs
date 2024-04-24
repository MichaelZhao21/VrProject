using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientsStorage : MonoBehaviour
{
    void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        // Get the center and size of the box collider
        Vector3 center = boxCollider.center + transform.position;
        Vector3 size = boxCollider.size;

        // Define an array to store overlapping colliders
        Collider[] overlappingColliders = Physics.OverlapBox(center, size / 2, Quaternion.Euler(0, 90, 0));

        // Loop through overlapping colliders and do something with them
        foreach (Collider collider in overlappingColliders)
        {
            Debug.Log("Found object: " + collider.gameObject.name);
        }
    }
}
