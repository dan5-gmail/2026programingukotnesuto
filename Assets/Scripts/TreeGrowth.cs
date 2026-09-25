using UnityEngine;

public class TreeGrowth : MonoBehaviour
{
    [Header("成長")]
    [SerializeField] private int maxGrowthLevel = 3;

    [SerializeField] private float growthScaleMultiplier = 1.15f;

    [Header("根")]
    [SerializeField] private GameObject rootPrefab;

    [SerializeField] private Transform[] rootSpawnPoints;

    [SerializeField] private int rootsPerGrowth = 2;

    [Header("栄養アイテム吸収位置")]
    [SerializeField] private Transform nutritionTarget;

    private int growthLevel = 0;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Grow()
    {
        // 最大成長していたら何もしない
        if (growthLevel >= maxGrowthLevel)
        {
            return;
        }

        growthLevel++;

        // 木そのものを少し大きくする
        float scaleMultiplier =
            Mathf.Pow(growthScaleMultiplier, growthLevel);

        transform.localScale =
            originalScale * scaleMultiplier;

        // 根を生成
        SpawnRoots();

        Debug.Log(
            "TreeGrowth : 大木が成長しました。Growth Level = "
            + growthLevel
        );
    }

    private void SpawnRoots()
    {
        if (rootPrefab == null)
        {
            Debug.LogWarning(
                "TreeGrowth : rootPrefabが設定されていません。"
            );

            return;
        }

        if (rootSpawnPoints == null ||
            rootSpawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "TreeGrowth : rootSpawnPointsが設定されていません。"
            );

            return;
        }

        int spawnCount = Mathf.Min(
            rootsPerGrowth,
            rootSpawnPoints.Length
        );

        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawnPoint = rootSpawnPoints[i];

            if (spawnPoint == null)
                continue;

            GameObject root = Instantiate(
                rootPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            // 根を大木の子にする
            root.transform.SetParent(
                transform,
                true
            );
        }
    }

    public Vector3 GetNutritionTargetPosition()
    {
        if (nutritionTarget != null)
        {
            return nutritionTarget.position;
        }

        // 指定がない場合は大木の中心
        return transform.position;
    }

    public int GetGrowthLevel()
    {
        return growthLevel;
    }

    public bool IsFullyGrown()
    {
        return growthLevel >= maxGrowthLevel;
    }
}