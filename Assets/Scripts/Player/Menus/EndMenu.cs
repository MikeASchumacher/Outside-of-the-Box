using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour {

    public Button ret;
    public Canvas canvas;
    public Text win, lose;

    public void Start()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        ret.onClick.AddListener(Leave);

        win.enabled = false;
        lose.enabled = false;
    }

    public void Win()
    {
        canvas.GetComponent<Canvas>().enabled = true;
        win.enabled = true;
    }

    public void Lose(bool water)
    {
        if (water)
            lose.text = "Boxes Can't Get Wet!\nYou Didn't Make It Back!";

        canvas.GetComponent<Canvas>().enabled = true;
        lose.enabled = true;
    }

    void Leave()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene("SampleScene");
    }
}
