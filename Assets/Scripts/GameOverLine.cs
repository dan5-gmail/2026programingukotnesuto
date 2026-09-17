using System.Collections;
using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    [Header("Editor")]
    [Tooltip("Scene上のEditor")]
    [SerializeField]
    private GameObject editor;

    [Tooltip("Editorを戻す初期地点")]
    [SerializeField]
    private Transform editorInitialPoint;

    [Tooltip("直接座標を使用する")]
    [SerializeField]
    private bool useDirectEditorPosition = false;

    [Tooltip("直接指定するEditorの座標")]
    [SerializeField]
    private Vector3 editorInitialPosition;

    [Tooltip("Editorの移動スクリプト")]
    [SerializeField]
    private MonoBehaviour editorMoveScript;

    [Header("Goal Reset")]
    [Tooltip("GoalEnterスクリプト")]
    [SerializeField]
    private GoalEnter goalEnter;


    [Header("Editor Log")]
    [Tooltip("Editor復帰完了から赤ログを表示するまでの時間")]
    [SerializeField]
    private float errorLogDelay = 0.5f;


    private Rigidbody editorRb;

    private Camera mainCamera;

    private bool resetting = false;


    private void Awake()
    {
        // EditorのRigidbody
        if (editor != null)
        {
            editorRb = editor.GetComponent<Rigidbody>();
        }

        // EditorのカメラはMainCamera
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning(
                "GameOverLine : MainCameraが見つかりません。"
            );
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (resetting)
            return;


        // Playerかどうかを判定
        PPlayerAutoMove player =
            other.GetComponentInParent<PPlayerAutoMove>();


        if (player != null)
        {
            resetting = true;

            Debug.Log(
                "GameOverLine : Playerの落下を検知。"
            );

            StartCoroutine(
                PlayerFallSequence(player.gameObject)
            );
        }
    }


    private IEnumerator PlayerFallSequence(GameObject fallenPlayer)
    {
        // =========================================
        // 1. PlayerCameraをOFF
        // =========================================

        if (fallenPlayer != null)
        {
            Camera playerCamera =
                fallenPlayer.GetComponentInChildren<Camera>(true);

            if (playerCamera != null)
            {
                playerCamera.enabled = false;

                Debug.Log(
                    "GameOverLine : PlayerCamera OFF"
                );
            }
        }


        // =========================================
        // 2. MainCameraをON
        // =========================================

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            mainCamera.enabled = true;

            Debug.Log(
                "GameOverLine : MainCamera ON"
            );
        }


        // =========================================
        // 3. Playerを削除
        // =========================================

        if (fallenPlayer != null)
        {
            Destroy(fallenPlayer);

            Debug.Log(
                "GameOverLine : Player Destroy"
            );
        }


        // =========================================
        // 4. Editorを初期位置へ戻す
        // =========================================

        ResetEditor();


        // =========================================
        // 5. Editorを表示
        // =========================================

        if (editor != null)
        {
            editor.SetActive(true);

            Debug.Log(
                "GameOverLine : Editor ON"
            );
        }


        // =========================================
        // 6. Editor操作を有効化
        // =========================================

        if (editorMoveScript != null)
        {
            editorMoveScript.enabled = true;

            Debug.Log(
                "GameOverLine : Editor操作 ON"
            );
        }


        // =========================================
        // 7. ゴール判定をリセット
        // =========================================

        if (goalEnter != null)
        {
            // GoalEnterのtriggeredフラグをリセット
            // Reflectionを使用してprivateフィールドにアクセス
            var goalEnterType = goalEnter.GetType();
            var triggeredField = goalEnterType.GetField("triggered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (triggeredField != null)
            {
                triggeredField.SetValue(goalEnter, false);
                Debug.Log("GameOverLine : ゴール判定をリセットしました");
            }
        }


        // =========================================
        // ★ここが重要★
        //
        // Editorが表示され、
        // 初期位置へ戻り、
        // 操作可能になった後で0.5秒待つ
        // =========================================

        yield return new WaitForSeconds(errorLogDelay);


        // =========================================
        // 8. 0.5秒後に赤いEditorLog
        // =========================================

        Debug.Log("GameOverLine : EditorLog追加を開始");

        if (EditorLogManager.Instance != null)
        {
            Debug.Log("GameOverLine : EditorLogManager.Instance found");
            EditorLogManager.Instance.AddErrorLog(
                "! Player fell into the abyss."
            );

            Debug.Log(
                "GameOverLine : 赤いEditorLogを表示しました。"
            );
        }
        else
        {
            Debug.LogWarning(
                "GameOverLine : EditorLogManagerが見つかりません。"
            );
        }


        // =========================================
        // 9. リセット終了
        // =========================================

        resetting = false;
    }


    private void ResetEditor()
    {
        if (editor == null)
        {
            Debug.LogWarning(
                "GameOverLine : Editorが設定されていません。"
            );

            return;
        }

        Vector3 resetPosition;
        Quaternion resetRotation = Quaternion.identity;

        if (useDirectEditorPosition)
        {
            resetPosition = editorInitialPosition;
            resetRotation = Quaternion.identity;
        }
        else
        {
            if (editorInitialPoint == null)
            {
                Debug.LogWarning(
                    "GameOverLine : EditorInitialPointが設定されていません。"
                );

                return;
            }

            resetPosition = editorInitialPoint.position;
            resetRotation = editorInitialPoint.rotation;
        }


        // =========================================
        // Rigidbodyがある場合
        // =========================================

        if (editorRb != null)
        {
            // 速度を完全停止
            editorRb.linearVelocity = Vector3.zero;
            editorRb.angularVelocity = Vector3.zero;


            // 初期位置へ戻す
            editorRb.position = resetPosition;


            // 初期回転へ戻す
            editorRb.rotation = resetRotation;


            // もう一度速度を0にする
            editorRb.linearVelocity = Vector3.zero;
            editorRb.angularVelocity = Vector3.zero;
        }
        else
        {
            // Rigidbodyがない場合
            editor.transform.position = resetPosition;
            editor.transform.rotation = resetRotation;
        }


        // Physicsへ即時反映
        Physics.SyncTransforms();


        Debug.Log(
            "GameOverLine : Editorを初期位置へ戻しました。"
        );
    }
}