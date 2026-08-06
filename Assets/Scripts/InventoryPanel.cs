using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public RectTransform panel;

    [Header("移動の位置設定")]
    [SerializeField]
    private float startX = 650f;
    [SerializeField]
    private float endX = 0f;

    [Header("アニメーション設定")]
    [SerializeField]
    private float speed = 10f;

    private bool isOpen = false;
    private Vector2 target;
    private float fixedY;

    private void Start()
    {
        fixedY = panel.anchoredPosition.y;
        target = new Vector2(startX, fixedY);
        panel.anchoredPosition = target;
    }

    private void Update()
    {
        panel.anchoredPosition = Vector2.Lerp(
            panel.anchoredPosition,
            target,
            Time.deltaTime * speed
        );
    }

    public void TogglePanel()
    {
        isOpen = !isOpen;

        float targetX = isOpen ? endX : startX;
        target = new Vector2(targetX, fixedY);
    }
}