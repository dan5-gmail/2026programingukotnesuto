using UnityEngine;

public class NutritionItemUseButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 栄養アイテム設置モード開始
        GameManager.Instance.StartNutritionItemPlacement();
    }
}