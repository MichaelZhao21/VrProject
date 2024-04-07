using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject foodPlaceholder;

    [SerializeField]
    private GameObject scoreText;

    public void Start()
    {
        GameObject finalPlateDisplay = Instantiate(GameMaster.finalPlating);
        finalPlateDisplay.transform.SetPositionAndRotation(foodPlaceholder.transform.position, Quaternion.Euler(0, 180, 0));

        Destroy(GameMaster.finalPlating);

        float finalScore = GameMaster.CalculateAverageScore();
        scoreText.GetComponent<Text>().text = string.Format("Your Score: {0:F2}%", finalScore * 100);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
