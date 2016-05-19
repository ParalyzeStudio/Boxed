using UnityEngine;
using System.Collections;

public class IsometricCameraController : MonoBehaviour
{
    float height = 40f;

    float camMoveSpeed = 30f;
    float zoomSpeed = 2.0f;

    void Start()
    {
        transform.position = Vector3.zero;
        //Look at the center to get an angle
        transform.rotation = Quaternion.Euler(new Vector3(30, 45, 0));

        Vector3 cameraDirection = this.transform.forward;

        //Set up the camera position so it looks at Vector3.zero and move it far enough from object scenes
        transform.position = -50 * cameraDirection;
    }

    void LateUpdate()
    {
        //Move camera with keys
        //Move left/right
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= new Vector3(camMoveSpeed * Time.deltaTime, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(camMoveSpeed * Time.deltaTime, 0f, 0f);
        }

        //Move forward/back
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= new Vector3(0f, 0f, camMoveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            transform.position += new Vector3(0f, 0f, camMoveSpeed * Time.deltaTime);
        }

        //Zoom
        float cameraMaxSize = 30;
        float cameraMinSize = 5;
        Camera camera = this.GetComponent<Camera>();
        float cameraCurrentSize = camera.orthographicSize;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.I))
        {
            if (cameraCurrentSize > cameraMinSize)
            {
                cameraCurrentSize -= zoomSpeed;
                cameraCurrentSize = Mathf.Clamp(cameraCurrentSize, cameraMinSize, cameraMaxSize);
                camera.orthographicSize = cameraCurrentSize;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.O))
        {
            if (cameraCurrentSize < cameraMaxSize)
            {
                cameraCurrentSize += zoomSpeed;
                cameraCurrentSize = Mathf.Clamp(cameraCurrentSize, cameraMinSize, cameraMaxSize);
                camera.orthographicSize = cameraCurrentSize;
            }
        }

        //make the camera movement speed relative to the level of zoom
        camMoveSpeed = cameraCurrentSize;
    }
}