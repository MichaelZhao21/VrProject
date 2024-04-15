using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progress : MonoBehaviour
{
    public float percent = 0;
    public GameObject canvas;

    void Update() {
        if (percent < 0) percent = 0;
        if (percent > 100) percent = 100;

        GetComponent<RectTransform>().offsetMax = new Vector2(-2 - (100 - percent), -2);
    }

    public void UpdateProgress(float amt) {
        percent = amt;
        canvas.GetComponent<Canvas>().enabled = true;
    }

    public void Hide() {
        canvas.GetComponent<Canvas>().enabled = false;
    }
}
