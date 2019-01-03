using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour {

    public Button start, leave, options, fly;
    public CarControl truck;
    public BoxController player;
    public CameraController camera;
    public Canvas canvas, optionsMenu;

    public void Start()
    {
        start.onClick.AddListener(StartGame);
        leave.onClick.AddListener(EndGame);
        options.onClick.AddListener(OptionMenu);
        fly.onClick.AddListener(FlyMode);
    }

    void StartGame()
    {
        truck.BeginGame();
        player.BeginGame();
        camera.BeginGame();

        canvas.GetComponent<Canvas>().enabled = false;
    }

    void EndGame()
    {
        Application.Quit();
    }

    void OptionMenu()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        optionsMenu.GetComponent<Canvas>().enabled = true;
    }

    void FlyMode()
    {
        truck.FlyMode();
        player.FlyMode();
        camera.FlyMode();

        canvas.GetComponent<Canvas>().enabled = false;
    }
}
