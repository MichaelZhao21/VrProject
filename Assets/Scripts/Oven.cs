using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Oven : MonoBehaviour
{
    private bool isBaking = false;

    private Color defaultColor;

    private readonly List<GameObject> insideOven = new();

    private PhotonView pv;

    

    void Start()
    {
        // Save the default color of the oven
        defaultColor = gameObject.GetComponent<MeshRenderer>().materials[1].color;
        pv = gameObject.GetComponent<PhotonView>();
    }

    [PunRPC]
    public void ToggleBakingRPC()
    {
        ToggleBaking();
    }

    public void click()
    {
        pv.RPC("ToggleBakingRPC", RpcTarget.AllBuffered);
    }


    public void ToggleBaking()
    {
        isBaking = !isBaking;
        GetComponent<MeshRenderer>().materials[1].color = isBaking ? Color.yellow : defaultColor;
        GetComponent<AudioSource>().enabled = isBaking;
    }

    void Update()
    {
        foreach (GameObject food in insideOven)
        {
            if (isBaking)
                food.GetComponent<Ingredient>().ContinueCooking();
            else
                food.GetComponent<Ingredient>().StopCooking();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ingredient"))
        {
            insideOven.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ingredient"))
        {
            other.GetComponent<Ingredient>().isCooking = false;
            insideOven.Remove(other.gameObject);
        }
    }
}
