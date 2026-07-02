// using Unity.VisualScripting;
using UnityEngine;
// using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerMove : MonoBehaviour
{
    [Header("プレイヤー動作")]
    [SerializeField]
    private float moveSpeed = 2f;

    private float defaultmoveSpeed;

    [SerializeField]
    private float jumpPower;

    [SerializeField]
    private float Gravity;

    [Header("アニメーション")]
    private Animator walk;

    private Rigidbody rb;
    private bool isGrounded;

    private Animator animator;

    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        // rb.freezeRotation = true; //回転しない,固定

        // defaultmoveSpeed = moveSpeed;  //初期速度記憶

        // walk = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();

        Debug.Log(animator);

        rb.freezeRotation = true;

        defaultmoveSpeed = moveSpeed;


    }


    void Update()
    {
        // Debug.Log(moveSpeed);
        Playermove();

        PlayerJump();


        // Z軸固定
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            0
        );

    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * Gravity);
    }


    void Playermove()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveSpeed = defaultmoveSpeed * 2;
        }
        else
        {
            moveSpeed = defaultmoveSpeed;
        }

        float moveX = Input.GetAxis("Horizontal");

        float currentX = rb.linearVelocity.x;
        float targetX = moveX * moveSpeed;

        float smoothX = Mathf.Lerp(currentX, targetX, 0.15f);

        rb.linearVelocity = new Vector3(smoothX,
        rb.linearVelocity.y,
        0);


        animator.SetBool("PlayerWalk", moveX != 0);


        // 向き変更
        if (moveX > 0)
        {
            transform.rotation = Quaternion.Euler(0, 92, 0);
        }
        else if (moveX < 0)
        {
            transform.rotation = Quaternion.Euler(0, -88, 0);
        }


    }



    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded || Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpPower,
                0
            );
        }
    }

    //＊ChatGPT使用
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // 接地面が上向きなら地面
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
            }
        }
    }
    // ＊ChatGPT使用
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;


    }
}
