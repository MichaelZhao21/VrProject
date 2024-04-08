using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private string[] recipeList;
    private int selected = 0;

    [SerializeField]
    private GameObject textDisplay;

    public void Start()
    {
        // Load recipes from text file
        TextAsset data = Resources.Load("Recipes") as TextAsset;

        // Ignore all lines that are comments (# foo) or only whitespace/blank
        recipeList = data.text.Split('\n').Where(c => !c.Trim().StartsWith("#") && c.Trim() != "").ToArray();

        // Show the first recipe
        UpdateDisplay();
    }

    public void OptionLeft()
    {
        if (selected == 0) selected = recipeList.Length - 1;
        else selected--;
        UpdateDisplay();
    }

    public void OptionRight()
    {
        if (selected == recipeList.Length - 1) selected = 0;
        else selected++;
        UpdateDisplay();

    }

    private void UpdateDisplay()
    {
        textDisplay.GetComponent<Text>().text = recipeList[selected];
    }

    public void StartGame()
    {
        GameMaster.stateFile = recipeList[selected];
        SceneManager.LoadScene("Kitchen");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
