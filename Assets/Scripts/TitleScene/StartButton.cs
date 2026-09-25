using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [Header("シーン遷移")]
    [SerializeField]
    private string nextSceneName = "Level1";

    [Header("ホバー時の明るさ")]
    [SerializeField]
    private float hoverBrightness = 1.25f;

    private Renderer buttonRenderer;

    private Color originalColor;
    private Color hoverColor;

    private void Start()
    {
        buttonRenderer = GetComponent<Renderer>();

        if (buttonRenderer == null)
        {
            Debug.LogError("StartButton: Renderer が見つかりません。");
            return;
        }

        // 元の色を保存
        originalColor = buttonRenderer.material.color;

        // ホバー時の色を作る
        hoverColor = originalColor * hoverBrightness;
    }

    private void OnMouseEnter()
    {
        if (buttonRenderer == null)
        {
            return;
        }

        buttonRenderer.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
        if (buttonRenderer == null)
        {
            return;
        }

        buttonRenderer.material.color = originalColor;
    }

    private void OnMouseDown()
    {
        Debug.Log("START BUTTON CLICK");

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("StartButton: 移動先のシーン名が設定されていません。");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}