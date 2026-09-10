using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PPlayerAutoMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float jumpPower = 5f;

    [Header("Stop")]
    public float walkTime = 5f;
    public float stopTime = 2f;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody rb;

    private bool isGrounded = false;
    private bool isWalking = true;
    private bool isJumping = false;

    private float stateTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        isWalking = true;
        stateTimer = walkTime;

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsJumping", false);
    }

    private void Update()
    {
        UpdateMovementState();
    }

    private void FixedUpdate()
    {
        Move();
    }

    // 歩く / 止まるを切り替える
    private void UpdateMovementState()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            if (isWalking)
            {
                // 歩く → 停止
                isWalking = false;
                stateTimer = stopTime;
            }
            else
            {
                // 停止 → 歩く
                isWalking = true;
                stateTimer = walkTime;
            }

            animator.SetBool("IsWalking", isWalking);
        }
    }

    // 自動前進
    private void Move()
    {
        if (!isWalking)
        {
            // 停止
            Vector3 velocity = rb.linearVelocity;
            velocity.x = 0f;
            rb.linearVelocity = velocity;
            return;
        }

        // 前進
        Vector3 moveVelocity = rb.linearVelocity;
        moveVelocity.x = moveSpeed;
        rb.linearVelocity = moveVelocity;
    }

    // ジャンプ
    public void Jump()
    {
        if (!isGrounded || isJumping)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = jumpPower;
        rb.linearVelocity = velocity;

        isJumping = true;

        animator.SetBool("IsJumping", true);
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;

                if (isJumping)
                {
                    isJumping = false;
                    animator.SetBool("IsJumping", false);
                }

                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}