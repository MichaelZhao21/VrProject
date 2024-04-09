using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour
{
    private bool isBaking = false;

    private Color defaultColor;

    private bool coRoutine = false;

    private List<GameObject> insideOven = new List<GameObject>();
    
    void Start(){
        defaultColor = gameObject.GetComponent<MeshRenderer>().materials[1].color;
    }
    void OvenOn(){
        // Change color of oven to red
        gameObject.GetComponent<MeshRenderer>().materials[1].color = Color.yellow;
    }

    void OvenOff(){
        // Change color of oven to black
        gameObject.GetComponent<MeshRenderer>().materials[1].color = defaultColor;
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

    void Update(){
        if (isBaking){
            if (insideOven.Count > 0){
                foreach (GameObject food in insideOven){
                    if (food.tag == "ingredient"){
                        if (!coRoutine)
                        {
                            StartCoroutine(cookFood(food));
                        }
                    }
                }
            }
            
        }
    }

    public void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "ingredient"){
            insideOven.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "ingredient"){
            other.gameObject.GetComponent<Ingredient>().isCooking = false;
            insideOven.Remove(other.gameObject);
        }
        
    }


    IEnumerator cookFood(GameObject food)
    {
        coRoutine = true;
        yield return new WaitForSeconds(1);
        if (food.GetComponent<Ingredient>().CookingPercentage < 150){
            food.GetComponent<Ingredient>().isCooking = true;
            food.GetComponent<Ingredient>().CookingPercentage += 10;
            if (food.GetComponent<Ingredient>().CookingPercentage == 100){
                food.GetComponent<Ingredient>().isCooked = true;
                gameObject.GetComponent<AudioSource>().Play();
            }
            if (food.GetComponent<Ingredient>().CookingPercentage == 150)
            {
                if (food.transform.childCount > 1){
                    GameObject cooked = food.transform.GetChild(1).gameObject;
                    for (int i = 0; i < cooked.transform.childCount; i++)
                    {
                        cooked.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color = Color.black;
                    }
                }
                else
                {
                    for (int i = 0; i < food.GetComponent<MeshRenderer>().materials.Length; i++)
                    {
                        food.GetComponent<MeshRenderer>().materials[i].color = Color.black;
                    }
                }
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
        coRoutine = false;
    }

}
