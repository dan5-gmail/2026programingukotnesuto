using UnityEngine;

public class CraftNutritionItemButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 栄養アイテムをクラフト
        GameManager.Instance.CraftNutritionItem();
    }
}