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

    // If need to change more than one thing
    public void Change(string otherName, string value)
    {
        gameMaster.GetComponent<GameMaster>().ChangeState(otherName, value);
    }
}
