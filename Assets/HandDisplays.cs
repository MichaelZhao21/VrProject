using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class HandDisplays : MonoBehaviourPunCallbacks
{
    public GameObject text;
    public GameObject panel;
    public GameObject masterText;
    public GameObject mainCamera;

    void Update() {
        text.GetComponent<Text>().text = masterText.GetComponent<Text>().text;

        if (Vector3.Distance(mainCamera.transform.position, transform.position) < 0.4f) {
            panel.SetActive(true);
        } else {
            panel.SetActive(false);
        }
    }
}
