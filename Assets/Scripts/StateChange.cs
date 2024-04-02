using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChange : MonoBehaviour
{
    [SerializeField]
    private string stateName;

    [SerializeField]
    private GameObject gameMaster;

    public void Change(string value)
    {
        gameMaster.GetComponent<GameMaster>().ChangeState(stateName, value);
    }
}
