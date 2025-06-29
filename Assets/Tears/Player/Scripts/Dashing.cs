using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("Referencces")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerCam;
    Rigidbody rb;
    PlayerMovement pM;

    [Header("Dashing")]
    [SerializeField] float dashForce;
    [SerializeField] float dashUpwardForce;
    [SerializeField] float dashDuration;
    [SerializeField] float maxDashYSpeed;

    [Header("Cooldonw")]
    [SerializeField] float dashCooldown;
    [SerializeField] float dashCooldownTimer;

    [Header("Input")]
    [SerializeField] KeyCode dashKey = KeyCode.LeftShift;

    [Header("Settings")]
    [SerializeField] bool useCameraForward = true;
    [SerializeField] bool allowAllDirections = true;
    [SerializeField] bool disableGravaity = false;
    [SerializeField] bool resetVel = true;

    Vector3 forceToApply;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pM = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(dashKey)) Dash();
        if(dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        
    }

    private void Dash()
    {
        if (dashCooldownTimer > 0) return; else dashCooldownTimer = dashCooldown;

        pM.dashing = true;
        pM.MaxYSpeed = maxDashYSpeed;

        Transform forwardTransform;
        if (useCameraForward) forwardTransform = playerCam; else forwardTransform = orientation;

        Vector3 dirrectionToDash = GetDirection(forwardTransform);
        forceToApply = dirrectionToDash * dashForce + orientation.up * dashUpwardForce;

        if (disableGravaity) {
            rb.useGravity = false;
        }

        Invoke(nameof(DelayDashForce), 0.025f);
        Invoke(nameof(DashCooldown), dashDuration);
    }

    private void DelayDashForce()
    {
        if (resetVel) rb.velocity = Vector3.zero;
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    private void DashCooldown()
    {
        pM.dashing = false;
        pM.MaxYSpeed = 0;
        if (disableGravaity)
        {
            rb.useGravity = true;
        }
    }

    private Vector3 GetDirection(Transform forwardTransform)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();
        direction = forwardTransform.forward;
        if (allowAllDirections)
        {
            direction = forwardTransform.forward * verticalInput + forwardTransform.right * horizontalInput;
        }
        //else
        //{
        //    direction = forwardTransform.forward;
        //}

        //if (verticalInput == 0 && horizontalInput == 0)
        //{
        //    direction = forwardTransform.forward;
        //}
        return direction.normalized;
    }
}
