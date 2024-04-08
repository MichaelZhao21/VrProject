using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChange : MonoBehaviour
{
    public string stateName = "";

    public GameObject gameMaster;

    public void Change(string value)
    {
        gameMaster.GetComponent<GameMaster>().ChangeState(stateName, value);
    }
}
