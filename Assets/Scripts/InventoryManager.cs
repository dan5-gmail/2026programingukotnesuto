using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("クラフト可能アイテム")]
    private int woodenStake = 0;

    [Header("木の杭の最大所持数")]
    public int maxWoodenStake = 10;

    [Header("木の杭の表示")]
    [SerializeField] private GameObject woodStakePanel;
    [SerializeField] private TMP_Text woodStakeCount;

    private void Start()
    {
        RefreshWoodenStakeUI();
    }

    // =========================================
    // 木の杭を追加できるか
    // =========================================
    public bool CanAddWoodenStake(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        return woodenStake + amount <= maxWoodenStake;
    }

    // =========================================
    // 木の杭を追加
    // =========================================
    public bool AddWoodenStake(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        // 最大所持数を超えるなら追加しない
        if (!CanAddWoodenStake(amount))
        {
            return false;
        }

        woodenStake += amount;

        RefreshWoodenStakeUI();

        return true;
    }

    // =========================================
    // 木の杭を使用
    // =========================================
    public bool UseWoodenStake(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (woodenStake < amount)
        {
            return false;
        }

        woodenStake -= amount;

        RefreshWoodenStakeUI();

        return true;
    }

    // =========================================
    // 木の杭の個数取得
    // =========================================
    public int GetWoodenStake()
    {
        return woodenStake;
    }

    // =========================================
    // 木の杭UI更新
    // =========================================
    private void RefreshWoodenStakeUI()
    {
        // 0個
        if (woodenStake <= 0)
        {
            woodenStake = 0;

            if (woodStakePanel != null)
            {
                woodStakePanel.SetActive(false);
            }

            return;
        }

        // 1個以上
        if (woodStakePanel != null)
        {
            woodStakePanel.SetActive(true);
        }

        // 数字だけ表示
        if (woodStakeCount != null)
        {
            woodStakeCount.gameObject.SetActive(true);
            woodStakeCount.text = woodenStake.ToString();
        }
    }
}