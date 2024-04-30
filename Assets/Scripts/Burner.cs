using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Burner : MonoBehaviour
{
    public bool isBurning = false;

    private Color defaultColor;

    private PhotonView pv;

    void Start()
    {
        // Save the default color of the burner
        defaultColor = GetComponent<MeshRenderer>().material.color;
        pv = GetComponent<PhotonView>();
        
    }

    // Update is called once per frame
    [PunRPC]
    public void ToggleBurningRPC()
    {
        ToggleBurning();
    }

    public void click(){
        pv.RPC("ToggleBurningRPC", RpcTarget.AllBuffered);
    }


    public void ToggleBurning()
    {
        isBurning = !isBurning;
        GetComponent<Renderer>().material.color = isBurning ? Color.red : defaultColor;
    }
}
