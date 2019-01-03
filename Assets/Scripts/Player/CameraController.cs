using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private enum Focus {PLAYER, TRUCK, FLY};

    public GameObject terrain;

    public GameObject player;
    public GameObject truck;
    public Camera cam;

    private Vector3 offset;
    private bool curtain = false;
    private float x, y, z;

    public float offsetX, offsetY, offsetZ, rotation;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    public bool clampVerticalRotation = true;
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public float smoothTime = 5f;

    float pitch = 0, yaw = 0;

    Focus currentFocus;

    bool gameEnd = false;

    float currentCameraAngle;

    // Use this for initialization
    void Start()
    {
        currentFocus = Focus.TRUCK;
        m_CharacterTargetRot = player.transform.localRotation;
        m_CameraTargetRot = transform.localRotation;

        currentCameraAngle = 0;
    }

    /*void Update()
    {
        float yRot = Input.GetAxis("Mouse X") * XSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, yRot);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (false)
        {
            player.transform.localRotation = Quaternion.Slerp(player.transform.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            player.transform.localRotation = m_CharacterTargetRot;
            transform.localRotation = m_CameraTargetRot;
        }
    }*/

    void LateUpdate()
    {

        if (currentFocus == Focus.PLAYER)
        {
            //transform.position = new Vector3(player.transform.position.x + offsetX, player.transform.position.y + offsetY, player.transform.position.z + offsetZ);
            //transform.localPosition = new Vector3(offsetX, offsetY, offsetZ);
            //yaw += 2 * Input.GetAxis("Mouse X");
            //pitch -= 2 * Input.GetAxis("Mouse Y");
            float moveH = Input.GetAxis("Horizontal");
            float moveV = Input.GetAxis("Vertical");

            Vector3 playerPos = player.GetComponent<BoxController>().GetPosition();

            if (!gameEnd)
            {
                //player is moving backwards
                if (moveV == -1)
                {
                    currentCameraAngle = Mathf.LerpAngle(currentCameraAngle, 180, 2 * Time.deltaTime);
                }
                //player is not trying to move backwards
                else
                {
                    currentCameraAngle = Mathf.LerpAngle(currentCameraAngle, moveH * 60, 2 * Time.deltaTime);
                }

                float x = -15 * Mathf.Sin(currentCameraAngle * Mathf.Deg2Rad);
                float y = 5;
                float z = -15 * Mathf.Cos(currentCameraAngle * Mathf.Deg2Rad);

                Vector3 directionalVector = new Vector3(x, y, z);
                transform.position = player.transform.position + directionalVector;
                transform.LookAt(player.transform);


                //transform.forward = player.transform.forward;

                //transform.Rotate(0, Vector3.Angle(transform.forward.normalized, playerPos.normalized), 0);
                //transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
                //transform.LookAt(playerPos);
                //Vector3 rot = new Vector3(moveH, 0, 0);
                //transform.Translate(rot);
            }
            else
            {
                transform.parent = null;
                transform.position = new Vector3(player.transform.position.x + offsetX, player.transform.position.y + offsetY, player.transform.position.z + offsetZ);
            }
        }
        else if (currentFocus == Focus.TRUCK)
        {
            transform.position = new Vector3(truck.transform.position.x - 2, truck.transform.position.y + 2, truck.transform.position.z - 3);
            Quaternion rot = Quaternion.Euler(13, 0, 0);
            transform.rotation = rot;
        }
        else if (currentFocus == Focus.FLY)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 5, player.transform.position.z - 2);
            //Quaternion rot = Quaternion.Euler(0, 0, player.transform.rotation.z);
            //transform.rotation = rot;

            yaw += 2 * Input.GetAxis("Mouse X");
            pitch -= 2 * Input.GetAxis("Mouse Y");

            if (!gameEnd)
                transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }

    public void BeginGame()
    {
        currentFocus = Focus.PLAYER;
        transform.position = new Vector3(player.transform.position.x + offsetX, player.transform.position.y + offsetY, player.transform.position.z + offsetZ);
        Quaternion rot = Quaternion.Euler(25, 0, 0);
        transform.rotation = rot;
        transform.parent = player.transform;
    }

    public void FlyMode()
    {
        currentFocus = Focus.FLY;
        transform.position = new Vector3(player.transform.position.x + offsetX, player.transform.position.y + offsetY, player.transform.position.z + offsetZ);

        Cursor.lockState = CursorLockMode.Confined;

        GetComponent<Camera>().farClipPlane = 12000;
        terrain.GetComponent<EndlessTerrain>().detailLevels[0].visibleDistanceThreshold = 1200;
        terrain.GetComponent<EndlessTerrain>().detailLevels[1].visibleDistanceThreshold = 2000;
        terrain.GetComponent<EndlessTerrain>().detailLevels[2].visibleDistanceThreshold = 8000;
    }

    public void GameOver()
    {
        gameEnd = true;
    }

    public void AllowCameraMovement(bool b)
    {
        gameEnd = !b;
    }
}
