using UnityEngine;
using TMPro;

public class WoodenStakeItemUI : MonoBehaviour
{
    [Header("表示テキスト")]
    [SerializeField]
    private TMP_Text countText;

    [Header("WoodStake管理Manager")]
    [SerializeField]
    private InventoryManager inventoryManager;

    [Header("WoodStakePanel")]
    [SerializeField]
    private GameObject woodStakePanel;

    private void Update()
    {
        if (countText == null || inventoryManager == null)
        {
            return;
        }

        int amount = inventoryManager.GetWoodenStake();

        // =========================================
        // 0個
        // =========================================
        if (amount <= 0)
        {
            if (woodStakePanel != null)
            {
                woodStakePanel.SetActive(false);
            }

            if (countText != null)
            {
                countText.text = "";
            }

            return;
        }

        // =========================================
        // 1個以上
        // =========================================
        if (woodStakePanel != null)
        {
            woodStakePanel.SetActive(true);
        }

        // 個数表示
        countText.text = $"×{amount}";
    }
}
