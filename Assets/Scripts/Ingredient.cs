using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public enum CookingState { RAW, PARTIAL, COOKED, OVERCOOKED }

    public bool grabbed = false;

    [Tooltip("Minimum volume to allow to be cut, set to 0 for infinite thinness")]
    public float minVolume = 0f;

    [Tooltip("Expected number of pieces this ingredient should be cut in; score is max{1, pieces/expectedPieces}")]
    public int expectedPieces = 5;

    [Tooltip("Total amount of time item should be cooking for")]
    public float totalCookingTime = 10f;

    [Tooltip("Amount of time it takes for the item to be overcooked after total cooking time has finished")]
    public float overcookedTime = 5f;

    public float cookingPercent = 0f;
    public float currentCookingTime = 0f;
    public CookingState cookingState = CookingState.RAW;
    public bool isCooking = false;

    [Tooltip("Material for inside of mesh when cut")]
    public Material innerMaterial;

    public void Grab()
    {
        gameObject.GetComponent<StateChange>().Change(name);
        grabbed = true;

        // Don't do if no joint
        if (!gameObject.TryGetComponent<FixedJoint>(out var joint)) return;
        Destroy(joint);
    }

    public void Ungrab()
    {
        grabbed = false;
    }

    public void ContinueCooking()
    {
        // Start cooking if not already
        if (!isCooking)
        {
            isCooking = true;
            return;
        }

        // Continue cooking
        currentCookingTime += Time.deltaTime;
        cookingPercent = currentCookingTime / totalCookingTime;
        Debug.Log(name + " " + cookingPercent + " - " + currentCookingTime);

        // Check for raw
        if (cookingState < CookingState.PARTIAL)
        {
            cookingState = CookingState.PARTIAL;
            return;
        }

        // If partially cooked
        if (currentCookingTime < totalCookingTime)
        {
            GameMaster.scores["cooking"] = cookingPercent;
        }

        // Check for done
        if (cookingState < CookingState.COOKED && currentCookingTime >= totalCookingTime)
        {
            gameObject.GetComponent<StateChange>().Change("cook", gameObject.name);
            GameMaster.scores["cooking"] = 1f;
            cookingState = CookingState.COOKED;
            gameObject.GetComponent<AudioSource>().Play();

            // Change the mesh materials
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            return;
        }

        // Check for burnt
        if (cookingState < CookingState.OVERCOOKED && currentCookingTime >= totalCookingTime + overcookedTime)
        {
            cookingState = CookingState.OVERCOOKED;
            GameMaster.scores["cooking"] = 0f;

            // Change color of mesh renderer materials
            if (transform.childCount > 1)
            {
                var cooked = transform.GetChild(1);
                for (int i = 0; i < cooked.transform.childCount; i++)
                {
                    cooked.GetChild(i).GetComponent<Renderer>().material.color = Color.black;
                }
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                var mr = GetComponent<MeshRenderer>();
                for (int i = 0; i < mr.materials.Length; i++)
                {
                    mr.materials[i].color = Color.black;
                }
            }
        }
    }

    public void StopCooking()
    {
        // Ignore if not cooking
        if (!isCooking)
        {
            return;
        }

        // Stop the cooking
        isCooking = false;
    }
}
