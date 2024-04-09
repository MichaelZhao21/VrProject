using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burner : MonoBehaviour
{

    [SerializeField] public GameObject burner;
    private bool isBurning = false;
    
    // Start is called before the first frame update
    void BurnerOn(){
        // Change color of burner to red
        burner.GetComponent<Renderer>().material.color = Color.red;
    }

    void BurnerOff(){
        // Change color of burner to grey
        burner.GetComponent<Renderer>().material.color = new Color(0.13f, 0.13f, 0.13f);
    }

    // Update is called once per frame
    public void CallScript()
    {
        if (isBurning == false)
        {
            BurnerOn();
            isBurning = true;
        }
        else
        {
            BurnerOff();
            isBurning = false;
        }
    }
}
