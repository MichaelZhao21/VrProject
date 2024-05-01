using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private InputActionReference menuInputActionReference;

    [SerializeField]
    private GameObject canvasRef;

    [SerializeField]
    private GameObject cameraRef;

    [SerializeField]
    private GameObject leftRayRef;

    [SerializeField]
    private GameObject rightRayRef;

    private bool shown = false;

    void OnEnable()
    {
        menuInputActionReference.action.performed += TogglePauseMenu;
    }

    private void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (shown)
        {
            canvasRef.SetActive(false);
        }
        else
        {
            canvasRef.SetActive(true);

            // Place in front of the camera, level in the xz plane with the camera position
            canvasRef.transform.position = cameraRef.transform.position + new Vector3(cameraRef.transform.forward.x, 0, cameraRef.transform.forward.z);

            // Set rotation to camera rotation except always upright
            canvasRef.transform.rotation = Quaternion.Euler(0, cameraRef.transform.rotation.eulerAngles.y, 0);
        }

        shown = !shown;
    }

    // Update is called once per frame
    void Update()
    {
        if (!shown)
        {
            leftRayRef.SetActive(false);
            rightRayRef.SetActive(false);
            return;
        }

        // Activate the rays
        leftRayRef.SetActive(true);
        rightRayRef.SetActive(true);
    }

    public void CloseMenu()
    {
        canvasRef.SetActive(false);
        shown = false;
    }

    public void BackToMenu() {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }
}
