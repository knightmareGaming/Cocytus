using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public Rigidbody2D rb;


    [Header("Run Set-up")]
    public float moveSpeed = 10;
    public float acceleration = 1;
    public float deceleration = -1;
    public float velPower = 2;

    public float friction = 0.2f;

    [Header("Ground Set-up")]
    public Transform groundCheck;
    public const float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump Set-up")]
    public float coyoteTime = 0.05f;
    public float jumpCutMultiplier = 0.5f;

    private float coyoteTimeCounter;
    public float jumpStrength = 2f;
    bool isJumping = false;
    bool jumpInputReleased = false;
    


    // Hall of variables
    [SerializeField] bool isGrounded = false;
    float lastGrounded = 0;
    float lastJumped = 0;

    private void Update()
    {
        GroundCheck();
    }

    void FixedUpdate()
    {

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float jump = Input.GetAxis("Jump");
        // Maybe compress into a function for easier use?
        // Nah

        //movement(horizontal);

        #region Movement
        float target = horizontal * moveSpeed;
        float speedDif = target - rb.velocity.x;
        float accelRate = (Mathf.Abs(target) > 0.01f) ? acceleration : deceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
        #endregion


        // Friction
        #region Friction
        if (isGrounded && horizontal < 0.05f)
        {
            float dec = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(friction)) * Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -dec, ForceMode2D.Impulse);
        }
        #endregion

        #region Jump
        if (coyoteTimeCounter > 0f && isGrounded && jump > 0.01f) {
            rb.AddForce(jumpStrength * Vector2.up, ForceMode2D.Impulse);
            Debug.Log("jumped");
            lastGrounded = 0.2f;
            lastJumped = 0.2f;
            isJumping = true;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0) {
            Debug.Log("HURHUR");
            jumpInputReleased = true;
            isJumping = false;
         }


        #endregion

        #region Crouch

        float crouch = Input.GetAxis("Crouch");
        if (crouch > 0) {
            Debug.Log(crouch);
        }

        #endregion


        Debug.Log("jumping " + isJumping);
    }


    private void GroundCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);

        isGrounded = false;

        if (colliders.Length > 0)
        {
            isGrounded = true;
        }
        

    }

    private void Jump() {

        rb.AddForce(jumpStrength * Vector2.up, ForceMode2D.Impulse);
        lastGrounded = 0;
        lastJumped = 0;
        isJumping = true;
        jumpInputReleased = false;

    }

    private void OnJumpUp() {

        if (rb.velocity.y > 0 && isJumping) {

            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }

    }
}
