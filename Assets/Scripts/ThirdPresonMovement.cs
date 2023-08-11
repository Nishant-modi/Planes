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
    void FixedUpdate()
    {
       /* if(transform.position.y>=0)
        {
            isAbove = true;
        }
        else
        {
            isAbove=false;
        }*/


        if(Input.GetKeyDown(KeyCode.Space))
        {
            print("spaced");
            DimensionChanged();
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
       // print(isGrounded);
        

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

    public void DimensionChanged()
    {

        if (!isAbove)
        {
            gravity = -Mathf.Abs(gravity);
            Check = groundCheck;
            cine.m_Orbits[1].m_Height = 45;
            //gameObject.transform.position = new Vector3(transform.position.x, 5, transform.position.z);
            //PlayerDimensionChange(yDisplacement);
            if (isGrounded && velocity.y < 0)
            {
                print("isGounded");
                velocity.y = -2f;
            }
            isAbove = true;
        }
        else
        {
            gravity = Mathf.Abs(gravity);
            Check = ceilingCheck;
            cine.m_Orbits[1].m_Height = -45;
            //gameObject.transform.position = new Vector3(transform.position.x, -5, transform.position.z);
            //PlayerDimensionChange(-yDisplacement);
            if (isGrounded && velocity.y > 0)
            {
                velocity.y = 2f;
            }
            isAbove = false;
        }
    }

    public void PlayerDimensionChange(Vector3 newPos)
    {
        Vector3 newPosition = newPos;
        Vector3 initialPos = gameObject.transform.position;
        float timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, (initialPos + newPosition), timeSinceStarted);

            // If the object has arrived, stop the coroutine
            if (transform.position == (initialPos + newPosition))
            {
                break;
            }
        }
    }

}
