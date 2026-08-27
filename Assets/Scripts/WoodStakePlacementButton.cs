using UnityEngine;

public class WoodStakePlacementButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 木杭設置mode開始
        GameManager.Instance.StartWoodenStakePlacement();
    }
}