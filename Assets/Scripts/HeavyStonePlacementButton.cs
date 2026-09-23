using UnityEngine;

public class HeavyStonePlacementButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();

        if (inventoryManager == null)
        {
            return;
        }

        // 重い石を持っていなければ開始しない
        if (inventoryManager.GetHeavyStone() <= 0)
        {
            return;
        }

        // 重い石設置モード開始
        GameManager.Instance.StartHeavyStonePlacement();
    }
}