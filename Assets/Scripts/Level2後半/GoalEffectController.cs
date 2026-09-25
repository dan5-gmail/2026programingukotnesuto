using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoalEffectController : MonoBehaviour
{
    [Header("リングPrefab")]
    [SerializeField] private GameObject ringPrefab;

    [Header("リング生成位置")]
    [SerializeField] private Transform spawnPoint;

    [Header("上昇")]
    [SerializeField] private float moveHeight = 3.0f;

    [SerializeField] private float moveSpeed = 1.5f;

    [Header("生成間隔")]
    [SerializeField] private float spawnInterval = 2.0f;

    [Header("同時存在数")]
    [SerializeField] private int maxRings = 5;

    [Header("フェード開始位置")]
    [Range(0f, 1f)]
    [SerializeField] private float fadeStart = 0.3f;

    // 現在生成されているリングだけを管理
    private readonly List<GameObject> activeRings =
        new List<GameObject>();


    private void Start()
    {
        if (ringPrefab == null)
        {
            Debug.LogError(
                "GoalEffectController : Ring Prefabが設定されていません。"
            );

            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError(
                "GoalEffectController : Spawn Pointが設定されていません。"
            );

            return;
        }

        if (maxRings < 1)
        {
            maxRings = 1;
        }

        if (spawnInterval < 0.01f)
        {
            spawnInterval = 0.01f;
        }

        StartCoroutine(SpawnLoop());
    }


    /// <summary>
    /// リングを一定間隔で生成する。
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // 破棄済みリングを整理
            activeRings.RemoveAll(ring => ring == null);

            // 最大数未満なら生成
            if (activeRings.Count < maxRings)
            {
                SpawnRing();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }


    /// <summary>
    /// リングPrefabを1個だけ生成する。
    /// </summary>
    private void SpawnRing()
    {
        if (ringPrefab == null)
        {
            return;
        }

        if (spawnPoint == null)
        {
            return;
        }

        // ★ここで生成するのは「リングPrefab」だけ
        GameObject ring = Instantiate(
            ringPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        if (ring == null)
        {
            return;
        }

        activeRings.Add(ring);

        StartCoroutine(MoveAndFade(ring));
    }


    /// <summary>
    /// 生成したリングを上昇させながらフェードアウトする。
    /// </summary>
    private IEnumerator MoveAndFade(GameObject ring)
    {
        if (ring == null)
        {
            yield break;
        }

        Vector3 startPosition =
            ring.transform.position;

        Vector3 targetPosition =
            startPosition + Vector3.up * moveHeight;

        Renderer[] renderers =
            ring.GetComponentsInChildren<Renderer>(true);

        float elapsed = 0f;

        float duration =
            moveHeight /
            Mathf.Max(moveSpeed, 0.001f);


        while (elapsed < duration)
        {
            // 途中で破棄された場合
            if (ring == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(elapsed / duration);


            // -------------------------
            // 上へ移動
            // -------------------------

            ring.transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );


            // -------------------------
            // フェード
            // -------------------------

            float alpha = 1f;

            if (t > fadeStart)
            {
                float fadeT =
                    (t - fadeStart) /
                    Mathf.Max(1f - fadeStart, 0.001f);

                alpha =
                    Mathf.Lerp(
                        1f,
                        0f,
                        fadeT
                    );
            }

            SetAlpha(
                renderers,
                alpha
            );

            yield return null;
        }


        // 完全に消す
        if (ring != null)
        {
            Destroy(ring);
        }

        // リストから除外
        activeRings.Remove(ring);

        // 念のため破棄済みも整理
        activeRings.RemoveAll(
            currentRing => currentRing == null
        );
    }


    /// <summary>
    /// リングの透明度を変更する。
    /// </summary>
    private void SetAlpha(
        Renderer[] renderers,
        float alpha
    )
    {
        if (renderers == null)
        {
            return;
        }

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            Material[] materials =
                renderer.materials;

            foreach (Material material in materials)
            {
                if (material == null)
                {
                    continue;
                }

                if (material.HasProperty("_Color"))
                {
                    Color color =
                        material.color;

                    color.a = alpha;

                    material.color = color;
                }
                else if (material.HasProperty("_BaseColor"))
                {
                    Color color =
                        material.GetColor("_BaseColor");

                    color.a = alpha;

                    material.SetColor(
                        "_BaseColor",
                        color
                    );
                }
            }
        }
    }


    /// <summary>
    /// 現在生成されているリングを全削除。
    /// シーン終了時などにも利用可能。
    /// </summary>
    public void ClearRings()
    {
        foreach (GameObject ring in activeRings)
        {
            if (ring != null)
            {
                Destroy(ring);
            }
        }

        activeRings.Clear();
    }


    private void OnDestroy()
    {
        StopAllCoroutines();

        foreach (GameObject ring in activeRings)
        {
            if (ring != null)
            {
                Destroy(ring);
            }
        }

        activeRings.Clear();
    }
}