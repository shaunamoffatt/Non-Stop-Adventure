using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControls : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 10f;
    [SerializeField] float turnSpeed = 2f;
    [SerializeField] float jumpSpeed = 7f;

    [SerializeField] private float maxSpeed = 11f;
    Rigidbody rb;
    float startPosX;
    public bool grounded = true;

    private float distToGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosX = 0;
        // get the distance to ground
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    private void FixedUpdate()
    {
        //constantly forward movement
        if (rb.velocity.magnitude < maxSpeed)
        {

            rb.AddRelativeForce(Vector3.forward * forwardSpeed);
            //rb.velocity = forwardSpeed * (rb.velocity.normalized);
        }

        ProcessMovement();
        Jump();
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(new Vector3(0, jumpSpeed, 0), ForceMode.Impulse);
            grounded = false;
        }
    }

    void ProcessMovement()
    {
        if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");

            //Rotate on the y
            if (moveHorizontal != 0)
                transform.Rotate(0, turnSpeed, 0);
          //  transform.Rotate(0, turnSpeed * moveHorizontal, 0);

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
