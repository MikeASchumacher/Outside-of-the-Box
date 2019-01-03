using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public Button noise, terrain, back;
    public Canvas canvas, noiseMenu, terrainMenu, prevMenu;

    public void Start()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        noise.onClick.AddListener(NoiseOptions);
        terrain.onClick.AddListener(TerrainOptions);
        back.onClick.AddListener(Leave);
    }

    void NoiseOptions()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        noiseMenu.GetComponent<Canvas>().enabled = true;
    }

    void TerrainOptions()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        terrainMenu.GetComponent<Canvas>().enabled = true;
    }

    void Leave()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        prevMenu.GetComponent<Canvas>().enabled = true;
    }
}
