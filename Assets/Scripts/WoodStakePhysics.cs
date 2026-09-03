using UnityEngine;

public class WoodStakePhysics : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float checkInterval = 0.5f; // 地面チェックの間隔
    [SerializeField] private float checkDistance = 0.5f; // 地面チェックの距離
    [SerializeField] private LayerMask groundLayers = -1; // 地面として扱うレイヤー（デフォルト: 全レイヤー）

    private Rigidbody rb;
    private float nextCheckTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        nextCheckTime = Time.time + checkInterval;
    }

    private void Update()
    {
        // 定期的に地面チェック
        if (Time.time >= nextCheckTime)
        {
            CheckGroundSupport();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void CheckGroundSupport()
    {
        if (rb == null) return;

        // 杭の方向（前方）にレイキャストして壁/地面をチェック
        Vector3 stakeDirection = transform.forward;
        Ray ray = new Ray(transform.position, stakeDirection);
        RaycastHit hit;

        bool hasSupport = Physics.Raycast(ray, out hit, checkDistance, groundLayers);

        if (hasSupport)
        {
            // 支持体がある場合は固定
            if (!rb.isKinematic)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            // 支持体がない場合は物理演算を有効にして落下
            if (rb.isKinematic)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }
}
