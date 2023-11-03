using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Configurable vars
    public float movementSpeed = 5f;
    public float sprintModifier = 1.5f;
    public float jumpSpeed = 5f;
    public Vector3 gravity = new(0, -9.81f, 0);
    public Vector2 linearLookSensitivity = new(1, 1);
    public Vector2 progressiveLookSensitivity = new(1, 1);
    public float maxVerticalLookAngle = 85f;

    // General vars
    private Camera mainCamera;
    private CharacterController controller;

    // Movement related vars
    private Vector2 inputSpeed = Vector2.zero;
    private bool isSprinting = false;
    private bool isJumping = false;

    // Lookaround related vars
    private Vector2 lookRotation = Vector2.zero;
    private Vector2 lookVelocity = Vector2.zero;
    private bool lookIsLinear = true;

    // Gravity related vars
    private Vector3 gravityVelocity = Vector3.zero;
    private bool isGrounded = true;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void UpdateCameraRotation() {
        Vector2 additionalRotation = lookVelocity;

        if(!lookIsLinear) {
            additionalRotation *= Time.deltaTime;
        }

        lookRotation += additionalRotation;
        lookRotation.x = Mathf.Repeat(lookRotation.x, 360);
        lookRotation.y = Mathf.Clamp(lookRotation.y, -maxVerticalLookAngle, maxVerticalLookAngle);

        Quaternion xQuat = Quaternion.AngleAxis(lookRotation.x, Vector3.up);
        Quaternion yQuat = Quaternion.AngleAxis(lookRotation.y, Vector3.left);

        transform.localRotation = xQuat;
        mainCamera.transform.localRotation = yQuat;
    }

    void Update()
    {
        UpdateCameraRotation();

        Vector3 movement = (isSprinting ? sprintModifier : 1)
            * (transform.right * inputSpeed.x + transform.forward * inputSpeed.y);

        gravityVelocity += gravity * Time.deltaTime;

        if(isJumping) {
            isJumping = false;

            gravityVelocity = (-gravity.normalized) * jumpSpeed;
        }


        Vector3 movementThisFrame = (movement + gravityVelocity) * Time.deltaTime;

        CollisionFlags collisions = controller.Move(movementThisFrame);

        if(collisions.HasFlag(CollisionFlags.Above) && Vector3.Dot(gravityVelocity, -gravity.normalized) > 0) {
            gravityVelocity = Vector3.zero;
        }

        isGrounded = controller.isGrounded;

        if(isGrounded) {
            gravityVelocity = gravity.normalized * 2;
        }
    }

    public float GetMovementSpeedFactor() {
        if(!isGrounded) {
            return 0;
        }

        return inputSpeed.magnitude * (isSprinting ? sprintModifier : 1) / movementSpeed;
    }

    internal void OnMovement(InputValue value) {
        Vector2 inputVelocity = value.Get<Vector2>();

        inputSpeed = inputVelocity * movementSpeed;
    }

    internal void OnJump(InputValue value) {
        isJumping = (value.Get<float>() > 0) && isGrounded;
    }

    internal void OnLinearLookaround(InputValue value) {
        Vector2 inputVelocity = value.Get<Vector2>();

        lookVelocity = new Vector2(
            inputVelocity.x * linearLookSensitivity.x,
            inputVelocity.y * linearLookSensitivity.y
        );

        lookIsLinear = true;
    }

    internal void OnProgressiveLookaround(InputValue value) {
        Vector2 inputVelocity = value.Get<Vector2>();

        lookVelocity = new Vector2(
            inputVelocity.x * progressiveLookSensitivity.x,
            inputVelocity.y * progressiveLookSensitivity.y
        ) * 100;

        lookIsLinear = false;
    }

    internal void OnSprint(InputValue value) {
        isSprinting = value.Get<float>() > 0;
    }

    internal void OnMenu(InputValue value) {
        if(value.Get<float>() > 0) {
            Application.Quit();
        }
    }
}
