using UnityEngine;
using TMPro;

public class HeavyStoneUI : MonoBehaviour
{
    [Header("表示テキスト")]
    [SerializeField]
    private TMP_Text countText;

    [Header("InventoryManager")]
    [SerializeField]
    private InventoryManager inventoryManager;

    [Header("HeavyStonePanel")]
    [SerializeField]
    private GameObject heavyStonePanel;

    private void Update()
    {
        if (countText == null || inventoryManager == null)
        {
            return;
        }

        int amount = inventoryManager.GetHeavyStone();

        // =========================================
        // 0個
        // =========================================
        if (amount <= 0)
        {
            if (heavyStonePanel != null)
            {
                heavyStonePanel.SetActive(false);
            }

            if (countText != null)
            {
                countText.gameObject.SetActive(false);
                countText.text = "";
            }

            return;
        }

        // =========================================
        // 1個以上
        // =========================================
        if (heavyStonePanel != null)
        {
            heavyStonePanel.SetActive(true);
        }

        // 個数表示
        if (countText != null)
        {
            countText.gameObject.SetActive(true);
            countText.text = $"×{amount}";
        }
    }
}