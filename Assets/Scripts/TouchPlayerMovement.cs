using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    Vector3 velocity;
    public float gravity = -9.81f;


    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 2f;

    private int leftFingerID, rightFingerID;
    private float halfScreenWidth;

    private Vector2 moveInput;
    private Vector2 moveTouchStartPosition;
    private Vector2 lookInput;

    public float cameraSensitivity;
    private float cameraPitch;
    public Transform characterCamera;

    // Start is called before the first frame update
    void Start()
    {
        leftFingerID = -1;
        rightFingerID = -1;
        halfScreenWidth = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        GetTouchInput();

        if (leftFingerID != -1)
        {
            Move();
        }
        if (rightFingerID != -1)
        {
            LookAround();
        }
    }

    void GetTouchInput()
    {
        //if (Input.touchCount > 0)
        //    Debug.Log("Currently" + Input.touchCount + " fingers are touching the screen");

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began)
            {
                if (t.position.x < halfScreenWidth && leftFingerID == -1)
                {
                    leftFingerID = t.fingerId;
                    moveTouchStartPosition = t.position;
                }
                else if (t.position.x > halfScreenWidth && rightFingerID == -1)
                {
                    rightFingerID = t.fingerId;
                }
            }
            if (t.phase == TouchPhase.Canceled)
            {

            }
            if (t.phase == TouchPhase.Moved)
            {
                if (leftFingerID == t.fingerId)
                {
                    moveInput = t.position - moveTouchStartPosition;
                }
                else if (rightFingerID == t.fingerId)
                {
                    lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                }
            }
            if (t.phase == TouchPhase.Stationary)
            {
                if (t.fingerId == rightFingerID)
                {
                    lookInput = Vector2.zero;
                }
            }
            if (t.phase == TouchPhase.Ended)
            {
                if (leftFingerID == t.fingerId)
                {
                    leftFingerID = -1;
                }
                if (rightFingerID == t.fingerId)
                {
                    leftFingerID = -1;
                }
            }
        }
    }
    void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * moveInput.normalized.x + transform.forward * moveInput.normalized.y;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }
    void LookAround()
    {
        cameraPitch = Mathf.Clamp(cameraPitch, -90, 90);
        characterCamera.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.Rotate(transform.up, lookInput.x);
    }
}
