using UnityEngine;

public class Plusone : MonoBehaviour
{
    [Header("浮遊速度")]
    [SerializeField]
    private float moveSpeed = 2f;

    [Header("存在時間")]
    [SerializeField]
    private float lifeTime = 1f;

    [Header("透明化速度")]
    [SerializeField]
    private float fadeSpeed = 1f;

    [Header("拡大演出")]
    [SerializeField]
    private float startScale = 0.8f;

    [SerializeField]
    private float endScale = 1.2f;

    private Vector3 moveDirection;

    private Renderer[] renderers;

    void Start()
    {
        // Renderer全取得
        renderers = GetComponentsInChildren<Renderer>();

        // 少し左右ランダムに飛ぶ
        moveDirection = new Vector3(
            Random.Range(-0.2f, 0.2f),
            0.5f,
            0f
        ).normalized;

        transform.localScale = Vector3.one * startScale;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {

        // 上へ移動
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 少しずつ大きくする
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            Vector3.one * endScale,
            5f * Time.deltaTime
        );

        // scratchの幽霊効果再現　※ChatGPT使用
        foreach (Renderer rend in renderers)
        {
            if (rend.material.HasProperty("_Color"))
            {
                Color color = rend.material.color;
                color.a -= fadeSpeed * Time.deltaTime;
                rend.material.color = color;
            }
        }
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    }
}