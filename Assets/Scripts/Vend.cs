using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vend : MonoBehaviour
{
    public GameObject[] ingredients;
    private string[] nameList;
    public GameObject spawnArea;

    // Start is called before the first frame update
    void Start()
    {
        nameList = new string[ingredients.Length];
        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i] == null)
            {
                Debug.LogError("The ingredient at index " + i + " is null or undefined!");
                continue;
            }
            if (!ingredients[i].CompareTag("ingredient"))
            {
                Debug.LogError("The ingredient at index " + i + " (name: " + ingredients[i].name + ") is not an ingredient!");
                continue;
            }
            nameList[i] = ingredients[i].name;
            ingredients[i].SetActive(false);
        }
    }

    public void Dispense(string item)
    {
        if (!nameList.Contains(item)) {
            Debug.Log("Invalid vending machine option: " + item);
            return;
        }

        int ind = Array.IndexOf(nameList, item);
        GameObject clone = Instantiate(ingredients[ind], spawnArea.transform.position, spawnArea.transform.rotation);
        clone.name = item;
        clone.SetActive(true);

        GetComponent<StateChange>().Change(item);
    }
}
