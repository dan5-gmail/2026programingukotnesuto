using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LilyPad : MonoBehaviour
{
    [Header("泥のLayer")]
    [SerializeField] private LayerMask mudLayer;

    [Header("浮力設定")]
    [SerializeField] private float buoyancyForce = 10f;

    [SerializeField] private float damping = 3f;

    [Header("浮き位置")]
    [SerializeField] private float waterLevelOffset = 0.2f;

    [Header("レイキャスト設定")]
    [SerializeField] private float raycastDistance = 2f;

    private Rigidbody rb;
    private bool isOnMud = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 重力を有効
        rb.useGravity = true;

        // 回転は制限
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        // 泥レイヤー判定
        CheckMudLayer();

        // 泥の上なら浮力を適用
        if (isOnMud)
        {
            ApplyBuoyancy();
        }
    }

    private void CheckMudLayer()
    {
        // 下方にレイキャストして泥レイヤーをチェック
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, mudLayer))
        {
            isOnMud = true;

            // 抵抗を増やして沈みにくく
            rb.linearDamping = damping;

            Debug.Log($"LilyPad : 泥検知 - 距離: {hit.distance}");
        }
        else
        {
            isOnMud = false;

            // 泥でないなら通常の抵抗
            rb.linearDamping = 0.1f;
        }
    }

    private void ApplyBuoyancy()
    {
        // 重力を相殺する力
        Vector3 counterGravity = Vector3.up * rb.mass * 9.81f;
        rb.AddForce(counterGravity, ForceMode.Force);

        // 目標の高さ（泥表面 + オフセット）
        if (transform.position.y < waterLevelOffset)
        {
            float depth = waterLevelOffset - transform.position.y;
            Vector3 buoyancy = Vector3.up * depth * buoyancyForce;
            rb.AddForce(buoyancy, ForceMode.Acceleration);
        }

        // 高すぎたら下向きの力を加える
        else if (transform.position.y > waterLevelOffset)
        {
            float excessHeight = transform.position.y - waterLevelOffset;
            Vector3 downwardForce = Vector3.down * excessHeight * buoyancyForce * 0.5f;
            rb.AddForce(downwardForce, ForceMode.Acceleration);
        }
    }

    // デバッグ用：泥レイヤー設定
    private void OnDrawGizmos()
    {
        if (isOnMud)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}