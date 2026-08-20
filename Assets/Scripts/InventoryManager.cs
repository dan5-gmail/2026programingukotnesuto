using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("クラフト可能アイテム")]
    public int woodenStake;

    // =========================================
    // 木の杭アイテム欄追加
    // =========================================
    public void AddWoodenStake(int amount)
    {
        woodenStake += amount;
    }

    // =========================================
    // 木の杭ステージ使用
    // =========================================
    public bool UseWoodenStake(int amount)
    {
        if (woodenStake < amount)
        {
            return false;
        }

        woodenStake -= amount;
        return true;
    }
}