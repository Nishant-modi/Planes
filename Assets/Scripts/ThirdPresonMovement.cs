using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPresonMovement : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;

    [SerializeField] private CharacterController controller;
    [SerializeField] private GameObject tpc;
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
    Coroutine gravCoroutine;
    Vector3 direction = Vector3.zero;
    bool canMove;

    private void Start()
    {
        Check = groundCheck;
        //Cursor.lockState = CursorLockMode.Locked;
        canMove = true;
        gravCoroutine = StartCoroutine(Gravity());
        Guard.OnGuardHasSpottedPlayer += Disable;
    }

    IEnumerator Gravity()
    {
        yield return new WaitForSeconds(0.1f);
        //canMove = true;
        while (true)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            yield return null;
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(Check.position, groundDistance, groundMask);

        if (isAbove)
        {
            if (isGrounded && velocity.y < 0)
            {
                print("isGounded");
                velocity.y = -2f;
            }
        }
        else
        {
            if (isGrounded && velocity.y > 0)
            {
                print("isCeiled");
                velocity.y = 2f;
            }
        }

        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DimensionChanged();
            }


            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            direction = new Vector3(horizontal, 0f, vertical).normalized;


            if (direction.magnitude > 0.1)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }
        }

        //velocity.y += gravity * Time.deltaTime;
        //controller.Move(velocity * Time.deltaTime);

        //print(" " + gameObject.transform.position + " " + gameObject.name);
    }

    public void DimensionChanged()
    {
        canMove = false;
        StopCoroutine(gravCoroutine);
        if (!isAbove)
        {
            transform.position = new Vector3(tpc.transform.position.x, 5, tpc.transform.position.z);
            gravity = -Mathf.Abs(gravity);
            Check = groundCheck;
            cine.m_Orbits[1].m_Height = 45;
            
            print(" " + tpc.transform.position + " " + tpc.name);
            //PlayerDimensionChange(yDisplacement);

            isAbove = true;
        }
        else
        {
            transform.position = new Vector3(tpc.transform.position.x, -5, tpc.transform.position.z);
            gravity = Mathf.Abs(gravity);
            Check = ceilingCheck;
            cine.m_Orbits[1].m_Height = -45;
            
            print(" " + tpc.transform.position + " " + tpc.name);
            //PlayerDimensionChange(-yDisplacement);
            
            isAbove = false;
        }
        
        gravCoroutine = StartCoroutine(Gravity());
        canMove = true;
        
        //yield return null;
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

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Finish")
        {
            Disable();
            if (OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }
    }

    void Disable()
    {
        canMove = false;
    }

    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }

}
