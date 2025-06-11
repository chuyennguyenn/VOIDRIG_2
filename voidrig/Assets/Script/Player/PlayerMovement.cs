using System;
using UnityEngine;

public class PlayerMovement: MonoBehaviour
{
    private CharacterController controller;

    public float speed = 10f;
    public float gravity = -9.81f;
    public float jump = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGround;
    bool isMoving;

    private Vector3 lastPos = new Vector3(0f, 0f, 0f);

    Vector3 velocity;
    private void Start()
    {
        controller= GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGround = Physics.CheckSphere(groundCheck.position , groundDistance, groundMask);

        if (isGround && velocity.y < 0 )
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGround == true)
        {
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (lastPos != gameObject.transform.position && isGround == true) {
            isMoving = true;
        }
        else isMoving = false;

        lastPos = gameObject.transform.position;


    }
}
