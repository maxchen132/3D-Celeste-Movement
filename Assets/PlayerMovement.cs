using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float groundDrag = 5f;
    [SerializeField] float airResistance = 0.5f;
    [SerializeField] Transform xzOrientation;
    [SerializeField] Transform xyzOrientation;
    [SerializeField] Transform groundChecker;
    [SerializeField] float checkSphereRadius = 0.5f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float dashSpeed = 35f;
    [SerializeField] float normalMaxSpeed = 10f;
    [SerializeField] float dashTime = 0.5f;
    [SerializeField] float dashRecoveryTime = 0.3f;

    float horizontalInput;
    float verticalInput;
    Vector3 movementDirection;
    Vector3 dashDirection;
    float dashRotationX;
    bool isGrounded;
    bool hasDash = true;
    bool isDashing = false;
    float maxSpeed = 10f;
    Vector3 momentumVelocity = new Vector3(0, 0, 0);

    Vector3 inputVelocity = new Vector3(0, 0, 0);
    bool aReleased, dReleased, sReleased, wReleased = false;
    bool aPressed, dPressed, sPressed, wPressed = false;
    bool aHeld, dHeld, sHeld, wHeld = false;


    Rigidbody rb;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

    }

    // Update is called once per frame
    void Update() {
        isGrounded = Physics.CheckSphere(groundChecker.position, checkSphereRadius, groundMask);
        rb.drag = isGrounded ? groundDrag : airResistance;

        GetInput();
        GetKeyPressed();
        GetKeyRelease();
        
        if (hasDash && Input.GetMouseButtonDown(0)) {
            StartDash();
        }

    }

    void FixedUpdate() {
        
        SetMovementVelocity();

        Jump();
        if (isDashing) {
            Dash();
        } else {
            if (isGrounded && !hasDash && !IsInvoking("RecoverDash")) {
                Invoke("RecoverDash", dashRecoveryTime);
            }
            if (IsInvoking("RecoverDash") && !isGrounded) {
                CancelInvoke("RecoverDash");
            }
        }

        

        // if (maxSpeed > normalMaxSpeed && isGrounded && !isDashing) {
        //     maxSpeed = normalMaxSpeed;
        // }
        // if (maxSpeed < normalMaxSpeed) {
        //     maxSpeed = normalMaxSpeed;
        // }

        if (momentumVelocity.magnitude > 0 && isGrounded && !isDashing) {
            momentumVelocity = new Vector3(0, 0, 0);
        }

    }

    void GetInput() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        movementDirection = xzOrientation.forward * verticalInput + xzOrientation.right * horizontalInput;
        
        Vector3 inputVelocityCompRight = Vector3.Project(inputVelocity, xzOrientation.right);
        Vector3 inputVelocityCompForward = Vector3.Project(inputVelocity, xzOrientation.forward);

        // When movement button is first pressed
        if (aPressed) {
            inputVelocity -= xzOrientation.right * movementSpeed;
            aPressed = false;
            aHeld = true;
        }
        if (dPressed) {
            inputVelocity += xzOrientation.right * movementSpeed;
            dPressed = false;
            dHeld = true;
        }
        if (sPressed) {
            inputVelocity -= xzOrientation.forward * movementSpeed;
            sPressed = false;
            sHeld = true;
        }
        if (wPressed) {
            inputVelocity += xzOrientation.forward * movementSpeed;
            wPressed = false;
            wHeld = true;
        }

        // When movement button is continuously held
        if (aHeld && !isDashing) {
            // rb.velocity -= xzVelocity;
            // rb.velocity += Vector3.RotateTowards(xzVelocity, -xzOrientation.right, 10f * Time.deltaTime, 0.0f);
            inputVelocity = Vector3.RotateTowards(inputVelocity, -xzOrientation.right, 10f * Time.deltaTime, 0f);
        }
        if (dHeld && !isDashing) {
            // rb.velocity -= xzVelocity;
            // rb.velocity += Vector3.RotateTowards(xzVelocity, xzOrientation.right, 10f * Time.deltaTime, 0.0f);
            inputVelocity = Vector3.RotateTowards(inputVelocity, xzOrientation.right, 10f * Time.deltaTime, 0f);
        }
        if (sHeld && !isDashing) {
            // rb.velocity -= xzVelocity;
            // rb.velocity += Vector3.RotateTowards(xzVelocity, -xzOrientation.forward, 10f * Time.deltaTime, 0.0f);
            inputVelocity = Vector3.RotateTowards(inputVelocity, -xzOrientation.forward, 10f * Time.deltaTime, 0f);
        }
        if (wHeld && !isDashing) {
            // rb.velocity -= xzVelocity;
            // rb.velocity += Vector3.RotateTowards(xzVelocity, xzOrientation.forward, 10f * Time.deltaTime, 0.0f);
            inputVelocity = Vector3.RotateTowards(inputVelocity, xzOrientation.forward, 10f * Time.deltaTime, 0f);
        }


        // When movement button is released
        if (aReleased) {
            inputVelocity -= inputVelocityCompRight;
            aReleased = false;
            aHeld = false;
        }
        if (dReleased) {
            inputVelocity -= inputVelocityCompRight;
            dReleased = false;
            dHeld = false;
        }
        if (sReleased) {
            inputVelocity -= inputVelocityCompForward;
            sReleased = false;
            sHeld = false;
        }
        if (wReleased) {
            inputVelocity -= inputVelocityCompForward;
            wReleased = false;
            wHeld = false;
        }

    }

    void SetMovementVelocity() {
        
        Vector3 xzVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);   
        rb.velocity -= xzVelocity;
        rb.velocity += momentumVelocity + inputVelocity;

           

        // if (xzVelocity.magnitude > maxSpeed) {
        //     Vector3 limitedVelocity = xzVelocity.normalized * maxSpeed;
        //     rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        // }

        
    }

    void Jump() {
        if (Input.GetKey(KeyCode.Space) && isGrounded) {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            if (isDashing) {
                CancelInvoke("StopDash");
                StopDash();
            }
            
        }
    }

    void GetKeyRelease() {
        if (Input.GetKeyUp(KeyCode.A)) {
            aReleased = true;
        }
        if (Input.GetKeyUp(KeyCode.D)) {
            dReleased = true;
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            sReleased = true;
        }
        if (Input.GetKeyUp(KeyCode.W)) {
            wReleased = true;
        }
    }

    void GetKeyPressed() {
        if (Input.GetKeyDown(KeyCode.A)) {
            aPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            dPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            sPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            wPressed = true;
        }
    }

    void StartDash() {
        hasDash = false;
        isDashing = true;
        dashDirection = xyzOrientation.forward;
        dashRotationX = xyzOrientation.rotation.eulerAngles.x;
    }

    void Dash() {
        rb.velocity = dashDirection * dashSpeed;
        Invoke("StopDash", dashTime);
    }

    void RecoverDash() {
        hasDash = true;
    }

    void StopDash() {
        isDashing = false;

        Debug.Log(dashRotationX);
        if (dashRotationX > 12.5 && dashRotationX < 37.5 || isGrounded) {
            //maxSpeed = dashSpeed;
            momentumVelocity += dashDirection * dashSpeed;
            // Debug.Log("Maintaining Speed");
        } else {
            //maxSpeed = normalMaxSpeed;
            rb.velocity = new Vector3 (rb.velocity.x, 0f, rb.velocity.z);
            // Debug.Log("Not Maintaining Speed");
        }
    }

}
