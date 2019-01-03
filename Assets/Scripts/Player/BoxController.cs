using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoxController : MonoBehaviour {

    enum MODE { REGULAR, FLY};
    private MODE mode;

    private Rigidbody rb;

    public float speed;
    public GameObject camera;
    public GameObject player;
    public GameObject boxPrefab;
    public GameObject truck;
    CameraController cameraScript;

    public CarControl car;

    float gravity = 9.8f;
    bool inAir = false;
    public float verticalOffset = 0;
    public float verticalSpeed = 0;
    float jumpImpulse = 15;

    bool gameStart = false, gameEnd = false;

    public Text controls;
    public Canvas endScreen, pauseMenu;
    float timeText;
    int controlShowTime = 5;

    public TerrainData terrainData;
    public TextureData textureData;
    float waterHeight = 0;

    float timer;
    float timerLength = 50;
    bool timerStart = false;
    public Text timeLeft, flyControls;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraScript = (CameraController)camera.GetComponent(typeof(CameraController));
        player.SetActive(false);

        controls.enabled = false;

        waterHeight = terrainData.meshHeightCurve.Evaluate(textureData.layers[1].startHeight) * terrainData.meshHeightMultiplier;
        waterHeight *= terrainData.uniformScale;

        timeLeft.enabled = false;
        flyControls.enabled = false;
    }

    public float GetWaterHeight()
    {
        return waterHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < waterHeight)
        {
            GameLose(true);
        }
        
        //check if we need to show the controls
        if (gameStart)
        {
            if (timeText + controlShowTime < Time.time)
            {
                controls.enabled = false;
                camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
                car.ControlsOver();

                if (!timerStart)
                {
                    timer = Time.time;
                    timerStart = true;
                    timeLeft.enabled = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            car.Pause();
            pauseMenu.enabled = true;
            camera.GetComponent<CameraController>().AllowCameraMovement(false);
        }

        if(mode == MODE.FLY)
        {
            Quaternion rot = Quaternion.Euler(0, 0, 0);
            transform.rotation = rot;

            float moveU = 0, moveD = 0, moveFB = 0, moveLR;

            moveFB = Input.GetAxis("Vertical");
            moveLR = Input.GetAxis("Horizontal");
            moveU = Input.GetAxis("Up");
            moveD = Input.GetAxis("Down");
            
            Vector3 move = new Vector3(moveLR*6, moveU*4 - moveD*4, moveFB*6);

            transform.Translate(move * speed * Time.deltaTime);
            transform.rotation = camera.transform.rotation;
        }
        //check if we are in the game and if the controls are no longer being shown
        else if (mode == MODE.REGULAR && !controls.enabled && !gameEnd)
        {
            //check if the timer length has elapsed
            if (timer + timerLength < Time.time)
            {
                GameLose(false);
            }

            timeLeft.text = "Time: " + Math.Round((timer + timerLength) - Time.time, 1);

            float yaw = 0;
            yaw += 2 * Input.GetAxis("Mouse X");
            float moveH = Input.GetAxis("Horizontal");
            float moveV = Input.GetAxis("Vertical");
            float jump = 0;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jump = 1;
                inAir = true;
            }

            verticalSpeed -= gravity * Time.deltaTime;
            verticalOffset += verticalSpeed * Time.deltaTime;
            Vector3 move;
            /*if (inAir)
            {
                move = new Vector3(moveH, gravity * Time.deltaTime, moveV);
            }
            else
            {*/
                move = new Vector3(moveH, jump * jumpImpulse, moveV);
            //}
            //transform.Rotate(0,moveH,0);

            //transform.Translate(move * speed * Time.deltaTime);
            //rb.AddForce(Vector3.up * jump);

            rb.AddForce(move * speed);
            //transform.forward += new Vector3(moveH, 0, 0);
        }
    }

    public void BeginGame()
    {
        mode = MODE.REGULAR;
        transform.parent = null;
        player.SetActive(true);
        gameStart = true;

        timeText = Time.time;
        controls.enabled = true;
        flyControls.enabled = false;

        //camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == car.transform && !gameStart)
        {
            transform.position -= Vector3.back * -1;
        }
        else if (collision.transform == car.transform && gameStart && !controls.enabled)
        {
            GameWin();
            car.GameWin();
        }
        else if (collision.transform == boxPrefab.transform)
        {
            timerLength += 10;
        }
        else
            inAir = false;
    }

    void GameWin()
    {
        gameEnd = true;
        endScreen.GetComponent<EndMenu>().Win();
        camera.GetComponent<CameraController>().GameOver();
        transform.parent = truck.transform;
    }

    void GameLose(bool water)
    {
        gameEnd = true;
        endScreen.GetComponent<EndMenu>().Lose(water);
        camera.GetComponent<CameraController>().GameOver();
    }

    public void FlyMode()
    {
        transform.parent = null;
        player.SetActive(true);
        mode = MODE.FLY;
        //GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = new Vector3(0, 40, 0);
        speed *= 2;
        timeLeft.enabled = false;
        flyControls.enabled = true;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        pauseMenu.enabled = false;
        camera.GetComponent<CameraController>().AllowCameraMovement(true);
    }
}
