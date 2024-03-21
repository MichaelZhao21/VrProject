using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Get spawner game object
    GameObject spawnerLocation;

    // Get the thing to spawn
    public GameObject thingToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        // Get spawner game object
        spawnerLocation = this.gameObject;
    }

    public void Spawn()
    {
        // Duplicate thing to spawn
        GameObject newThing = Instantiate(thingToSpawn, spawnerLocation.transform.position, spawnerLocation.transform.rotation);

        // Set the parent of the new thing to the spawner
        newThing.transform.parent = spawnerLocation.transform;

        // Set the new thing to active
        newThing.SetActive(true);
    }


}
