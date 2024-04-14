using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    private readonly HashSet<GameObject> touching = new();

    private bool isCooking = false;

    void Update()
    {
        // Check if to see if the its on a burning bunrer
        foreach (var c in touching)
        {
            // Set cooking state based on cooking state
            if (c.CompareTag("ingredient"))
            {
                if (isCooking)
                    c.GetComponent<Ingredient>().ContinueCooking();
                else
                    c.GetComponent<Ingredient>().StopCooking();
            }

            // Set cooking to true if burner is on
            if (c.CompareTag("burner"))
            {
                if (c.GetComponent<Burner>().isBurning)
                {
                    isCooking = true;
                    GetComponent<AudioSource>().enabled = true;
                }
                else
                {
                    isCooking = false;
                    GetComponent<AudioSource>().enabled = false;
                }
            }
        }
    }

    public void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("ingredient") || c.gameObject.CompareTag("burner"))
            touching.Add(c.gameObject);
    }

    public void OnCollisionExit(Collision c)
    {
        if (c.gameObject.CompareTag("burner") && isCooking)
        {
            isCooking = false;
            GetComponent<AudioSource>().enabled = false;
        }

        if (c.gameObject.CompareTag("ingredient") || c.gameObject.CompareTag("burner"))
            touching.Remove(c.gameObject);
    }


}
