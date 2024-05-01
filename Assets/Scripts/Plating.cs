using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plating : MonoBehaviour
{
    void OnCollisionStay(Collision collision)
    {
        // Update state
        if (!gameObject.TryGetComponent<StateChange>(out var sc)) return;
        sc.Change(collision.gameObject.name.Replace("(Clone)", "").Trim());
    }
}
