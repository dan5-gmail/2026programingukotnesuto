using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Manager")]
    [SerializeField] private EditorLogManager editorlogManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private CraftManager craftManager;

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
}