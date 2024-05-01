using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class HandDisplays : MonoBehaviourPunCallbacks
{
    public GameObject text;
    public GameObject masterText;

    void Update() {
        text.GetComponent<Text>().text = masterText.GetComponent<Text>().text;
    }
}
