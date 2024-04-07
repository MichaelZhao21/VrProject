using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubmitDish : MonoBehaviour
{
    private List<GameObject> inBox = new();

    public void Submit()
    {
        SceneManager.LoadScene("End");
        return;

        // Make sure plate is in the submit area
        bool hasPlate = false;
        GameObject plate = null;
        foreach (GameObject g in inBox)
        {
            if (g.CompareTag("plate"))
            {
                hasPlate = true;
                plate = g;
                break;
            }
        }

        // Does not have plate
        if (!hasPlate)
        {
            return;
        }

        // Calculate plating score by getting the average position of objects on the plate
        // Score: 1 - (distance to center)/(radius of plate)
        float totalDist = 0f;
        foreach (GameObject g in inBox)
        {
            if (!g.CompareTag("plate"))
            {
                // Use distance formula ignoring y distance (vertical positioning)
                totalDist += (float)Math.Sqrt(Math.Pow(g.transform.position.x - plate.transform.position.x, 2) + (Math.Pow(g.transform.position.z - plate.transform.position.z, 2)));
            }
        }
        GameMaster.scores["plating"] = totalDist / (inBox.Count-1);
    }

    public void OnTriggerEnter(Collider c)
    {
        inBox.Add(c.gameObject);
    }

    public void OnTriggerExit(Collider c)
    {
        inBox.Remove(c.gameObject);
    }
}
