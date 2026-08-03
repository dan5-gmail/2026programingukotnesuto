using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditorLogManager : MonoBehaviour
{
    public static EditorLogManager Instance;

    // ChatGPT一部使用

    [Header("ログ表示")]
    [SerializeField]
    private TextMeshPro logText;

    [Header("表示行数")]
    [SerializeField]
    private int visibleLines = 8;

    [Header("保存最大数")]
    [SerializeField]
    private int maxLogs = 200;

    private List<string> logs = new List<string>();

    // 一番上に表示する行番号
    private int topIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddLog(Element.ElementType type, int amount)
    {
        string itemName = "";

        switch (type)
        {
            case Element.ElementType.Wood:
                itemName = "木材";
                break;

            case Element.ElementType.Leaf:
                itemName = "葉っぱ";
                break;

            case Element.ElementType.Stone:
                itemName = "石";
                break;

            default:
                itemName = "不明";
                break;
        }

        string message = $"! {itemName}を{amount}つストックしました";

        logs.Add(message);

        if (logs.Count > maxLogs)
        {
            logs.RemoveAt(0);
        }

        // 最新ログへスクロール
        topIndex = Mathf.Max(0, logs.Count - visibleLines);

        RefreshLog();
    }
    public void Scroll(int direction)
    {
        topIndex -= direction;

        topIndex = Mathf.Clamp(
            topIndex,
            0,
            Mathf.Max(0, logs.Count - visibleLines)
        );

        RefreshLog();
    }

    private void RefreshLog()
    {
        logText.text = "";

        int end = Mathf.Min(topIndex + visibleLines, logs.Count);

        for (int i = topIndex; i < end; i++)
        {
            logText.text += logs[i] + "\n";
        }
    }
}