using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPresonMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;

    [SerializeField] private float speed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    public CinemachineFreeLook cine;

    Vector3 velocity;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public Transform ceilingCheck;
    private Transform Check;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;
    public Vector3 yDisplacement = new Vector3(0f, 5f, 0f);
    public bool isAbove = true;
    // Update is called once per frame
    void Update()
    {
        if(isAbove)
        {
            gravity = -Mathf.Abs(gravity);
            Check = groundCheck;
            cine.m_Orbits[1].m_Height = 45;
            transform.position = yDisplacement;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }
        else
        {
            gravity = Mathf.Abs(gravity);
            Check = ceilingCheck;
            cine.m_Orbits[1].m_Height = -45;
            transform.position = -yDisplacement;
            if (isGrounded && velocity.y > 0)
            {
                velocity.y = 2f;
            }
        }

        isGrounded = Physics.CheckSphere(Check.position, groundDistance, groundMask);

        

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f , vertical).normalized;

        if(direction.magnitude > 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetAngle,ref turnSmoothVelocity,turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle,0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
