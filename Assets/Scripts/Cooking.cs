using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour
{

    // List of food objects that are touching the object
    private List<GameObject> touchingFood = new List<GameObject>();
    
    bool coRoutine = false;
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
                    if (!coRoutine)
                    {
                        StartCoroutine(cookFood());
                    }
                }
            }else{
                foreach (GameObject food in touchingFood)
                {
                    // If the object is not already cooking, start cooking it
                    if (food.GetComponent<Ingredients>().isCooking)
                    {
                        food.GetComponent<Ingredients>().isCooking = false;
                    }
                }
            
            }
        }
    }

    IEnumerator cookFood()
    {
        coRoutine = true;
        foreach (GameObject food in touchingFood)
        {
            // If the object is not already cooking, start cooking it
            if (food.GetComponent<Ingredients>().CookingPercentage < 120)
            {
                food.GetComponent<Ingredients>().isCooking = true;
                yield return new WaitForSeconds(2);
                food.GetComponent<Ingredients>().CookingPercentage += 10;
                if (food.GetComponent<Ingredients>().CookingPercentage == 100)
                {
                    food.GetComponent<Renderer>().material.color = Color.black;
                    food.GetComponent<Ingredients>().isCooked = true;
                    gameObject.GetComponent<AudioSource>().Play();

                }
                if (food.GetComponent<Ingredients>().CookingPercentage >= 120)
                {
                    food.GetComponent<Renderer>().material.color = Color.red;
                    gameObject.GetComponent<AudioSource>().Play();
                    food.GetComponent<Ingredients>().isCooking = false;
                }
            }
        }
        coRoutine = false;
    }



}
