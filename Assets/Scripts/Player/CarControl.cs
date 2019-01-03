using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour {

    float speed = 5;
    float turnSpeed = 180;
    public BoxController player;
    public CameraController camera;
    public MapGenerator mapGen;
    public TerrainData terrainData;
    public NoiseData noiseData;

    public GameObject boxPrefab, self;

    float lastTimeBoxDropped;
    float timeBetweenBoxDrops = 2;

    float timeSinceLastTurn;
    float timeBetweenTurns = 10;

    bool gameEnd = false;
    bool inGame = false;
    bool onGround = false;

    private void Start()
    {

        self.SetActive(false);

        MapData mapData = mapGen.GenerateMapData(new Vector2(0, 0));
        int x = MapGenerator.mapChunkSize / 2;
        int y = MapGenerator.mapChunkSize / 2;
        float height = terrainData.meshHeightCurve.Evaluate(mapData.heightMap[x, y]) * terrainData.meshHeightMultiplier;
        Vector3 pos = transform.TransformPoint(new Vector3(0, height * 2, 0));

        transform.position = pos;

        self.SetActive(true);

        /*MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        foreach (Transform chunk in mapGen.transform)
        {
            if(chunk.position.x == 0 && transform.position.z == 0)
            {
                transform.position = new Vector3(chunk.position.x, chunk.position.y + 10, chunk.position.z);
                break;
            }
        }*/

        lastTimeBoxDropped = Time.time;
    }

    public void TerrainFinish()
    {
        //find our starting point
        /*Vector3 nearestPoint = mapGen.GetComponent<EndlessTerrain>().GetBelowChunk(new Vector2(0, 0)).GetNearestVertex(new Vector3(0, Mathf.Infinity, 0));
        print(nearestPoint.ToString());
        nearestPoint = transform.TransformPoint(nearestPoint);
        transform.position = new Vector3(nearestPoint.x, nearestPoint.y + 2, nearestPoint.z);*/
    }

    public void ControlsOver()
    {
        speed = 7;
    }

    private void Update()
    {

        if (!gameEnd)
        {
            //check if it is time for us to make a turn
            if (timeSinceLastTurn + timeBetweenTurns < Time.time && inGame)
            {
                float rand = Random.Range(-60.0f, 60.0f);
                transform.Rotate(0, rand, 0);
                timeSinceLastTurn = Time.time;
            }


            //get input
            //float steering = Input.GetAxis("Horizontal");
            float gas = 2;

            float moveDistance = gas * speed * Time.deltaTime;

            if (onGround)
                transform.Translate(Vector3.forward * moveDistance);

            if (Time.time - lastTimeBoxDropped > timeBetweenBoxDrops)
            {
                GameObject boxSpawn = Instantiate(boxPrefab, null);
                boxSpawn.transform.position = transform.position + Vector3.up * 2;
                lastTimeBoxDropped = Time.time;
            }
        }
        
    }

    public void BeginGame()
    {
        inGame = true;
    }

    public void GameWin()
    {
        gameEnd = true;
    }

    public void GameLose()
    {
        gameEnd = true;
        inGame = false;
    }

    public void FlyMode()
    {
    }

    public void Unpause()
    {
        Time.timeScale = 1;
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform != boxPrefab.transform)
        {
            onGround = true;
        }
    }
}
