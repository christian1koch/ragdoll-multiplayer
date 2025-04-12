using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineCamera mainCamera;
    public CinemachineCamera aimCamera;

    // Update is called once per frame
    void Update()
    {
        // if the right mouse button is pressed then aim the camera
        if (Input.GetMouseButton(1))
        {
            // set the aim camera to have a higher priority
            aimCamera.Priority = 10;
            // set the main camera to have a lower priority
            mainCamera.Priority = 0;
        }
        else
        {
            // set the main camera to have a higher priority
            mainCamera.Priority = 10;
            // set the aim camera to have a lower priority
            aimCamera.Priority = 0;
        }
    }
}
