using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    float moveSpeed;
    [SerializeField] Transform orientation;
    [SerializeField] float groundDrag;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMultiplier;
    [SerializeField] float regMoveSpeed;
    [SerializeField] float slowMoveSpeed;
    [SerializeField] float dashSpeed;
    public float MaxYSpeed;
    bool readyToJump = true;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    float desiredMoveSPeed;
    float lastDesiredMoveSpeed;
    [SerializeField] float slideSpeed;
    [SerializeField] float speedIncreaseMultiplier;
    [SerializeField] float slopeIncreaseMultiplier;


    [Header("Ground Ceck")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask isGround;
    bool grounded;

    [Header("KeyBinds")]
    [SerializeField] KeyCode jumpKey= KeyCode.Space;
    [SerializeField] KeyCode slowKey = KeyCode.Y;

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    public MovementState State;
    public bool IsSliding;
    public bool dashing;

    public enum MovementState
    {
        walking,
        slowwalking,
        air,
        sliding,
        dashing
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        StateHandler();
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isGround);
        if (grounded && State != MovementState.dashing) rb.drag = groundDrag; else rb.drag = 0;
        SpeedControl();
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            if (grounded) Invoke(nameof(ResetJump), jumpCooldown);
            else if (!grounded) rb.AddForce(moveDirection.normalized * desiredMoveSPeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void StateHandler()
    {
        if (dashing)
        {
            State = MovementState.dashing;
            moveSpeed = dashSpeed;
        }
        //SlowWalk
       else if (grounded && Input.GetKey(slowKey))
        {
            State = MovementState.slowwalking;
            desiredMoveSPeed = slowMoveSpeed;
        }

        else if (IsSliding)
        {
            State = MovementState.sliding;

            if(OnSLope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSPeed = slideSpeed;
            }
            else
            {
                desiredMoveSPeed = regMoveSpeed;
            }
        }

        //reg walk
        else if (grounded)
        {
            State = MovementState.walking;
            desiredMoveSPeed = regMoveSpeed;
        }

        //air
        else
        {
            State = MovementState.air;

        }
        if(Mathf.Abs(desiredMoveSPeed - lastDesiredMoveSpeed) > (regMoveSpeed - slowMoveSpeed) && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(LerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSPeed;
        }

        lastDesiredMoveSpeed = desiredMoveSPeed;
    }

    private void MovePlayer()
    {
        if (State == MovementState.dashing) return;
        //on slope
        if (OnSLope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * desiredMoveSPeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        rb.useGravity = !OnSLope();

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * desiredMoveSPeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        if (OnSLope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > desiredMoveSPeed) { 
                rb.velocity = rb.velocity.normalized * desiredMoveSPeed;
            }

        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > desiredMoveSPeed)
            {
                Vector3 limitedVel = flatVel.normalized * desiredMoveSPeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }
        if(MaxYSpeed != 0 && rb.velocity.y > MaxYSpeed) rb.velocity = new Vector3(rb.velocity.x, MaxYSpeed, rb.velocity.z);

    }

    private void Jump() 
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSLope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal.normalized);
    }

    private IEnumerator LerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSPeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSPeed, time / difference);
            if (OnSLope()) {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }
            yield return null;
        }

        moveSpeed = desiredMoveSPeed;
    }
}
