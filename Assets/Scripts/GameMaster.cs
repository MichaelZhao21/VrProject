using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class GameMaster : MonoBehaviourPunCallbacks
{
    public class StateItem
    {
        public string name;
        public string value;
        public StateItem(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }

    // Track the current state
    private int state = -1;

    private StateItem[] stateList;

    public static string stateFile = "watermelon";

    public static Dictionary<string, string> textMap = new();

    [SerializeField]
    private GameObject textBox;

    // static variables
    public static Dictionary<string, float> scores = new();

    public static GameObject finalPlating;

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("stateFile"))
        {
            stateFile = (string)PhotonNetwork.CurrentRoom.CustomProperties["stateFile"];
            Debug.Log("Recipe: " + stateFile);
        }

        // Read text from files
        TextAsset data = Resources.Load(stateFile) as TextAsset;
        if (data == null)
        {
            Debug.LogError("Invalid name for stateFile (does not exist): " + stateFile);
            return;
        }

        string[] lines = data.text.Split('\n').Where(c => !c.Trim().StartsWith("#") && c.Trim() != "").ToArray();

        // Parse all items
        stateList = new StateItem[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Trim().Split(' ');

            if (parts.Length >= 2)
            {
                stateList[i] = new StateItem(parts[0], parts[1]);
            }
            else
            {
                stateList[i] = new StateItem(parts[0], "");
            }
        }

        UpdateUI();
    }

    public void ChangeState(string stateName, string value = "")
    {
        // End of state
        if (IsGameOver())
        {
            return;
        }

        // Not valid state
        if (stateName != stateList[state + 1].name || (value != "" && value != stateList[state + 1].value))
        {
            return;
        }

        // If all conditions pass, increment state
        state++;

        UpdateUI();
    }

    public void UpdateUI()
    {
        // Show debug message if needed
        if (textBox != null)
        {
            string currentState = state == -1 ? "N/A" : GetDisplay(stateList[state].name);
            string nextState = state == stateList.Length - 1 ? "Submit Dish!" : GetDisplay(stateList[state + 1].name);
            string currentValue = state == -1 ? "" : stateList[state].value == "" ? "" : String.Format(" [{0}]", stateList[state].value);
            string nextValue = state == stateList.Length - 1 ? "" : stateList[state + 1].value == "" ? "" : String.Format(" [{0}]", stateList[state + 1].value);
            string stateMsg = string.Format("Next Task: {2}{3}\n\n(Last Completed: {0}{1})", currentState, currentValue, nextState, nextValue);
            textBox.GetComponent<UnityEngine.UI.Text>().text = stateMsg;
        }
    }

    public static float CalculateAverageScore()
    {
        if (scores.Count == 0) return 0;

        Debug.Log(scores.Count);

        // Calculate score for dish and show final menu
        float totalScore = 0f;
        foreach (float v in scores.Values)
        {
            totalScore += v;
        }
        float avgScore = totalScore / scores.Count;
        return avgScore;
    }

    public bool IsGameOver()
    {
        return state == stateList.Length - 1;
    }

    public static void LoadTextMap()
    {
        TextAsset data = Resources.Load("word-map") as TextAsset;
        string[] lines = data.text.Split('\n').Where(c => !c.Trim().StartsWith("#") && c.Trim() != "").ToArray();
        foreach (var line in lines)
        {
            string[] parts = line.Trim().Split(' ');
            string val = line[(parts[0].Length + 1)..];
            textMap.Add(parts[0], val);
        }
    }

    public static string GetDisplay(string key)
    {
        if (textMap.Count == 0) LoadTextMap();

        if (!textMap.ContainsKey(key)) return key;
        return textMap[key];
    }
}
