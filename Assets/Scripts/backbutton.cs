using UnityEngine;

public class BackButton : MonoBehaviour
{
    [Header("閉じるパネル")]
    [SerializeField] private GameObject inventoryPanel;

    private void OnMouseDown()
    {
        if (inventoryPanel == null)
            return;

        InventoryPanel panel = inventoryPanel.GetComponent<InventoryPanel>();

        if (panel != null)
        {
            panel.ClosePanel();
        }
    }
}