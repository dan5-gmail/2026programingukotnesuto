using UnityEngine;
// using UnityEngine.Scripting.APIUpdating;

public class PlayerMove : MonoBehaviour
{
    [Header("playerMove")]
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float jumpPower = 7f;


    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; //回転しない,固定
    }


    void Update()
    {
        Playermove();

        PlayerJump();


        // Z軸固定
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            0
        );
    }


    void Playermove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector3(
            moveX * moveSpeed,
            rb.linearVelocity.y,
            0
        );
    }



    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpPower,
                0
            );
        }
    }


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

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;


    }
}
