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

    // =========================================================
    // アイテム取得ログ
    // =========================================================
    public void AddLog(Element.ElementType type, int amount)
    {
        string itemName = "";
        string itemColor = "";

        switch (type)
        {
            case Element.ElementType.Wood:
                itemName = "Wood";
                itemColor = "#8B4513"; // 茶色
                break;

            case Element.ElementType.Leaf:
                itemName = "Leaves";
                itemColor = "#32CD32"; // 緑色
                break;

            case Element.ElementType.Stone:
                itemName = "Stone";
                itemColor = "#AAAAAA"; // 灰色
                break;

            default:
                itemName = "Unknown";
                itemColor = "#FFFFFF";
                break;
        }

        // ! = 黄色
        // アイテム名 = 種類ごとの色
        string message =
            $"<color=#FFD700>!</color> {amount} <color={itemColor}>{itemName}</color> added.";

        AddMessage(message);
    }

    // =========================================================
    // クラフト成功ログ
    // =========================================================
    public void AddCraftLog(string itemName, int amount)
    {
        // ! = 黄色
        // クラフトしたアイテム = 白色
        string message =
            $"<color=#FFD700>!</color> You crafted {amount} <color=#FFFFFF>{itemName}</color>.";

        AddMessage(message);
    }

    // =========================================================
    // エラーログ
    // =========================================================
    public void AddErrorLog(string message)
    {
        // エラーは ! も文章も赤色
        string errorMessage =
            $"<color=#FF0000>! {message}</color>";

        AddMessage(errorMessage);
    }

    // =========================================================
    // ログ追加共通処理
    // =========================================================
    private void AddMessage(string message)
    {
        // 新しいログを先頭に追加
        logs.Insert(0, message);

        if (logs.Count > maxLogs)
        {
            logs.RemoveAt(logs.Count - 1);
        }

        // 最新ログ（先頭）を表示
        topIndex = 0;

        RefreshLog();
    }

    // =========================================================
    // ログスクロール
    // =========================================================
    public void Scroll(int direction)
    {
        topIndex += direction;

        topIndex = Mathf.Clamp(
            topIndex,
            0,
            Mathf.Max(0, logs.Count - visibleLines)
        );

        RefreshLog();
    }

    // =========================================================
    // ログ表示更新
    // =========================================================
    private void RefreshLog()
    {
        if (logText == null)
        {
            return;
        }

        logText.text = "";

        int end = Mathf.Min(
            topIndex + visibleLines,
            logs.Count
        );

        for (int i = topIndex; i < end; i++)
        {
            logText.text += logs[i] + "\n";
        }
    }
}