using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{

    public GameObject playerObject;
    public GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Cook");
        cam = GameObject.Find("Main Camera");

        // -1.36144f

        playerObject.transform.position = new Vector3(cam.transform.position.x, -0.2f, cam.transform.position.z);
        playerObject.transform.SetParent(cam.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
