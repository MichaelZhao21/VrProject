using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour
{
    private readonly HashSet<GameObject> inBox = new();

    bool coRoutine = false;
    void Update()
    {
        // Check if to see if the its on a burning bunrer
        foreach (var c in inBox)
        {
            if (c.CompareTag("ingredient"))
            {
                // If object already has a fixed joint, don't add another one
                if (c.GetComponent<FixedJoint>() == null)
                {
                    FixedJoint joint = c.AddComponent<FixedJoint>();
                    joint.connectedBody = gameObject.GetComponent<Rigidbody>();
                }
            }
            if (c.CompareTag("burner"))
            {
                // Check if the color of the object is red
                if (c.GetComponent<Burner>().isBurning == true)
                {
                    print("first part");
                    if (!coRoutine)
                    {
                        StartCoroutine(cookFood());
                    }
                }
            }
        }
    }

    IEnumerator cookFood()
    {
        coRoutine = true;
        foreach (GameObject food in inBox)
        {
            if (!food.CompareTag("ingredient")) continue;
            print("second part");
            yield return new WaitForSeconds(1);
            // If the object is not already cooking, start cooking it
            if (food.GetComponent<Ingredient>().CookingPercentage < 100)
            {
                food.GetComponent<Ingredient>().isCooking = true;
                food.GetComponent<Ingredient>().CookingPercentage += 10;
                if (food.GetComponent<Ingredient>().CookingPercentage == 100)
                {
                    food.GetComponent<Ingredient>().isCooked = true;
                    gameObject.GetComponent<AudioSource>().Play();

                }
            }
        }
        coRoutine = false;
    }

    public void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("ingredient") || c.gameObject.CompareTag("burner"))
            inBox.Add(c.gameObject);
    }

    public void OnCollisionExit(Collision c)
    {
        if (c.gameObject.CompareTag("ingredient") || c.gameObject.CompareTag("burner"))
            inBox.Remove(c.gameObject);
    }


}
