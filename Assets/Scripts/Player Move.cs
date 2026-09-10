using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerMove : MonoBehaviour
{
    //一部チャットgpt使用
    [Header("プレイヤー動作")]
    [SerializeField]
    private float moveSpeed = 2f;

    private float defaultmoveSpeed;

    [SerializeField]
    private float jumpPower;

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

        Debug.Log(animator);

        // 回転を固定
        rb.freezeRotation = true;

        // 2.5DなのでZ軸を固定
        rb.constraints |= RigidbodyConstraints.FreezePositionZ;

        defaultmoveSpeed = moveSpeed;
    }


    void Update()
    {
        Playermove();

        PlayerJump();

        animator.SetBool("PlayerJump", !isGrounded);

        // Z軸をRigidbody側で固定しているので、
        // transform.positionによる強制変更はしない
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


    void Playermove()
    {
        // =========================================
        // 速度変更
        // =========================================

        if (Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift))
        {
            moveSpeed = defaultmoveSpeed * 1.5f;
        }
        else
        {
            moveSpeed = defaultmoveSpeed;
        }


        // =========================================
        // 左右移動
        // =========================================

        float moveX = Input.GetAxis("Horizontal");

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

        animator.SetBool(
            "PlayerWalk",
            moveX != 0
        );


        // =========================================
        // 向き変更
        // =========================================

        if (moveX > 0)
        {
            transform.rotation =
                Quaternion.Euler(0, 92, 0);
        }
        else if (moveX < 0)
        {
            transform.rotation =
                Quaternion.Euler(0, -88, 0);
        }
    }


    void PlayerJump()
    {
        if (
            (Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.UpArrow))
            && isGrounded
        )
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpPower,
                0
            );
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