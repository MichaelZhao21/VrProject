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
        textDisplay.GetComponent<Text>().text = GameMaster.GetDisplay(recipeList[selected]);
    }

    public void StartGame()
    {
        GameMaster.stateFile = recipeList[selected];
        SceneManager.LoadScene("MP test 2");
        
        // NetworkManager network = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
        // // network.ConnectToServer();
        // network.InitializeRoom(0);

    }

    public void StartMultiplayerSession(){
        GameMaster.stateFile = recipeList[selected];

        NetworkManager network = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
        // network.ConnectToServer();
        network.InitializeRoom(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
