using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoalEffect : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject ringPrefab;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    [Header("Movement")]
    [SerializeField] private float moveHeight = 3.0f;
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Spawn Interval")]
    [SerializeField] private float spawnInterval = 50f;

    [Header("Limit")]
    [SerializeField] private int maxRings = 5; // 同時に存在できる最大数

    [Header("Fade")]
    [SerializeField] private float fadeStart = 0.3f;

    // 生成したリングを安全にリストで追跡する
    private List<GameObject> activeRings = new List<GameObject>();

    private void Start()
    {
        if (ringPrefab == null || spawnPoint == null)
        {
            Debug.LogError("GoalEffect: PrefabまたはSpawnPointが設定されていません！");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // nullになった要素や破棄されたものをリストから掃除
            activeRings.RemoveAll(ring => ring == null);

            // リストの数が上限に達していなければ新しく生成する
            if (activeRings.Count < maxRings)
            {
                SpawnRing();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRing()
    {
        GameObject ring = Instantiate(
            ringPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        activeRings.Add(ring);
        StartCoroutine(MoveAndFade(ring));
    }

    private IEnumerator MoveAndFade(GameObject ring)
    {
        if (ring == null) yield break;

        Vector3 startPosition = ring.transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * moveHeight;

        Renderer[] renderers = ring.GetComponentsInChildren<Renderer>();

        float elapsed = 0f;
        float duration = moveHeight / Mathf.Max(moveSpeed, 0.001f);

        while (elapsed < duration)
        {
            if (ring == null)
                break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 上へ移動
            ring.transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            // フェード処理
            float alpha = 1f;
            if (t > fadeStart)
            {
                alpha = Mathf.Lerp(
                    1f,
                    0f,
                    (t - fadeStart) / (1f - fadeStart)
                );
            }

            SetAlpha(renderers, alpha);

            yield return null;
        }

        if (ring != null)
        {
            Destroy(ring);
        }

        // リストからも確実に除外
        if (ring != null)
        {
            activeRings.Remove(ring);
        }
        activeRings.RemoveAll(r => r == null);
    }

    private void SetAlpha(Renderer[] renderers, float alpha)
    {
        foreach (Renderer rend in renderers)
        {
            if (rend == null) continue;

            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;
                }
            }
        }
    }
}