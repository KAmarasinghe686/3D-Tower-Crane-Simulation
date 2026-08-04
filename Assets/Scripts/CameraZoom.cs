using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 10f;
    public float rotateSpeed = 100f;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    //Scroll wheel zooms, right mouse button rotates view
    void Update()
    {
        // Zoom toward cursor
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0f)
        {
            Vector3 moveDir = cam.ScreenPointToRay(
                Mouse.current.position.ReadValue()).direction;
            transform.position += moveDir * scroll * zoomSpeed * Time.deltaTime;
        }

        // Rotate with right mouse button
        if (Mouse.current.rightButton.isPressed)
        {
            float mouseX = Mouse.current.delta.ReadValue().x;
            float mouseY = Mouse.current.delta.ReadValue().y;
            transform.eulerAngles += new Vector3(-mouseY, mouseX, 0)
                * rotateSpeed * Time.deltaTime;
        }
    }
}