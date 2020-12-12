using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControls : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 200;
    [SerializeField] float turnSpeed = 2f;
    [SerializeField] float jumpSpeed = 20f;

    [SerializeField] private float maxSpeed = 11f;
    Rigidbody rb;
    float startPosX;
    public bool grounded = true;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosX = 0;
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        ProcessConstantForwardMovement();
        ProcessAnimation();
        ProcessHorzontalMovement();
        Jump(); 
    }

    void ProcessConstantForwardMovement()
    {
        //constantly forward movement
        rb.AddRelativeForce(Vector3.forward * forwardSpeed);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Debug.Log("Jump presesed");
            rb.AddForce(new Vector3(0, jumpSpeed, 0), ForceMode.Impulse);
            grounded = false;
            SoundManager.PlaySound(SoundManager.Sound.jump);
        }else
        {
            if (IsGrounded()) grounded = true;
        }
    }

    bool IsGrounded()
    {
        Vector3 position = transform.position;
        float extra = 0.5f;
        return Physics.Raycast(position, Vector3.down, GetComponent<Collider>().bounds.extents.y + extra);
    }

    void ProcessAnimation()
    {
        if (!grounded)
        {
            anim.SetBool("Jump", true);
        }

        if (grounded)
        {
            anim.SetBool("Jump", false);
        }
    }

    void ProcessHorzontalMovement()
    {
        if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");

            //Rotate on the y
            if (moveHorizontal != 0)
               transform.Rotate(0, turnSpeed * moveHorizontal, 0);

        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        startPosX = touch.position.x;
                        break;
                    case TouchPhase.Moved:
                        if (startPosX > touch.position.x)
                        {
                            transform.Rotate(0, -turnSpeed ,0);
                        }
                        else if (startPosX < touch.position.x)
                        {
                            transform.Rotate(0, turnSpeed,0);
                        }
                        break;
                    case TouchPhase.Ended:
                        Debug.Log("Touch Phase Ended.");
                        break;
                }
            }
        }
    }
}
