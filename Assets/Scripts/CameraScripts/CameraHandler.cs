using UnityEngine;

public class CameraHandler
{
    private Camera cam;
    private float moveSpeed, zoomSpeed, minZoom, maxZoom;

    public CameraHandler(Camera cam, float moveSpeed, float zoomSpeed, float minZoom, float maxZoom)
    {
        this.cam = cam;
        this.moveSpeed = moveSpeed;
        this.zoomSpeed = zoomSpeed;
        this.minZoom = minZoom;
        this.maxZoom = maxZoom;
    }

    public void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        cam.transform.position += new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime;
    }

    public void HandleZoom()
    {
        if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
            cam.orthographicSize = Mathf.Max(minZoom, cam.orthographicSize - zoomSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            cam.orthographicSize = Mathf.Min(maxZoom, cam.orthographicSize + zoomSpeed * Time.deltaTime);
    }
}
