using UnityEngine;

public class CraftWoodBridgeButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("★★★ CraftWoodBridgeButton OnMouseDown 発火 ★★★");

        if (GameManager.Instance == null)
        {
            Debug.LogError("CraftWoodBridgeButton : GameManager.Instance が NULL");
            return;
        }

        Debug.Log("CraftWoodBridgeButton : CraftWoodBridge() 実行");

        GameManager.Instance.CraftWoodBridge();
    }
}