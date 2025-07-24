using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;

    private CameraHandler cameraHandler;

    void Start()
    {
        cameraHandler = new CameraHandler(GetComponent<Camera>(), moveSpeed, zoomSpeed, minZoom, maxZoom);
    }

    void Update()
    {
        cameraHandler.HandleMovement();
        cameraHandler.HandleZoom();
    }
}
