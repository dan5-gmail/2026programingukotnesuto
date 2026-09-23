using UnityEngine;

public class LilyPadPlacementButton : MonoBehaviour
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

        // 睡蓮を持っていなければ開始しない
        if (inventoryManager.GetLilyPad() <= 0)
        {
            return;
        }

        // 睡蓮設置モード開始
        GameManager.Instance.StartLilyPadPlacement();
    }
}