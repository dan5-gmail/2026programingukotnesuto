using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float startX = 10f;
    [SerializeField] private float endX = 1f;

    [Header("アニメーション")]
    [SerializeField] private float speed = 10f;

    [Header("インベントリ画面")]
    [SerializeField] private GameObject bottlePanel;
    [SerializeField] private GameObject craftPanel;

    private bool isOpen = false;

    private Vector3 targetPosition;
    private float fixedY;
    private float fixedZ;

    private void Start()
    {
        fixedY = transform.localPosition.y;
        fixedZ = transform.localPosition.z;

        // 最初は右側へ隠す
        targetPosition = new Vector3(
            startX,
            fixedY,
            fixedZ
        );

        transform.localPosition = targetPosition;

        // 最初はBottle画面
        ShowBottlePanel();
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * speed
        );
    }

    // =========================================
    // インベントリを開く / 閉じる
    // =========================================
    public void TogglePanel()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            // インベントリを開くたびにBottle画面から開始
            ShowBottlePanel();
        }

        float targetX = isOpen ? endX : startX;

        targetPosition = new Vector3(
            targetX,
            fixedY,
            fixedZ
        );
    }

    // =========================================
    // Bottle画面を表示
    // =========================================
    public void ShowBottlePanel()
    {
        if (bottlePanel != null)
        {
            bottlePanel.SetActive(true);
        }

        if (craftPanel != null)
        {
            craftPanel.SetActive(false);
        }
    }

    // =========================================
    // Craft画面を表示
    // =========================================
    public void ShowCraftPanel()
    {
        if (bottlePanel != null)
        {
            bottlePanel.SetActive(false);
        }

        if (craftPanel != null)
        {
            craftPanel.SetActive(true);
        }
    }

    // =========================================
    // Craftボタンから呼ぶ
    // =========================================
    public void OpenCraft()
    {
        // InventoryPanel自体が閉じていたら開く
        if (!isOpen)
        {
            isOpen = true;

            targetPosition = new Vector3(
                endX,
                fixedY,
                fixedZ
            );
        }

        // Craft画面を表示
        ShowCraftPanel();
    }

    // =========================================
    // Bottleボタンから呼ぶ
    // =========================================
    public void OpenBottle()
    {
        // InventoryPanel自体が閉じていたら開く
        if (!isOpen)
        {
            isOpen = true;

            targetPosition = new Vector3(
                endX,
                fixedY,
                fixedZ
            );
        }

        // Bottle画面を表示
        ShowBottlePanel();
    }

    // =========================================
    // ×ボタンから呼ぶ
    // =========================================
    public void ClosePanel()
    {
        isOpen = false;

        targetPosition = new Vector3(
            startX,
            fixedY,
            fixedZ
        );
    }
}