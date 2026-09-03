using UnityEngine;

public class CraftWoodBridgeButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 木の橋をクラフト
        GameManager.Instance.CraftWoodBridge();
    }
}
