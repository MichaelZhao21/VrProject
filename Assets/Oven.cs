using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour
{
    private bool isBaking = false;

    private Color defaultColor;

    private bool coRoutine = false;
    
    void Start(){
        defaultColor = gameObject.GetComponent<Renderer>().materials[1].color;
    }
    void OvenOn(){
        // Change color of oven to red
        gameObject.GetComponent<Renderer>().materials[1].color = Color.yellow;
    }

    void OvenOff(){
        // Change color of oven to black
        gameObject.GetComponent<Renderer>().materials[1].color = defaultColor;
    }

    // Update is called once per frame
    public void CallScript()
    {
        if (isBaking == false)
        {
            OvenOn();
            isBaking = true;
        }
        else
        {
            OvenOff();
            isBaking = false;
        }
    }

    public void OnTriggerEnter(Collider other){
        if (isBaking){
            if (other.gameObject.tag == "ingredient"){
                if (!coRoutine)
                {
                    StartCoroutine(cookFood(other.gameObject));
                }
            }
        }
    }

    IEnumerator cookFood(GameObject food)
    {
        coRoutine = true;
        yield return new WaitForSeconds(5);
        food.GetComponent<Ingredient>().CookingPercentage = 100;
        food.GetComponent<Renderer>().material.color = Color.black;
        food.GetComponent<Ingredient>().isCooked = true;
        gameObject.GetComponent<AudioSource>().Play();
        coRoutine = false;
    }

}
