using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class SubmitDish : MonoBehaviour
{
    [SerializeField]
    private GameObject textDisplay;

    [SerializeField]
    private GameObject gameMaster;

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
            textDisplay.GetComponent<Text>().text = "Submit area does not contain a plate!";
            return;
        }

        // Only has plate
        if (inBox.Count == 1)
        {
            textDisplay.GetComponent<Text>().text = "Plate is empty!";
            return;
        }

        // // Has not reached last state
        if (!gameMaster.GetComponent<GameMaster>().IsGameOver()) {
            textDisplay.GetComponent<Text>().text = "You have not completed all steps!";
            return;
        }

        // CalcPlatingScore(plate);
        CalcCuttingScore(plate);

        // Save the plate to the GameMaster reference
        GameObject submit = new("submitted food");
        submit.transform.position = transform.position;
        foreach (GameObject g in inBox)
        {
            var newObj = Instantiate(g);
            newObj.transform.SetParent(submit.transform);
            PhotonNetwork.Destroy(g);
        }

        GameMaster.finalPlating = submit;
        DontDestroyOnLoad(submit);

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("End");
    }

    public void CalcPlatingScore(GameObject plate) {
        // Calculate plating score by getting the average position of objects on the plate
        // Score: 1 - min{(avg distance to center)/(radius of plate), 1} + 0.1
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
        GameMaster.scores["plating"] = 1 - Math.Min(avgDist / plateRadius, 1) + 0.10f;
    }

    public void CalcCuttingScore(GameObject plate) {
        // Calculate based on number of pieces of each item
        Dictionary<string, int> pieces = new();
        Dictionary<string, int> expPieces = new();
        foreach (GameObject g in inBox) {
            if (g.CompareTag("plate") || !g.name.EndsWith("-cut")) continue;

            if (pieces.ContainsKey(g.name)) {
                pieces[g.name] += pieces[g.name] + 1;
            } else {
                pieces[g.name] = 1;
                expPieces[g.name] = g.GetComponent<Ingredient>().expectedPieces;
            }
        }

        if (pieces.Count == 0) return;

        float score = 0f;
        foreach (var entry in pieces) {
            score += Math.Min(1, entry.Value / expPieces[entry.Key]);
        }
        score /= pieces.Count;
        GameMaster.scores["cutting"] = score;
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
