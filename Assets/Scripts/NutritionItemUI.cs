using UnityEngine;
using TMPro;

public class NutritionItemUI : MonoBehaviour
{
    [Header("表示テキスト")]
    [SerializeField]
    private TMP_Text countText;

    [Header("InventoryManager")]
    [SerializeField]
    private InventoryManager inventoryManager;

    [Header("NutritionItemPanel")]
    [SerializeField]
    private GameObject nutritionItemPanel;

    private void Update()
    {
        if (countText == null || inventoryManager == null)
        {
            return;
        }

        int amount = inventoryManager.GetNutritionItem();

        // =========================================
        // 0個
        // =========================================
        if (amount <= 0)
        {
            if (nutritionItemPanel != null)
            {
                nutritionItemPanel.SetActive(false);
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
        if (nutritionItemPanel != null)
        {
            nutritionItemPanel.SetActive(true);
        }

        // 個数表示
        if (countText != null)
        {
            countText.gameObject.SetActive(true);
            countText.text = $"×{amount}";
        }
    }
}