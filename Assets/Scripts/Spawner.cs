using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Get the thing to spawn
    [SerializeField]
    [Tooltip("GameObject to spawn if not present in collider area; note this will use the NAME of the object to determine type")]
    private GameObject thingToSpawn;

    private GameObject objRef;

    private readonly HashSet<GameObject> inCollider = new();

    // Start is called before the first frame update
    void Start()
    {
        objRef = Instantiate(thingToSpawn, thingToSpawn.transform.position, thingToSpawn.transform.rotation);
        objRef.name = thingToSpawn.name;
        objRef.SetActive(false);
    }

    void Update()
    {
        // Don't spawn if we find any gameobject in collision area that has same name as spawn object
        foreach (GameObject g in inCollider) {
            if (g.name == objRef.name) {
                return;
            }
        }

        // Otherwise spawn!!!
        Spawn();
    }

    public void Spawn()
    {
        // Duplicate thing to spawn
        GameObject newThing = Instantiate(objRef, objRef.transform.position, objRef.transform.rotation);
        newThing.name = objRef.name;

        // Set the new thing to active
        newThing.SetActive(true);
    }

    public void OnTriggerEnter(Collider c)
    {
        inCollider.Add(c.gameObject);
    }

    public void OnTriggerExit(Collider c)
    {
        inCollider.Remove(c.gameObject);
    }
}
