using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubmitDish : MonoBehaviour
{
    private readonly HashSet<GameObject> inBox = new();

    public void Submit()
    {
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
            Debug.Log("Submit area does not contain plate!");
            return;
        }

        // Only has plate
        if (inBox.Count == 1)
        {
            Debug.Log("Plate is empty!");
            return;
        }

        // Calculate plating score by getting the average position of objects on the plate
        // Score: 1 - max{(avg distance to center)/(radius of plate), 1} + 0.1
        // This gives min plating 10%
        float totalDist = 0f;
        foreach (GameObject g in inBox)
        {
            if (!g.CompareTag("plate"))
            {
                // Use distance formula ignoring y distance (vertical positioning)
                totalDist += (float)Math.Sqrt(Math.Pow(g.transform.position.x - plate.transform.position.x, 2) + Math.Pow(g.transform.position.z - plate.transform.position.z, 2));
            }
        }
        float plateRadius = plate.GetNamedChild("plate").GetComponent<MeshRenderer>().bounds.size.x / 2;
        float avgDist = totalDist / (inBox.Count - 1);
        Debug.Log(avgDist + " " + plateRadius);
        GameMaster.scores["plating"] = 1 - Math.Max(avgDist / plateRadius, 1) + 0.10f;

        // Save the plate to the GameMaster reference
        GameObject submit = new("submitted food");
        submit.transform.position = transform.position;
        foreach (GameObject g in inBox)
        {
            g.transform.SetParent(submit.transform);
        }

        GameMaster.finalPlating = submit;
        DontDestroyOnLoad(submit);

        SceneManager.LoadScene("End");
    }

    public void OnTriggerEnter(Collider c)
    {
        Debug.Log("trigger enter");
        inBox.Add(c.gameObject);
    }

    public void OnTriggerExit(Collider c)
    {
        Debug.Log("trigger exit");
        inBox.Remove(c.gameObject);
    }
}
