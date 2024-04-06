using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour
{

    // List of food objects that are touching the object
    private List<GameObject> touchingFood = new List<GameObject>();
    
    void Update()
    {


        // Check if to see if the its on a burning bunrer
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("ingredient"))
            {
                // If object already has a fixed joint, don't add another one
                if (collider.GetComponent<FixedJoint>() == null)
                {
                    FixedJoint joint = collider.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = gameObject.GetComponent<Rigidbody>();
                    // Add the object to the touchingFood List
                    touchingFood.Add(collider.gameObject);
                }
            }
            if (collider.CompareTag("burner"))
            {
                // Check if the color of the object is red
                if (collider.GetComponent<Renderer>().material.color == Color.red)
                {
                    // Start a 5 second timer for cooking
                    StartCoroutine(cookFood());
                }
            }
        }
    }

    IEnumerator cookFood()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5);
        // Change the color of the object to black
        foreach (GameObject food in touchingFood)
        {
            food.GetComponent<Renderer>().material.color = Color.black;
        }
    }

}
