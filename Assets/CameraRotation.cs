using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{

    [SerializeField] float sensX = 300;
    [SerializeField] float sensY = 300;
    [SerializeField] Transform xzOrientation;
    [SerializeField] Transform xyzOrientation;

    float xRotation;
    float yRotation;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        xzOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
        xyzOrientation.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
