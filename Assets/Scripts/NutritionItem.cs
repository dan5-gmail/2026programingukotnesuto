using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class NutritionItem : MonoBehaviour
{
    [Header("吸収")]
    [SerializeField] private float absorbDuration = 0.8f;

    [Header("吸収時の縮小")]
    [SerializeField] private float startScale = 1f;
    [SerializeField] private float endScale = 0.5f;

    [Header("吸収時の移動")]
    [SerializeField] private float moveToTree = 0.3f;

    [Header("透明化")]
    [SerializeField] private bool fadeOut = true;

    private Rigidbody rb;
    private SphereCollider sphereCollider;

    private Renderer[] renderers;

    private bool isAbsorbing = false;

    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        originalScale = transform.localScale;

        // 子オブジェクトも含めてRendererを取得
        renderers = GetComponentsInChildren<Renderer>();

        // 普通の物理オブジェクトとして動かす
        rb.isKinematic = false;
        rb.useGravity = true;

        // 回転は自由
        rb.freezeRotation = false;

        // Trigger判定にする
        sphereCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // すでに吸収中なら何もしない
        if (isAbsorbing)
            return;

        // 大木を探す
        TreeGrowth treeGrowth = other.GetComponentInParent<TreeGrowth>();

        if (treeGrowth == null)
            return;

        // ここで大木に触れた
        StartCoroutine(Absorb(treeGrowth));
    }

    private IEnumerator Absorb(TreeGrowth treeGrowth)
    {
        isAbsorbing = true;

        // 物理挙動を停止
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;

        // これ以上他の物体に反応しない
        sphereCollider.enabled = false;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = treeGrowth.GetNutritionTargetPosition();

        float elapsed = 0f;

        while (elapsed < absorbDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / absorbDuration);

            // なめらかに
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 大木の内部へ少し吸い込まれる
            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothT * moveToTree
            );

            // 少しずつ小さくする
            float scaleT = Mathf.Lerp(
                startScale,
                endScale,
                smoothT
            );

            transform.localScale = originalScale * scaleT;

            // 透明化
            if (fadeOut)
            {
                SetAlpha(1f - smoothT);
            }

            yield return null;
        }

        // 完全に消す
        transform.localScale = Vector3.zero;
        SetAlpha(0f);

        // 大木を成長させる
        if (treeGrowth != null)
        {
            treeGrowth.Grow();
        }

        Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        if (renderers == null)
            return;

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            Material[] materials = renderer.materials;

            foreach (Material material in materials)
            {
                if (material == null)
                    continue;

                if (material.HasProperty("_Color"))
                {
                    Color color = material.color;
                    color.a = alpha;
                    material.color = color;
                }

                if (material.HasProperty("_BaseColor"))
                {
                    Color color = material.GetColor("_BaseColor");
                    color.a = alpha;
                    material.SetColor("_BaseColor", color);
                }
            }
        }
    }
}