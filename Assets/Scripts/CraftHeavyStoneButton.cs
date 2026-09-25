using UnityEngine;

public class CraftHeavyStoneButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 重い石をクラフト
        GameManager.Instance.CraftHeavyStone();
    }
}