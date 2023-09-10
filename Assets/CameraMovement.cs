using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] Transform firstPersonCamera;
    [SerializeField] Transform xyzOrientation;
    [SerializeField] float cameraHorizontalOffset = -4f;
    [SerializeField] float cameraVerticalOffset = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = firstPersonCamera.position + xyzOrientation.forward * cameraHorizontalOffset + xyzOrientation.up * cameraVerticalOffset;
    }
}
