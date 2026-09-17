using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PPlayerAutoMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpPower = 5f;

    [Header("Walk / Stop")]
    [SerializeField] private float walkTime = 5f;
    [SerializeField] private float stopTime = 2f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Obstacle Detection")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float obstacleCheckDistance = 1.5f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.3f;

    [Header("Goal")]
    [SerializeField] private GameObject fadeCube;
    [SerializeField] private GameObject tutorialClearText;
    [SerializeField] private float clearTextDelay = 1.0f;

    [Header("Debug")]
    [SerializeField] private bool debugLog = true;

    private Rigidbody rb;
    private BoxCollider boxCollider;

    private bool isGrounded;
    private bool isWalking;
    private bool isJumping;
    private bool forceStop;

    // Goal到達済みか
    private bool reachedGoal;

    private float stateTimer;
    private float lastJumpTime = -10f;

    private const float JumpCooldown = 0.2f;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        rb.freezeRotation = true;

        // 2.5D
        rb.constraints |= RigidbodyConstraints.FreezePositionZ;
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        isWalking = true;
        isJumping = false;
        isGrounded = false;
        forceStop = false;
        reachedGoal = false;

        stateTimer = walkTime;

        // クリア表示は最初は非表示
        if (tutorialClearText != null)
        {
            tutorialClearText.SetActive(false);
        }

        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsJumping", false);
        }
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (forceStop)
        {
            UpdateAnimation();
            return;
        }

        CheckGround();

        UpdateWalkStop();

        CheckAndJump();

        UpdateAnimation();
    }


    // =========================================================
    // FixedUpdate
    // =========================================================

    private void FixedUpdate()
    {
        if (forceStop)
        {
            StopImmediately();
            return;
        }

        Move();
    }


    // =========================================================
    // 歩行 / 停止
    // =========================================================

    private void UpdateWalkStop()
    {
        // ジャンプ中はタイマーを止める
        if (isJumping)
            return;

        stateTimer -= Time.deltaTime;

        if (stateTimer > 0f)
            return;

        if (isWalking)
        {
            isWalking = false;
            stateTimer = stopTime;
        }
        else
        {
            isWalking = true;
            stateTimer = walkTime;
        }
    }


    // =========================================================
    // 移動
    // =========================================================

    private void Move()
    {
        Vector3 velocity = rb.linearVelocity;

        // 歩行中またはジャンプ中は前進
        if (isWalking || isJumping)
        {
            velocity.x = moveSpeed;
        }
        else
        {
            velocity.x = 0f;
        }

        rb.linearVelocity = velocity;
    }


    // =========================================================
    // 障害物検知 → ジャンプ
    // =========================================================

    private void CheckAndJump()
    {
        if (!isWalking)
            return;

        if (isJumping)
            return;

        if (!isGrounded)
            return;

        if (Time.time - lastJumpTime < JumpCooldown)
            return;

        Bounds bounds = boxCollider.bounds;

        // Playerの中心から前方を見る
        Vector3 origin = bounds.center;

        // Playerより少し小さいBox
        Vector3 halfExtents = new Vector3(
            bounds.extents.x * 0.8f,
            bounds.extents.y * 0.8f,
            bounds.extents.z * 0.8f
        );

        bool hitObstacle = Physics.BoxCast(
            origin,
            halfExtents,
            Vector3.right,
            out RaycastHit hit,
            Quaternion.identity,
            obstacleCheckDistance,
            obstacleLayer,
            QueryTriggerInteraction.Ignore
        );

        if (!hitObstacle)
            return;

        if (debugLog)
        {
            Debug.Log(
                "PPlayerAutoMove : 障害物発見！ " +
                hit.collider.gameObject.name +
                " / Grounded = " +
                isGrounded
            );
        }

        Jump();
    }


    // =========================================================
    // ジャンプ
    // =========================================================

    private void Jump()
    {
        if (!isGrounded)
            return;

        if (isJumping)
            return;

        Vector3 velocity = rb.linearVelocity;

        // 横方向の速度はそのまま
        // 縦方向だけジャンプさせる
        velocity.y = jumpPower;

        rb.linearVelocity = velocity;

        isJumping = true;
        isGrounded = false;

        lastJumpTime = Time.time;

        if (debugLog)
        {
            Debug.Log("PPlayerAutoMove : ★★★ JUMP! ★★★");
        }

        if (animator != null)
        {
            animator.SetBool("IsJumping", true);
        }
    }


    // =========================================================
    // Ground判定
    // =========================================================

    private void CheckGround()
    {
        Bounds bounds = boxCollider.bounds;

        Vector3 origin = new Vector3(
            bounds.center.x,
            bounds.min.y + 0.02f,
            bounds.center.z
        );

        bool hitGround = Physics.Raycast(
            origin,
            Vector3.down,
            groundCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        if (hitGround)
        {
            if (!isGrounded)
            {
                isGrounded = true;

                if (isJumping)
                {
                    isJumping = false;

                    if (debugLog)
                    {
                        Debug.Log("PPlayerAutoMove : 着地！");
                    }

                    if (animator != null)
                    {
                        animator.SetBool("IsJumping", false);
                    }
                }
            }
        }
        else
        {
            isGrounded = false;
        }
    }


    // =========================================================
    // Goal判定
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        // すでにGoal到達済みなら何もしない
        if (reachedGoal)
            return;

        // Goalという名前のオブジェクトだけ反応
        if (other.gameObject.name != "Goal")
            return;

        reachedGoal = true;

        if (debugLog)
        {
            Debug.Log("PPlayerAutoMove : ★★★ GOAL到達！ ★★★");
        }

        // Playerを停止
        StopPlayer();

        // クリア演出開始
        StartCoroutine(GoalClearSequence());
    }


    // =========================================================
    // Goalクリア演出
    // =========================================================

    private IEnumerator GoalClearSequence()
    {
        if (debugLog)
        {
            Debug.Log("PPlayerAutoMove : FadeCube起動！");
        }

        // -----------------------------------------------------
        // FadeCubeを起動
        // -----------------------------------------------------

        if (fadeCube != null)
        {
            fadeCube.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "PPlayerAutoMove : FadeCubeが設定されていません。"
            );
        }

        // -----------------------------------------------------
        // 暗転を待つ
        // -----------------------------------------------------

        yield return new WaitForSeconds(clearTextDelay);

        // -----------------------------------------------------
        // 「チュートリアルクリア！」表示
        // -----------------------------------------------------

        if (tutorialClearText != null)
        {
            tutorialClearText.SetActive(true);

            if (debugLog)
            {
                Debug.Log(
                    "PPlayerAutoMove : ★★★ チュートリアルクリア！ ★★★"
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "PPlayerAutoMove : TutorialClearTextが設定されていません。"
            );
        }
    }


    // =========================================================
    // Animation
    // =========================================================

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        bool walkingAnimation =
            (isWalking || isJumping) &&
            !forceStop;

        animator.SetBool(
            "IsWalking",
            walkingAnimation
        );

        animator.SetBool(
            "IsJumping",
            isJumping
        );
    }


    // =========================================================
    // 外部から停止
    // =========================================================

    public void StopPlayer()
    {
        forceStop = true;

        isWalking = false;

        StopImmediately();

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsJumping", false);
        }
    }


    // =========================================================
    // 外部から再開
    // =========================================================

    public void ResumePlayer()
    {
        // Goal後は再開させない
        if (reachedGoal)
            return;

        forceStop = false;

        isWalking = true;
        isJumping = false;

        stateTimer = walkTime;
    }


    // =========================================================
    // 即時停止
    // =========================================================

    private void StopImmediately()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.x = 0f;

        rb.linearVelocity = velocity;
    }


    // =========================================================
    // Gizmos
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        BoxCollider col = GetComponent<BoxCollider>();

        if (col == null)
            return;

        Bounds bounds = col.bounds;

        Vector3 origin = bounds.center;

        Vector3 halfExtents = new Vector3(
            bounds.extents.x * 0.8f,
            bounds.extents.y * 0.8f,
            bounds.extents.z * 0.8f
        );

        Gizmos.color = Color.red;

        // 開始位置
        Gizmos.DrawWireCube(
            origin,
            halfExtents * 2f
        );

        // 終了位置
        Gizmos.DrawWireCube(
            origin + Vector3.right * obstacleCheckDistance,
            halfExtents * 2f
        );

        // 検知線
        Gizmos.DrawLine(
            origin,
            origin + Vector3.right * obstacleCheckDistance
        );
    }
}