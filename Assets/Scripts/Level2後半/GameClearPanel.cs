using System.Collections;
using UnityEngine;

public class GameClearPanel : MonoBehaviour
{
    [Header("フェード設定")]
    [SerializeField]
    private float fadeDuration = 1.0f;

    [Header("開始時の透明度")]
    [SerializeField]
    private float startAlpha = 1.0f;

    [Header("終了時の透明度")]
    [SerializeField]
    private float endAlpha = 0.0f;

    private Renderer[] panelRenderers;
    private bool isShowing = false;

    private void Awake()
    {
        // Panel自身と、その子オブジェクトにあるRendererを取得
        panelRenderers = GetComponentsInChildren<Renderer>(true);

        // 最初は非表示
        SetAlpha(0f);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// GameClearPanelを表示してフェードアウトする
    /// </summary>
    public void ShowGameClear()
    {
        if (isShowing)
        {
            return;
        }

        isShowing = true;

        gameObject.SetActive(true);

        StartCoroutine(GameClearSequence());
    }

    private IEnumerator GameClearSequence()
    {
        // 最初は完全に表示
        SetAlpha(startAlpha);

        // 少し待ってから幽霊化開始
        yield return null;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // 100 → 0
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            SetAlpha(alpha);

            yield return null;
        }

        // 最終的に完全透明
        SetAlpha(endAlpha);

        // ゲームを停止
        StopGame();
    }

    private void SetAlpha(float alpha)
    {
        if (panelRenderers == null)
        {
            return;
        }

        foreach (Renderer renderer in panelRenderers)
        {
            if (renderer == null)
            {
                continue;
            }

            Material material = renderer.material;

            Color color = material.color;
            color.a = alpha;
            material.color = color;
        }
    }

    private void StopGame()
    {
        MonoBehaviour[] scripts =
            FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (MonoBehaviour script in scripts)
        {
            if (script == null)
            {
                continue;
            }

            // GameClearPanel自身は止めない
            if (script == this)
            {
                continue;
            }

            // GameClearPanelの子にあるスクリプトも止めない
            if (script.transform.IsChildOf(transform))
            {
                continue;
            }

            script.enabled = false;
        }

        // 時間も止める
        Time.timeScale = 0f;
    }
}