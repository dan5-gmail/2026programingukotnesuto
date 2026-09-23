using UnityEngine;

public class NutritionItemPlacement : MonoBehaviour
{
    [Header("栄養アイテム")]
    [SerializeField] private GameObject nutritionItemPrefab;

    [Header("設置")]
    [SerializeField] private float raycastDistance = 100f;

    [SerializeField] private LayerMask placementLayerMask = ~0;

    [SerializeField] private float yOffset = 0.05f;

    private bool isPlacing = false;

    private InventoryManager inventoryManager;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            inventoryManager =
                GameManager.Instance.GetInventoryManager();
        }
    }

    private void Update()
    {
        if (!isPlacing)
            return;

        // 左クリックで設置
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceNutritionItem();
        }

        // 右クリックでキャンセル
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    public void StartPlacement()
    {
        if (nutritionItemPrefab == null)
        {
            Debug.LogWarning(
                "NutritionItemPlacement : nutritionItemPrefabが設定されていません。"
            );

            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning(
                "NutritionItemPlacement : GameManagerがありません。"
            );

            return;
        }

        inventoryManager =
            GameManager.Instance.GetInventoryManager();

        if (inventoryManager == null)
        {
            Debug.LogWarning(
                "NutritionItemPlacement : InventoryManagerがありません。"
            );

            return;
        }

        if (!inventoryManager.CanUseNutritionItem())
        {
            Debug.Log(
                "NutritionItemPlacement : 栄養アイテムを持っていません。"
            );

            return;
        }

        isPlacing = true;

        Debug.Log(
            "NutritionItemPlacement : 栄養アイテム設置モード開始"
        );
    }

    private void TryPlaceNutritionItem()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning(
                "NutritionItemPlacement : Main Cameraが見つかりません。"
            );

            return;
        }

        Ray ray =
            mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            raycastDistance,
            placementLayerMask
        ))
        {
            return;
        }

        Vector3 spawnPosition =
            hit.point + Vector3.up * yOffset;

        GameObject nutritionItem =
            Instantiate(
                nutritionItemPrefab,
                spawnPosition,
                Quaternion.identity
            );

        // アイテムを1個消費
        if (!inventoryManager.UseNutritionItem())
        {
            // 万一消費できなかった場合
            Destroy(nutritionItem);

            Debug.LogWarning(
                "NutritionItemPlacement : 栄養アイテムを消費できませんでした。"
            );

            return;
        }

        isPlacing = false;

        Debug.Log(
            "NutritionItemPlacement : 栄養アイテムを設置しました。"
        );
    }

    private void CancelPlacement()
    {
        isPlacing = false;

        Debug.Log(
            "NutritionItemPlacement : 設置をキャンセルしました。"
        );
    }

    public bool IsPlacing()
    {
        return isPlacing;
    }
}