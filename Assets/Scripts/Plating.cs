using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plating : MonoBehaviour
{
    void OnCollisionStay(Collision collision)
    {
        // Ignore everythign that isn't an ingredient
        if (!collision.gameObject.CompareTag("ingredient")) return;

        // Ignore everything that doesn't have the ingredient script
        if (!collision.gameObject.TryGetComponent<Ingredient>(out var ing)) return;

        // Ignore everything that is being grabbed
        if (ing.grabbed) return;

        // Ignore everything that already has a fixed joint
        if (collision.gameObject.GetComponent<FixedJoint>() != null) return;

        // Add a fixed joint to that ingredient to this plate
        FixedJoint joint = collision.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = gameObject.GetComponent<Rigidbody>();

        // Update state
        if (!gameObject.TryGetComponent<StateChange>(out var sc)) return;
        sc.Change(collision.gameObject.name);
    }
}
