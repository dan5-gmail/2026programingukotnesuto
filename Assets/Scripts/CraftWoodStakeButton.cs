using UnityEngine;

public class CraftWoodenStakeButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 木の杭をクラフト
        GameManager.Instance.CraftWoodenStake();
    }
}