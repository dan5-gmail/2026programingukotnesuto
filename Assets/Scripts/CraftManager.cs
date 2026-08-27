using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [Header("必要な木材")]
    [SerializeField] private int woodCost = 1;

    [Header("作成木の杭数")]
    [SerializeField] private int woodenStakeAmount = 4;

    // =========================================
    // 木の杭クラフト
    // =========================================
    public void CraftWoodenStake()
    {
        // GameManager存在確認
        if (GameManager.Instance == null)
        {
            return;
        }

        // Bottle取得
        Bottle bottle = GameManager.Instance.GetBottle();

        if (bottle == null)
        {
            return;
        }

        // InventoryManager取得
        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();

        if (inventoryManager == null)
        {
            return;
        }

        // =====================================
        // 木材足りるかどうか
        // =====================================
        if (bottle.wood < woodCost)
        {
            GameManager.Instance.CraftErrorLog(
                "Not enough wood."
            );

            return;
        }

        // =====================================
        // 木の杭の在庫上限チェック
        // =====================================
        if (!inventoryManager.CanAddWoodenStake(woodenStakeAmount))
        {
            GameManager.Instance.CraftErrorLog(
                "Wooden Stake inventory is full."
            );

            return;
        }

        // =====================================
        // 木材消費
        // =====================================
        bottle.wood -= woodCost;

        // =====================================
        // 木の杭追加
        // =====================================
        GameManager.Instance.AddWoodenStake(
            woodenStakeAmount
        );

        // =====================================
        // 成功ログ
        // =====================================
        GameManager.Instance.CraftedItem(
            "Wooden Stakes",
            woodenStakeAmount
        );
    }
}