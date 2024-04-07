using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuAction : MonoBehaviour
{
    public void StartGame() {
        SceneManager.LoadScene("Kitchen");
    }
}
