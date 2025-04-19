using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement playerMovementScript;

    [Header("References")]
    [SerializeField] float maxSlideTime;
    [SerializeField] float slideForce;
    float slideTimer;
    [SerializeField] float slideYScale;
    float startYScale;

    [Header("References")]
    [SerializeField] KeyCode slideKey = KeyCode.LeftControl;
    float horizontalInput;
    float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovementScript = GetComponent<PlayerMovement>();
        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput !=0 || verticalInput !=0)) 
        {
            StartSLide();
        }

        if(Input.GetKeyUp(slideKey) && playerMovementScript.IsSliding)
        {
            StopSlide();
        }

    }

    private void FixedUpdate()
    {
        if (playerMovementScript.IsSliding)
        {
            SlidingMovement();
        }
    }

    private void StartSLide()
    {
        playerMovementScript.IsSliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void StopSlide()
    {
        playerMovementScript.IsSliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //sliding normally
        if(!playerMovementScript.OnSLope() || rb.velocity.y > -0.1f)
        {
          rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
         slideTimer -= Time.deltaTime;

        }

        //sliding down slope
        else
        {
            rb.AddForce(playerMovementScript.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0) {
            StopSlide();
        }
    }
}
