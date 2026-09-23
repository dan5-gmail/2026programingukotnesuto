using UnityEngine;

/// <summary>
/// 神殿の透明化を座標で判定する。
///
/// EditorまたはPlayerのX座標が指定した値以上になると、
/// Shindenn以下のRendererを半透明にする。
///
/// X座標が指定値より小さくなると、元の状態に戻す。
/// </summary>
public class ShindennTransparency : MonoBehaviour
{
    [Header("神殿")]
    [Tooltip("神殿の素材をまとめているShindenn")]
    [SerializeField] private Transform shindenn;

    [Header("判定対象")]
    [Tooltip("人間が操作するEditor")]
    [SerializeField] private Transform editor;

    [Tooltip("自動で動くPlayer")]
    [SerializeField] private Transform player;

    [Header("透明化するX座標")]
    [Tooltip("EditorまたはPlayerのX座標がこの値以上になると透明化します")]
    [SerializeField] private float transparentStartX = 10f;

    [Header("透明度")]
    [Range(0f, 1f)]
    [Tooltip("1 = 完全不透明、0 = 完全透明")]
    [SerializeField] private float transparentAlpha = 0.35f;

    [Header("透明化速度")]
    [Tooltip("透明化・復元にかかる時間")]
    [SerializeField] private float fadeDuration = 0.25f;


    // 神殿内のRenderer
    private Renderer[] shindennRenderers;

    // 元の色
    private Color[] originalColors;

    // 現在透明化しているか
    private bool isTransparent = false;

    // 透明化処理中
    private bool isChanging = false;


    private void Start()
    {
        // Shindennが未指定なら、このGameObjectを使用
        if (shindenn == null)
        {
            shindenn = transform;
        }

        // 神殿以下のRendererをすべて取得
        shindennRenderers =
            shindenn.GetComponentsInChildren<Renderer>(true);

        if (shindennRenderers == null ||
            shindennRenderers.Length == 0)
        {
            Debug.LogWarning(
                "ShindennTransparency : Shindenn以下にRendererがありません。"
            );

            return;
        }

        // 元の色を保存
        originalColors = new Color[shindennRenderers.Length];

        for (int i = 0; i < shindennRenderers.Length; i++)
        {
            Renderer renderer = shindennRenderers[i];

            if (renderer == null)
            {
                continue;
            }

            Material material = renderer.material;

            if (material == null)
            {
                continue;
            }

            if (material.HasProperty("_BaseColor"))
            {
                originalColors[i] = material.GetColor("_BaseColor");
            }
            else if (material.HasProperty("_Color"))
            {
                originalColors[i] = material.GetColor("_Color");
            }
            else
            {
                originalColors[i] = Color.white;
            }
        }
    }


    private void Update()
    {
        if (shindennRenderers == null ||
            shindennRenderers.Length == 0)
        {
            return;
        }

        bool shouldBeTransparent = false;


        // -------------------------
        // Editor判定
        // -------------------------

        if (editor != null)
        {
            if (editor.position.x >= transparentStartX)
            {
                shouldBeTransparent = true;
            }
        }


        // -------------------------
        // Player判定
        // -------------------------

        if (player != null)
        {
            if (player.position.x >= transparentStartX)
            {
                shouldBeTransparent = true;
            }
        }


        // -------------------------
        // 状態変更
        // -------------------------

        if (shouldBeTransparent && !isTransparent)
        {
            isTransparent = true;

            StopAllCoroutines();
            StartCoroutine(透明化());
        }
        else if (!shouldBeTransparent && isTransparent)
        {
            isTransparent = false;

            StopAllCoroutines();
            StartCoroutine(元に戻す());
        }
    }


    /// <summary>
    /// 神殿を半透明にする。
    /// </summary>
    private System.Collections.IEnumerator 透明化()
    {
        isChanging = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t;

            if (fadeDuration <= 0f)
            {
                t = 1f;
            }
            else
            {
                t = Mathf.Clamp01(elapsed / fadeDuration);
            }

            t = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < shindennRenderers.Length; i++)
            {
                Renderer renderer = shindennRenderers[i];

                if (renderer == null)
                {
                    continue;
                }

                Material material = renderer.material;

                if (material == null)
                {
                    continue;
                }

                Color color = originalColors[i];

                color.a = Mathf.Lerp(
                    originalColors[i].a,
                    transparentAlpha,
                    t
                );

                色を設定(material, color);
            }

            yield return null;
        }

        // 最終値
        for (int i = 0; i < shindennRenderers.Length; i++)
        {
            Renderer renderer = shindennRenderers[i];

            if (renderer == null)
            {
                continue;
            }

            Material material = renderer.material;

            if (material == null)
            {
                continue;
            }

            Color color = originalColors[i];
            color.a = transparentAlpha;

            透明Materialに変更(material);
            色を設定(material, color);
        }

        isChanging = false;
    }


    /// <summary>
    /// 神殿を元の状態に戻す。
    /// </summary>
    private System.Collections.IEnumerator 元に戻す()
    {
        isChanging = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t;

            if (fadeDuration <= 0f)
            {
                t = 1f;
            }
            else
            {
                t = Mathf.Clamp01(elapsed / fadeDuration);
            }

            t = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < shindennRenderers.Length; i++)
            {
                Renderer renderer = shindennRenderers[i];

                if (renderer == null)
                {
                    continue;
                }

                Material material = renderer.material;

                if (material == null)
                {
                    continue;
                }

                Color color = originalColors[i];

                color.a = Mathf.Lerp(
                    transparentAlpha,
                    originalColors[i].a,
                    t
                );

                色を設定(material, color);
            }

            yield return null;
        }

        // 完全に元へ戻す
        for (int i = 0; i < shindennRenderers.Length; i++)
        {
            Renderer renderer = shindennRenderers[i];

            if (renderer == null)
            {
                continue;
            }

            Material material = renderer.material;

            if (material == null)
            {
                continue;
            }

            色を設定(material, originalColors[i]);
        }

        isChanging = false;
    }


    /// <summary>
    /// Materialの色を設定する。
    /// </summary>
    private void 色を設定(Material material, Color color)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        else if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
    }


    /// <summary>
    /// Materialを透明描画モードに変更する。
    /// </summary>
    private void 透明Materialに変更(Material material)
    {
        if (material == null)
        {
            return;
        }

        // URP系
        if (material.HasProperty("_Surface"))
        {
            // Transparent
            material.SetFloat("_Surface", 1f);

            if (material.HasProperty("_Blend"))
            {
                // Alpha
                material.SetFloat("_Blend", 0f);
            }

            if (material.HasProperty("_AlphaClip"))
            {
                material.SetFloat("_AlphaClip", 0f);
            }

            if (material.HasProperty("_SrcBlend"))
            {
                material.SetFloat(
                    "_SrcBlend",
                    (float)UnityEngine.Rendering.BlendMode.SrcAlpha
                );
            }

            if (material.HasProperty("_DstBlend"))
            {
                material.SetFloat(
                    "_DstBlend",
                    (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
                );
            }

            if (material.HasProperty("_ZWrite"))
            {
                material.SetFloat("_ZWrite", 0f);
            }

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHATEST_ON");

            material.renderQueue =
                (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        // Standard Shader
        if (material.HasProperty("_Mode"))
        {
            // Fade
            material.SetFloat("_Mode", 2f);

            if (material.HasProperty("_SrcBlend"))
            {
                material.SetInt(
                    "_SrcBlend",
                    (int)UnityEngine.Rendering.BlendMode.SrcAlpha
                );
            }

            if (material.HasProperty("_DstBlend"))
            {
                material.SetInt(
                    "_DstBlend",
                    (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
                );
            }

            if (material.HasProperty("_ZWrite"))
            {
                material.SetInt("_ZWrite", 0);
            }

            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            material.renderQueue =
                (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
    }
}