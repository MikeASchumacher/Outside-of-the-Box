using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NoiseMenu : MonoBehaviour {

    public InputField noiseScale, lacunarity, seed, offsetX, offsetY;
    public Slider octaves, persistance;
    public Dropdown normMode;
    public Button apply, back;

    public Canvas canvas, prevMenu;

    public NoiseData noiseData;
    public MapGenerator terrain;

	// Use this for initialization
	void Start () {
        canvas.GetComponent<Canvas>().enabled = false;

        //set all the buttons
        noiseScale.text = noiseData.noiseScale.ToString();
        octaves.value = noiseData.octaves;
        persistance.value = noiseData.persistance;
        lacunarity.text = noiseData.lacunarity.ToString();
        seed.text = noiseData.seed.ToString();
        offsetX.text = noiseData.offset.x.ToString();
        offsetY.text = noiseData.offset.y.ToString();
        normMode.value = 0;

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
        noiseData.noiseScale = int.Parse(noiseScale.text);
        noiseData.octaves = (int)octaves.value;
        noiseData.persistance = persistance.value;
        noiseData.lacunarity = int.Parse(lacunarity.text);
        noiseData.seed = int.Parse(seed.text);
        noiseData.offset = new Vector2(float.Parse(offsetX.text), float.Parse(offsetY.text));
        //noiseData.normMode = 

        canvas.GetComponent<Canvas>().enabled = false;
        prevMenu.GetComponent<Canvas>().enabled = true;
        noiseData.reset = true;
        SceneManager.LoadScene("SampleScene");
    }
	
	
}
