using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerMove : MonoBehaviour
{
    // 一部ChatGPT使用

    [Header("プレイヤー動作")]
    [SerializeField]
    private float moveSpeed = 2f;

    private float defaultMoveSpeed;

    [SerializeField]
    private float jumpPower = 5f;

    [SerializeField]
    private float Gravity = 9.8f;

    [Header("坂の設定")]
    [SerializeField]
    private float groundNormalThreshold = 0.5f;

    [Header("アニメーション")]
    private Animator animator;

    private Rigidbody rb;

    private bool isGrounded;

    // 現在接触している地面の法線
    private Vector3 groundNormal = Vector3.up;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // 回転を固定
        rb.freezeRotation = true;

        // 2.5DなのでZ軸を固定
        rb.constraints |= RigidbodyConstraints.FreezePositionZ;

        defaultMoveSpeed = moveSpeed;
    }


    void Update()
    {
        // 移動
        PlayerMoveControl();

        // ジャンプ
        PlayerJump();
    }


    void FixedUpdate()
    {
        // =========================================
        // 重力
        // =========================================

        Vector3 gravityForce = Vector3.down * Gravity;

        rb.AddForce(gravityForce);


        // =========================================
        // 坂での滑りを防止
        // =========================================

        if (isGrounded && groundNormal.y > groundNormalThreshold)
        {
            // 重力のうち、地面に沿って働く成分を取得
            Vector3 slopeGravity =
                gravityForce -
                Vector3.Project(gravityForce, groundNormal);

            // 坂を滑り落ちる成分だけ打ち消す
            rb.AddForce(-slopeGravity);
        }
    }


    // =========================================
    // 左右移動
    // =========================================

    void PlayerMoveControl()
    {
        // =========================================
        // 速度変更
        // =========================================

        if (Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift))
        {
            moveSpeed = defaultMoveSpeed * 1.5f;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
        }


        // =========================================
        // 左右入力
        // =========================================

        float moveX = Input.GetAxis("Horizontal");


        // =========================================
        // Rigidbodyの速度
        // =========================================

        float currentX = rb.linearVelocity.x;

        float targetX = moveX * moveSpeed;

        float smoothX = Mathf.Lerp(
            currentX,
            targetX,
            0.15f
        );


        rb.linearVelocity = new Vector3(
            smoothX,
            rb.linearVelocity.y,
            0
        );


        // =========================================
        // 歩きアニメーション
        // =========================================

        // Input.GetAxis()の微小な値で
        // 勝手に歩き状態にならないようにする
        bool isWalking = Mathf.Abs(moveX) > 0.1f;

        animator.SetBool(
            "PlayerWalk",
            isWalking
        );


        // =========================================
        // プレイヤーの向き
        // =========================================

        if (moveX > 0.1f)
        {
            transform.rotation =
                Quaternion.Euler(0, 92, 0);
        }
        else if (moveX < -0.1f)
        {
            transform.rotation =
                Quaternion.Euler(0, -88, 0);
        }
    }


    // =========================================
    // ジャンプ
    // =========================================

    void PlayerJump()
    {
        if (
            (Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.UpArrow))
            && isGrounded
        )
        {
            // ジャンプ
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpPower,
                0
            );


            // =========================================
            // ジャンプアニメーション
            // =========================================

            animator.SetTrigger("PlayerJump");
        }
    }


    // =========================================
    // 地面との接触
    // =========================================

    private void OnCollisionStay(Collision collision)
    {
        bool foundGround = false;

        Vector3 bestNormal = Vector3.up;


        foreach (ContactPoint contact in collision.contacts)
        {
            // 上向きの面を地面として認識
            if (contact.normal.y > groundNormalThreshold)
            {
                if (!foundGround ||
                    contact.normal.y > bestNormal.y)
                {
                    bestNormal = contact.normal;

                    foundGround = true;
                }
            }
        }


        if (foundGround)
        {
            isGrounded = true;

            groundNormal = bestNormal;
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;

        groundNormal = Vector3.up;
    }
}