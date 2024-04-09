using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public bool grabbed = false;

    public GameObject cookedversion;
    
    public float minVolume = 0f;
    
    [Tooltip("Expected number of pieces this ingredient should be cut in; score is max{1, pieces/expectedPieces}")]
    public int expectedPieces = 5;

    public float CookingPercentage = 0;
    public bool isCooked = false;
    public bool isCooking = false;

    public bool isDone = false;

    [Tooltip("Material for inside of mesh when cut")]
    public Material innerMaterial;

    void Awake(){
        cookedversion = GameObject.Find(gameObject.name + "Cooked");
    }

    void Update(){
        if (isCooked && !isDone){
            GameObject cooked = Instantiate(cookedversion, transform.position, transform.rotation);
            cooked.SetActive(true);
            cooked.GetComponent<Ingredient>().isCooked = true;
            cooked.GetComponent<Ingredient>().isDone = true;
            cooked.GetComponent<Ingredient>().CookingPercentage = CookingPercentage;
            gameObject.SetActive(false);
            isDone = true;
        }
    }
    public void Grab() {
        gameObject.GetComponent<StateChange>().Change("");
        grabbed = true;

        // Don't do if no joint
        if (!gameObject.TryGetComponent<FixedJoint>(out var joint)) return;
        Destroy(joint);
    }
    
    public void Ungrab() {
        grabbed = false;
    }
}
