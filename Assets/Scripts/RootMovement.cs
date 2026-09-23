using System.Collections;
using UnityEngine;

public class RootMovement : MonoBehaviour
{
    [Header("開始座標")]
    [SerializeField] private Vector3 startPosition;

    [Header("終了座標")]
    [SerializeField] private Vector3 endPosition;

    [Header("移動時間")]
    [SerializeField] private float moveDuration = 2f;

    [Header("移動方式")]
    [SerializeField] private bool useLinear = false;

    [Header("ループするか")]
    [SerializeField] private bool loop = false;

    [Header("ループ間隔")]
    [SerializeField] private float loopDelay = 1f;

    private bool isMoving = false;
    private bool isAtEnd = false;

    public void StartMoving()
    {
        if (isMoving)
            return;

        if (isAtEnd)
        {
            // 終了位置から開始位置へ戻る
            StartCoroutine(MoveRoot(endPosition, startPosition, moveDuration, true));
        }
        else
        {
            // 開始位置から終了位置へ
            StartCoroutine(MoveRoot(startPosition, endPosition, moveDuration, false));
        }
    }

    private IEnumerator MoveRoot(Vector3 from, Vector3 to, float duration, bool isReturn)
    {
        isMoving = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 線形かスムーズか
            if (useLinear)
            {
                transform.position = Vector3.Lerp(from, to, t);
            }
            else
            {
                // スムーズに移動
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                transform.position = Vector3.Lerp(from, to, smoothT);
            }

            yield return null;
        }

        // 正確な終了位置に設定
        transform.position = to;
        isMoving = false;

        // 状態更新
        if (isReturn)
        {
            isAtEnd = false;
        }
        else
        {
            isAtEnd = true;
        }

        Debug.Log($"RootMovement : 根っこが{"終了" + (!isReturn ? "位置" : "開始位置")}に到達しました。");

        // ループ設定の場合
        if (loop && !isReturn)
        {
            yield return new WaitForSeconds(loopDelay);
            StartMoving();
        }
    }

    // 手動で位置をリセット
    public void ResetToStart()
    {
        transform.position = startPosition;
        isAtEnd = false;
        isMoving = false;
    }

    // 手動で終了位置に設定
    public void SetToEnd()
    {
        transform.position = endPosition;
        isAtEnd = true;
        isMoving = false;
    }

    // 現在の状態を取得
    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsAtEnd()
    {
        return isAtEnd;
    }
}