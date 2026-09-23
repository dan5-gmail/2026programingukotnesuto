using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Manager")]
    [SerializeField] private EditorLogManager editorlogManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private CraftManager craftManager;
    [SerializeField] private PlacementManager placementManager;

    [Header("Bottle")]
    [SerializeField] private Bottle bottle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================================
    // Bottle取得
    // =========================================
    public Bottle GetBottle()
    {
        return bottle;
    }

    // =========================================
    // InventoryManager取得
    // =========================================
    public InventoryManager GetInventoryManager()
    {
        return inventoryManager;
    }

    // =========================================
    // 木の杭を追加
    // =========================================
    public void AddWoodenStake(int amount)
    {
        if (inventoryManager == null)
        {
            return;
        }

        inventoryManager.AddWoodenStake(amount);
    }

    // =========================================
    // アイテム取得
    // =========================================
    public void ItemCollected(
        Element.ElementType type,
        int amount
    )
    {
        if (editorlogManager == null)
        {
            return;
        }

        editorlogManager.AddLog(type, amount);
    }

    // =========================================
    // クラフト成功ログ
    // =========================================
    public void CraftedItem(
        string itemName,
        int amount
    )
    {
        if (editorlogManager == null)
        {
            return;
        }

        editorlogManager.AddCraftLog(
            itemName,
            amount
        );
    }

    // =========================================
    // クラフト失敗ログ
    // =========================================
    public void CraftErrorLog(string message)
    {
        if (editorlogManager == null)
        {
            return;
        }

        editorlogManager.AddErrorLog(message);
    }

    // =========================================
    // 木の杭をクラフト
    // =========================================
    public void CraftWoodenStake()
    {
        if (craftManager == null)
        {
            return;
        }

        craftManager.CraftWoodenStake();
    }

    // =========================================
    // 木の橋をクラフト
    // =========================================
    public void CraftWoodBridge()
    {
        if (craftManager == null)
        {
            return;
        }

        craftManager.CraftWoodBridge();
    }

    // =========================================
    // 栄養アイテムをクラフト
    // =========================================
    public void CraftNutritionItem()
    {
        if (craftManager == null)
        {
            return;
        }

        craftManager.CraftNutritionItem();
    }

    // =========================================
    // 睡蓮をクラフト
    // =========================================
    public void CraftLilyPad()
    {
        if (craftManager == null)
        {
            return;
        }

        craftManager.CraftLilyPad();
    }

    // =========================================
    // ゴール達成ログ
    // =========================================
    public void GoalReached()
    {
        if (editorlogManager == null)
        {
            return;
        }

        editorlogManager.AddCraftLog(
            "Level Cleared!",
            1
        );
    }

    // =========================================
    // 木の杭の設置開始
    // =========================================
    public void StartWoodenStakePlacement()
    {
        if (placementManager == null)
        {
            return;
        }

        placementManager.StartWoodenStakePlacement();
    }

    // =========================================
    // 木の橋の設置開始
    // =========================================
    public void StartWoodBridgePlacement()
    {
        if (placementManager == null)
        {
            return;
        }

        placementManager.StartWoodBridgePlacement();
    }

    // =========================================
    // 栄養アイテムの設置開始
    // =========================================
    public void StartNutritionItemPlacement()
    {
        if (placementManager == null)
        {
            return;
        }

        placementManager.StartNutritionItemPlacement();
    }

    // =========================================
    // 睡蓮の設置開始
    // =========================================
    public void StartLilyPadPlacement()
    {
        if (placementManager == null)
        {
            return;
        }

        placementManager.StartLilyPadPlacement();
    }

    // =========================================
    // 重い石をクラフト
    // =========================================
    public void CraftHeavyStone()
    {
        if (craftManager == null)
        {
            return;
        }

        craftManager.CraftHeavyStone();
    }

    // =========================================
    // 重い石の設置開始
    // =========================================
    public void StartHeavyStonePlacement()
    {
        if (placementManager == null)
        {
            return;
        }

        placementManager.StartHeavyStonePlacement();
    }
}