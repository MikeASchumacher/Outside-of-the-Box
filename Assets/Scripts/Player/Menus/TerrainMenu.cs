using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TerrainMenu : MonoBehaviour {

    public InputField height, uniformScale;
    public Button apply, back;

    public Canvas canvas, prevMenu;

    public TerrainData terrainData;
    public MapGenerator terrain;

    // Use this for initialization
    void Start()
    {
        canvas.GetComponent<Canvas>().enabled = false;

        //set all the buttons
        height.text = terrainData.meshHeightMultiplier.ToString();
        uniformScale.text = terrainData.uniformScale.ToString();

        //set listeners
        apply.onClick.AddListener(Apply);
        back.onClick.AddListener(Back);
    }

    void Back()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        prevMenu.GetComponent<Canvas>().enabled = true;
    }

    void Apply()
    {
        //extract value from all of the inputs
        terrainData.meshHeightMultiplier= int.Parse(height.text);
        terrainData.uniformScale = int.Parse(uniformScale.text);

        canvas.GetComponent<Canvas>().enabled = false;
        prevMenu.GetComponent<Canvas>().enabled = true;
        terrainData.reset = true;
        SceneManager.LoadScene("SampleScene");
    }
}
