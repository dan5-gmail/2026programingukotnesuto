using UnityEngine;
using TMPro;

public class LilyPadUI : MonoBehaviour
{
    [Header("表示テキスト")]
    [SerializeField]
    private TMP_Text countText;

    [Header("InventoryManager")]
    [SerializeField]
    private InventoryManager inventoryManager;

    [Header("LilyPadPanel")]
    [SerializeField]
    private GameObject lilyPadPanel;

    private void Update()
    {
        if (countText == null || inventoryManager == null)
        {
            return;
        }

        int amount = inventoryManager.GetLilyPad();

        // =========================================
        // 0個
        // =========================================
        if (amount <= 0)
        {
            if (lilyPadPanel != null)
            {
                lilyPadPanel.SetActive(false);
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
        if (lilyPadPanel != null)
        {
            lilyPadPanel.SetActive(true);
        }

        // 個数表示
        if (countText != null)
        {
            countText.gameObject.SetActive(true);
            countText.text = $"×{amount}";
        }
    }
}