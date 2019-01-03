using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlane : MonoBehaviour {

    public GameObject player;
    public TextureData textureData;
    public TerrainData terrainData;
    float waterHeight = 0;

	// Use this for initialization
	void Start () {
        waterHeight = terrainData.meshHeightCurve.Evaluate(textureData.layers[1].startHeight) * terrainData.meshHeightMultiplier;
        waterHeight *= terrainData.uniformScale;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(player.transform.position.x, waterHeight, player.transform.position.z);
	}
}
