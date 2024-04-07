using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameMaster : MonoBehaviour
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

    [SerializeField]
    [Tooltip("The file name of the state file in the Assets/Resources folder.")]
    private string stateFile;

    [SerializeField]
    private GameObject textBox;

    // static variables
    public static Dictionary<string, float> scores = new();
    
    public static GameObject finalPlating;

    public void Start()
    {
        // Read text from files
        TextAsset data = Resources.Load(stateFile) as TextAsset;
        if (data == null) {
            Debug.LogError("Invalid name for stateFile (does not exist): " + stateFile);
            return;
        }

        string[] lines = data.text.Split('\n').Where(c => !c.Trim().StartsWith("#") && c.Trim() != "").ToArray();

        // Parse all items
        stateList = new StateItem[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(' ');

            if (parts.Length >= 2) {
                stateList[i] = new StateItem(parts[0], parts[1]);
            } else {
                stateList[i] = new StateItem(parts[0], "");
            }
        }

        UpdateUI();
    }

    public void ChangeState(string stateName, string value = "")
    {
        Debug.Log(stateName + " " + value + " " + state + " " + stateList.Length);
        // End of state
        if (state == stateList.Length - 1)
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

    public void UpdateUI() {
        // Show debug message if needed
        if (textBox != null)
        {
            string currentState = state == -1 ? "Start" : stateList[state].name;
            string nextState = state == stateList.Length - 1 ? "End" : stateList[state + 1].name;
            string currentValue = state == -1 ? "" : stateList[state].value == "" ? "" : String.Format(" [{0}]", stateList[state].value);
            string nextValue = state == stateList.Length - 1 ? "" : stateList[state + 1].value == "" ? "" : String.Format(" [{0}]", stateList[state + 1].value);
            string stateMsg = string.Format("Next Task: {2}{3}\n\n(previous: {0}{1})", currentState, currentValue, nextState, nextValue);
            textBox.GetComponent<UnityEngine.UI.Text>().text = stateMsg;
        }
    }

    public static float CalculateAverageScore() {
        // Calculate score for dish and show final menu
        float totalScore = 0f;
        foreach(float v in scores.Values) {
            totalScore += v;
        }
        float avgScore = totalScore / scores.Count;
        return avgScore;
    }
}
