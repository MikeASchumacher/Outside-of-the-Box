using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public BoxController player;
    public CarControl car;
    public Canvas canvas;
    public Button resume, leave;

    public void Start()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        resume.onClick.AddListener(Resume);
        leave.onClick.AddListener(Leave);
    }

    void Leave()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        player.Unpause();
        car.Unpause();
        SceneManager.LoadScene("SampleScene");
    }

    void Resume()
    {
        player.Unpause();
        car.Unpause();
    }
}
