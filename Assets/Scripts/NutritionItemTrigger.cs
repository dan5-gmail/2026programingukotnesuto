using UnityEngine;

public class NutritionItemTrigger : MonoBehaviour
{
    [Header("移動する根っこ")]
    [SerializeField] private GameObject rootObject;

    [Header("移動スクリプト")]
    [SerializeField] private RootMovement rootMovement;

    [Header("トリガーする特定のNutritionItem（指定しない場合は全て）")]
    [SerializeField] private GameObject specificNutritionItem = null;

    [Header("トリガーするNutritionItemのTag（空の場合は全て）")]
    [SerializeField] private string requiredTag = "NutritionItem";

    [Header("一度だけトリガーするか")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 一度きり設定で既にトリガー済みなら無視
        if (triggerOnce && hasTriggered)
            return;

        // NutritionItemが触れたかチェック
        NutritionItem nutritionItem = other.GetComponent<NutritionItem>();
        if (nutritionItem == null)
            return;

        // 特定のNutritionItemチェック（指定されている場合）
        if (specificNutritionItem != null)
        {
            if (other.gameObject != specificNutritionItem)
            {
                Debug.Log("NutritionItemTrigger : 指定したNutritionItemではありません。無視します。");
                return;
            }
        }

        // Tagチェック（指定されている場合）
        if (!string.IsNullOrEmpty(requiredTag))
        {
            if (!other.CompareTag(requiredTag))
            {
                Debug.Log($"NutritionItemTrigger : Tag '{requiredTag}' ではないNutritionItemが触れました。無視します。");
                return;
            }
        }

        // 根っこを動かす
        if (rootMovement != null)
        {
            rootMovement.StartMoving();
        }

        hasTriggered = true;

        Debug.Log("NutritionItemTrigger : NutritionItemがターゲットに触れました。根っこを移動開始。");
    }

    // トリガーをリセット（必要な場合）
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}