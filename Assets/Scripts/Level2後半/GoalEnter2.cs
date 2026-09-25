using System.Collections;
using UnityEngine;

public class GoalEnter2 : MonoBehaviour
{
    [Header("Editor")]
    [SerializeField] private GameObject editor;
    [SerializeField] private MonoBehaviour editorMoveScript;

    [Header("Player本体")]
    [Tooltip("Scene上に配置してあるPlayer本体を指定してください")]
    [SerializeField] private GameObject player;

    [Tooltip("Playerを出現させる位置")]
    [SerializeField] private Transform playerSpawnPoint;

    [Tooltip("Player本体の出現時のScale")]
    [SerializeField] private Vector3 playerScale = Vector3.one;

    [Header("Player Camera")]
    [SerializeField] private Camera playerCamera;

    [Header("Editor Camera / Main Camera")]
    [SerializeField] private Camera editorCamera;

    [Header("Fade")]
    [SerializeField] private GameObject fadeCube;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Game Clear")]
    [SerializeField] private GameObject gameClearObject;

    [Header("Editor Respawn")]
    [SerializeField] private Transform editorRespawnPoint;

    [Header("Debug")]
    [SerializeField] private bool debugLog = true;

    // =========================
    // 内部変数
    // =========================

    private PPlayerAutoMove2 playerMove;

    private Renderer fadeRenderer;
    private Material fadeMaterial;

    private bool editorGoalTriggered;
    private bool playerGoalTriggered;
    private bool playerFailedTriggered;
    private bool processing;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        // -------------------------
        // FadeCube
        // -------------------------

        if (fadeCube != null)
        {
            fadeRenderer = fadeCube.GetComponent<Renderer>();

            if (fadeRenderer != null)
            {
                fadeMaterial = fadeRenderer.material;

                SetFadeAlpha(0f);
                fadeRenderer.enabled = false;
            }
        }

        // -------------------------
        // Game Clear
        // -------------------------

        if (gameClearObject != null)
        {
            gameClearObject.SetActive(false);
        }

        // -------------------------
        // Editor Camera
        // -------------------------

        if (editorCamera != null)
        {
            editorCamera.gameObject.SetActive(true);
            editorCamera.enabled = true;
        }

        // -------------------------
        // Player Camera
        // -------------------------

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
            playerCamera.enabled = false;
        }

        // -------------------------
        // Player本体
        // 最初は非表示
        // -------------------------

        if (player != null)
        {
            playerMove = player.GetComponent<PPlayerAutoMove2>();

            if (playerMove == null)
            {
                Debug.LogError(
                    "GoalEnter2 : PlayerにPPlayerAutoMove2がありません。"
                );
            }

            player.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "GoalEnter2 : Player本体がInspectorに設定されていません。"
            );
        }

        if (debugLog)
        {
            Debug.Log("GoalEnter2 : 初期化完了。");
        }
    }


    // =========================================================
    // EditorがGoalに入った
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        if (processing)
            return;

        if (other.gameObject != editor)
            return;

        if (editorGoalTriggered)
            return;

        editorGoalTriggered = true;
        processing = true;

        if (debugLog)
        {
            Debug.Log("GoalEnter2 : EditorがGoalに到達！");
        }

        StartCoroutine(EditorGoalSequence());
    }


    // =========================================================
    // Editor → Player
    // =========================================================

    private IEnumerator EditorGoalSequence()
    {
        // -----------------------------------------------------
        // 1. Editor停止
        // -----------------------------------------------------

        if (editorMoveScript != null)
        {
            editorMoveScript.enabled = false;
        }

        // -----------------------------------------------------
        // 2. Fade開始
        // -----------------------------------------------------

        EnableFade();

        yield return StartCoroutine(FadeTo(1f));

        // -----------------------------------------------------
        // 3. Editor非表示
        // -----------------------------------------------------

        if (editor != null)
        {
            editor.SetActive(false);
        }

        // -----------------------------------------------------
        // 4. Player本体チェック
        // -----------------------------------------------------

        if (player == null)
        {
            Debug.LogError(
                "GoalEnter2 : Player本体がありません。"
            );

            yield return StartCoroutine(FadeTo(0f));

            DisableFade();

            processing = false;
            editorGoalTriggered = false;

            yield break;
        }

        // -----------------------------------------------------
        // 5. Player本体の位置を設定
        // -----------------------------------------------------

        if (playerSpawnPoint != null)
        {
            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning(
                "GoalEnter2 : playerSpawnPointが設定されていません。"
            );
        }

        // -----------------------------------------------------
        // 6. Player Scale
        // -----------------------------------------------------

        player.transform.localScale = playerScale;

        // -----------------------------------------------------
        // 7. Rigidbodyをリセット
        // -----------------------------------------------------

        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // -----------------------------------------------------
        // 8. Player本体を表示
        // -----------------------------------------------------

        player.SetActive(true);

        // -----------------------------------------------------
        // 9. PPlayerAutoMove2取得
        // -----------------------------------------------------

        playerMove = player.GetComponent<PPlayerAutoMove2>();

        if (playerMove == null)
        {
            Debug.LogError(
                "GoalEnter2 : Player本体にPPlayerAutoMove2がありません。"
            );

            player.SetActive(false);

            yield return StartCoroutine(FadeTo(0f));

            DisableFade();

            processing = false;
            editorGoalTriggered = false;

            yield break;
        }

        // -----------------------------------------------------
        // 10. Playerを一旦停止
        // -----------------------------------------------------

        playerMove.StopPlayer();

        // -----------------------------------------------------
        // 11. Player Camera
        // -----------------------------------------------------

        if (playerCamera != null)
        {
            PlayerCameraFollow follow =
                playerCamera.GetComponent<PlayerCameraFollow>();

            if (follow != null)
            {
                follow.target = player.transform;

                if (debugLog)
                {
                    Debug.Log(
                        "GoalEnter2 : PlayerCameraFollowのTargetをPlayer本体に設定。"
                    );
                }
            }
            else
            {
                Debug.LogWarning(
                    "GoalEnter2 : Player CameraにPlayerCameraFollowがありません。"
                );
            }

            // Player Camera ON
            playerCamera.gameObject.SetActive(true);
            playerCamera.enabled = true;
        }
        else
        {
            Debug.LogError(
                "GoalEnter2 : Player CameraがInspectorに設定されていません。"
            );
        }

        // -----------------------------------------------------
        // 12. Editor Camera OFF
        // -----------------------------------------------------

        if (editorCamera != null)
        {
            editorCamera.enabled = false;
            editorCamera.gameObject.SetActive(false);
        }

        // -----------------------------------------------------
        // 13. 1フレーム待つ
        // カメラ切り替えをUnityに反映させる
        // -----------------------------------------------------

        yield return null;

        // -----------------------------------------------------
        // 14. 少し待つ
        // -----------------------------------------------------

        yield return new WaitForSeconds(0.5f);

        // -----------------------------------------------------
        // 15. Fade解除
        // -----------------------------------------------------

        yield return StartCoroutine(FadeTo(0f));

        DisableFade();

        // -----------------------------------------------------
        // 16. Player開始
        // -----------------------------------------------------

        playerMove.ResumePlayer();

        if (debugLog)
        {
            Debug.Log(
                "GoalEnter2 : Player本体を表示してゲーム開始！"
            );
        }

        processing = false;
    }


    // =========================================================
    // PlayerがGoalに到達
    // =========================================================

    public void PlayerGoalClear()
    {
        if (playerGoalTriggered)
            return;

        if (processing)
            return;

        playerGoalTriggered = true;
        processing = true;

        if (debugLog)
        {
            Debug.Log(
                "GoalEnter2 : PlayerがGoalに到達！"
            );
        }

        StartCoroutine(PlayerGoalSequence());
    }


    // =========================================================
    // Player Goal Clear
    // =========================================================

    private IEnumerator PlayerGoalSequence()
    {
        // Player停止
        if (playerMove != null)
        {
            playerMove.StopPlayer();
        }

        // Fade
        EnableFade();

        yield return StartCoroutine(FadeTo(1f));

        // Player非表示
        if (player != null)
        {
            player.SetActive(false);
        }

        // Game Clear表示
        if (gameClearObject != null)
        {
            gameClearObject.SetActive(true);
        }

        if (debugLog)
        {
            Debug.Log(
                "GoalEnter2 : GAME CLEAR！"
            );
        }

        processing = false;
    }


    // =========================================================
    // Player失敗
    // =========================================================

    public void PlayerFailed()
    {
        if (playerFailedTriggered)
            return;

        if (processing)
            return;

        playerFailedTriggered = true;
        processing = true;

        if (debugLog)
        {
            Debug.Log(
                "GoalEnter2 : Player失敗！"
            );
        }

        StartCoroutine(PlayerFailedSequence());
    }


    // =========================================================
    // Player失敗 → Editor復帰
    // =========================================================

    private IEnumerator PlayerFailedSequence()
    {
        // Player停止
        if (playerMove != null)
        {
            playerMove.FailPlayer();
        }

        // Fade
        EnableFade();

        yield return StartCoroutine(FadeTo(1f));

        // Player非表示
        if (player != null)
        {
            player.SetActive(false);
        }

        // Editor復帰
        EditorRespawn();

        // Player Camera OFF
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
        }

        // Editor Camera ON
        if (editorCamera != null)
        {
            editorCamera.gameObject.SetActive(true);
            editorCamera.enabled = true;
        }

        // Editor表示
        if (editor != null)
        {
            editor.SetActive(true);
        }

        // Editor操作復帰
        if (editorMoveScript != null)
        {
            editorMoveScript.enabled = true;
        }

        yield return null;

        // Fade解除
        yield return StartCoroutine(FadeTo(0f));

        DisableFade();

        editorGoalTriggered = false;
        playerGoalTriggered = false;
        playerFailedTriggered = false;
        processing = false;

        if (debugLog)
        {
            Debug.Log(
                "GoalEnter2 : Editorへ復帰完了。"
            );
        }
    }


    // =========================================================
    // Editor Respawn
    // =========================================================

    private void EditorRespawn()
    {
        if (editor == null)
            return;

        if (editorRespawnPoint == null)
            return;

        editor.transform.position =
            editorRespawnPoint.position;

        editor.transform.rotation =
            editorRespawnPoint.rotation;

        Rigidbody rb =
            editor.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


    // =========================================================
    // Fade ON
    // =========================================================

    private void EnableFade()
    {
        if (fadeRenderer == null)
            return;

        fadeRenderer.enabled = true;
        SetFadeAlpha(0f);
    }


    // =========================================================
    // Fade OFF
    // =========================================================

    private void DisableFade()
    {
        if (fadeRenderer == null)
            return;

        SetFadeAlpha(0f);
        fadeRenderer.enabled = false;
    }


    // =========================================================
    // Fade
    // =========================================================

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeRenderer == null || fadeMaterial == null)
            yield break;

        fadeRenderer.enabled = true;

        Color color = fadeMaterial.color;

        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / fadeDuration
            );

            color.a = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                t
            );

            fadeMaterial.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        fadeMaterial.color = color;
    }


    // =========================================================
    // Fade Alpha
    // =========================================================

    private void SetFadeAlpha(float alpha)
    {
        if (fadeMaterial == null)
            return;

        Color color = fadeMaterial.color;
        color.a = alpha;
        fadeMaterial.color = color;
    }
}