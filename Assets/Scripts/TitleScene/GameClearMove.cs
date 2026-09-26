using UnityEngine;

public class GameClearMove : MonoBehaviour
{
    [Header("移動設定")]

    [Tooltip("最終的に到達するY座標")]
    [SerializeField]
    private float targetY = 2f;

    [Tooltip("移動にかかる時間")]
    [SerializeField]
    private float moveDuration = 0.8f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float elapsedTime = 0f;

    private void Start()
    {
        // 開始位置を保存
        startPosition = transform.position;

        // X・Zはそのまま、Yだけ変更
        targetPosition = new Vector3(
            startPosition.x,
            targetY,
            startPosition.z
        );
    }

    private void Update()
    {
        if (elapsedTime >= moveDuration)
        {
            // 最後は必ず指定位置に固定
            transform.position = targetPosition;
            return;
        }

        elapsedTime += Time.deltaTime;

        float t =
            Mathf.Clamp01(
                elapsedTime / moveDuration
            );

        // なめらかに減速して到着
        t = Mathf.SmoothStep(0f, 1f, t);

        transform.position =
            Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );
    }
}