using UnityEngine;

public class CraftLilyPadButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 睡蓮をクラフト
        GameManager.Instance.CraftLilyPad();
    }
}