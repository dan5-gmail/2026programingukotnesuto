using UnityEngine;
using System.Collections;

public class GoalEnter2 : MonoBehaviour
{
    // =========================================================
    // Editor
    // =========================================================

    [Header("Editor")]
    [SerializeField]
    private GameObject editor;

    [SerializeField]
    private MonoBehaviour editorMoveScript;


    // =========================================================
    // Player
    // =========================================================

    [Header("Player")]
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Transform playerSpawnPoint;

    [SerializeField]
    private Vector3 playerScale = Vector3.one;


    // =========================================================
    // Player Camera
    // =========================================================

    [Header("Player Camera")]
    [SerializeField]
    private Camera playerCamera;


    // =========================================================
    // Editor Camera
    // =========================================================

    [Header("Editor Camera")]
    [SerializeField]
    private Camera editorCamera;


    // =========================================================
    // Fade Cube
    // =========================================================

    [Header("Fade Cube")]
    [Tooltip("透明度をコードで変更するFadeCube")]
    [SerializeField]
    private GameObject fadeCube;

    [SerializeField]
    private float fadeDuration = 3f;


    // =========================================================
    // Game Clear
    // =========================================================

    [Header("Game Clear")]
    [SerializeField]
    private GameObject gameClearObject;


    // =========================================================
    // Editor Respawn
    // =========================================================

    [Header("Editor Respawn")]
    [Tooltip("Player失敗時にEditorを戻す位置")]
    [SerializeField]
    private Transform editorRespawnPoint;


    // =========================================================
    // 内部変数
    // =========================================================

    private GameObject spawnedPlayer;

    private PPlayerAutoMove2 playerMove;

    private PlayerCameraFollow playerCameraFollow;


    // =========================================================
    // Fade内部
    // =========================================================

    private Renderer fadeRenderer;

    private Material fadeMaterial;


    // =========================================================
    // Goal状態
    // =========================================================

    // EditorがGoalに到達したか
    private bool editorGoalTriggered = false;

    // PlayerがGoalに到達したか
    private bool playerGoalTriggered = false;

    // Player失敗処理中か
    private bool playerFailedTriggered = false;

    // 現在Goal処理中か
    private bool processing = false;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        // =====================================================
        // FadeCube初期化
        // =====================================================

        if (fadeCube != null)
        {
            fadeRenderer =
                fadeCube.GetComponent<Renderer>();

            if (fadeRenderer != null)
            {
                // Renderer.materialを使用
                // このMaterialだけをGoalEnter2側で操作する
                fadeMaterial =
                    fadeRenderer.material;

                // 最初は完全透明
                SetFadeAlpha(0f);

                // GameObjectは消さない
                // RendererだけOFFにする
                fadeRenderer.enabled = false;
            }
            else
            {
                Debug.LogWarning(
                    "GoalEnter2 : FadeCubeにRendererがありません。"
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "GoalEnter2 : FadeCubeが設定されていません。"
            );
        }


        // =====================================================
        // Game Clear初期化
        // =====================================================

        if (gameClearObject != null)
        {
            gameClearObject.SetActive(false);
        }


        // =====================================================
        // 初期カメラ状態
        // =====================================================

        if (editorCamera != null)
        {
            editorCamera.gameObject.SetActive(true);
            editorCamera.enabled = true;
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
        }
    }


    // =========================================================
    // Goal Trigger
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        if (processing)
            return;


        // =====================================================
        // Editor Goal
        // =====================================================

        if (!editorGoalTriggered)
        {
            if (editor != null &&
                other.gameObject == editor)
            {
                editorGoalTriggered = true;

                Debug.Log(
                    "GoalEnter2 : EditorがGoalに到達しました。"
                );

                StartCoroutine(
                    EditorGoalSequence()
                );

                return;
            }
        }
    }


    // =========================================================
    // Editor → Player
    // =========================================================

    private IEnumerator EditorGoalSequence()
    {
        processing = true;

        playerFailedTriggered = false;
        playerGoalTriggered = false;


        // =====================================================
        // ① Editor停止
        // =====================================================

        if (editorMoveScript != null)
        {
            editorMoveScript.enabled = false;
        }


        // =====================================================
        // ② FadeCube Renderer ON
        // =====================================================
        //
        // FadeCubeのGameObjectはON/OFFしない
        // RendererだけをONにする
        // =====================================================

        if (fadeRenderer != null)
        {
            fadeRenderer.enabled = true;
        }


        // =====================================================
        // ③ 暗転
        // =====================================================

        yield return StartCoroutine(
            FadeTo(1f)
        );


        // =====================================================
        // ④ Editor非表示
        // =====================================================

        if (editor != null)
        {
            editor.SetActive(false);
        }


        // =====================================================
        // ⑤ Player Prefab確認
        // =====================================================

        if (playerPrefab == null)
        {
            Debug.LogError(
                "GoalEnter2 : Player Prefabが設定されていません。"
            );

            yield return StartCoroutine(
                EditorRespawn()
            );

            yield break;
        }


        // =====================================================
        // ⑥ Spawn Point確認
        // =====================================================

        if (playerSpawnPoint == null)
        {
            Debug.LogError(
                "GoalEnter2 : Player Spawn Pointが設定されていません。"
            );

            yield return StartCoroutine(
                EditorRespawn()
            );

            yield break;
        }


        // =====================================================
        // ⑦ Player生成
        // =====================================================

        spawnedPlayer =
            Instantiate(
                playerPrefab,
                playerSpawnPoint.position,
                Quaternion.identity
            );

        spawnedPlayer.transform.localScale =
            playerScale;


        Debug.Log(
            "GoalEnter2 : Playerを生成しました。"
        );


        // =====================================================
        // ⑧ PPlayerAutoMove2取得
        // =====================================================

        playerMove =
            spawnedPlayer.GetComponent<PPlayerAutoMove2>();

        if (playerMove == null)
        {
            Debug.LogError(
                "GoalEnter2 : PlayerにPPlayerAutoMove2がありません。"
            );

            yield return StartCoroutine(
                PlayerFailedSequence()
            );

            yield break;
        }


        // =====================================================
        // ⑨ Player Camera確認
        // =====================================================

        if (playerCamera == null)
        {
            Debug.LogError(
                "GoalEnter2 : Player Cameraがありません。"
            );

            yield return StartCoroutine(
                PlayerFailedSequence()
            );

            yield break;
        }


        // =====================================================
        // ⑩ Player Camera Follow取得
        // =====================================================

        playerCameraFollow =
            playerCamera.GetComponent<PlayerCameraFollow>();

        if (playerCameraFollow != null)
        {
            playerCameraFollow.target =
                spawnedPlayer.transform;

            playerCameraFollow.enabled = true;
        }
        else
        {
            Debug.LogWarning(
                "GoalEnter2 : Player CameraにPlayerCameraFollowがありません。"
            );
        }


        // =====================================================
        // ⑪ Player Camera ON
        // =====================================================

        playerCamera.gameObject.SetActive(true);
        playerCamera.enabled = true;


        // =====================================================
        // ⑫ Editor Camera OFF
        // =====================================================

        if (editorCamera != null)
        {
            editorCamera.enabled = false;
            editorCamera.gameObject.SetActive(false);
        }


        // =====================================================
        // ⑬ カメラ切り替え確定
        // =====================================================

        yield return null;


        // =====================================================
        // ⑭ 少し待つ
        // =====================================================

        yield return new WaitForSeconds(1f);


        // =====================================================
        // ⑮ 明転
        // =====================================================

        yield return StartCoroutine(
            FadeTo(0f)
        );


        // =====================================================
        // ⑯ FadeCube Renderer OFF
        // =====================================================

        SetFadeAlpha(0f);

        if (fadeRenderer != null)
        {
            fadeRenderer.enabled = false;
        }


        // =====================================================
        // ⑰ Player開始
        // =====================================================

        if (playerMove != null)
        {
            playerMove.enabled = true;
        }


        processing = false;


        Debug.Log(
            "GoalEnter2 : Playerステージを開始しました。"
        );
    }


    // =========================================================
    // Player Goal Clear
    // =========================================================
    //
    // PPlayerAutoMove2から呼ばれる
    //
    // =========================================================

    public void PlayerGoalClear()
    {
        // 二重実行防止
        if (playerGoalTriggered)
            return;

        // Editor → Player移行中などは無視
        if (processing)
            return;


        playerGoalTriggered = true;


        Debug.Log(
            "GoalEnter2 : PlayerがGoalに到達しました。"
        );


        StartCoroutine(
            PlayerGoalSequence()
        );
    }


    // =========================================================
    // Player Goal
    // =========================================================

    private IEnumerator PlayerGoalSequence()
    {
        processing = true;


        // =====================================================
        // ① Player停止
        // =====================================================

        if (playerMove != null)
        {
            playerMove.StopPlayer();
        }


        // =====================================================
        // ② FadeCube Renderer ON
        // =====================================================

        if (fadeRenderer != null)
        {
            fadeRenderer.enabled = true;
        }


        // =====================================================
        // ③ 暗転
        // =====================================================

        yield return StartCoroutine(
            FadeTo(1f)
        );


        // =====================================================
        // ④ Player非表示
        // =====================================================

        if (spawnedPlayer != null)
        {
            spawnedPlayer.SetActive(false);
        }


        // =====================================================
        // ⑤ Game Clear
        // =====================================================

        if (gameClearObject != null)
        {
            gameClearObject.SetActive(true);
        }


        Debug.Log(
            "GoalEnter2 : GAME CLEAR!"
        );


        // =====================================================
        // ⑥ Scene移動なし
        // =====================================================
        //
        // MainGameSceneへの移動なし
        // TutorialClearなし
        //
        // このSceneでGame Clearを表示して終了
        // =====================================================

        processing = false;
    }


    // =========================================================
    // Player失敗
    // =========================================================
    //
    // 泥などの失敗システムから呼び出す
    //
    // =========================================================

    public void PlayerFailed()
    {
        // Goal到達済みなら失敗処理しない
        if (playerGoalTriggered)
            return;

        // 既に失敗処理中なら無視
        if (playerFailedTriggered)
            return;


        playerFailedTriggered = true;


        StartCoroutine(
            PlayerFailedSequence()
        );
    }


    // =========================================================
    // Player失敗処理
    // =========================================================

    private IEnumerator PlayerFailedSequence()
    {
        processing = true;


        Debug.Log(
            "GoalEnter2 : PlayerがGoalに到達できませんでした。"
        );


        // =====================================================
        // ① Player停止
        // =====================================================

        if (playerMove != null)
        {
            playerMove.StopPlayer();
        }


        // =====================================================
        // ② FadeCube Renderer ON
        // =====================================================

        if (fadeRenderer != null)
        {
            fadeRenderer.enabled = true;
        }


        // =====================================================
        // ③ 暗転
        // =====================================================

        yield return StartCoroutine(
            FadeTo(1f)
        );


        // =====================================================
        // ④ Player削除
        // =====================================================

        if (spawnedPlayer != null)
        {
            Destroy(
                spawnedPlayer
            );

            spawnedPlayer = null;
        }

        playerMove = null;
        playerCameraFollow = null;


        // =====================================================
        // ⑤ Editor確認
        // =====================================================

        if (editor == null)
        {
            Debug.LogError(
                "GoalEnter2 : Editorが設定されていません。"
            );

            yield break;
        }


        if (editorRespawnPoint == null)
        {
            Debug.LogError(
                "GoalEnter2 : Editor Respawn Pointが設定されていません。"
            );

            yield break;
        }


        // =====================================================
        // ⑥ Editor復帰
        // =====================================================

        editor.transform.position =
            editorRespawnPoint.position;

        editor.transform.rotation =
            editorRespawnPoint.rotation;

        editor.SetActive(true);


        // =====================================================
        // ⑦ Editor Camera ON
        // =====================================================

        if (editorCamera != null)
        {
            editorCamera.gameObject.SetActive(true);
            editorCamera.enabled = true;
        }


        // =====================================================
        // ⑧ Player Camera OFF
        // =====================================================

        if (playerCamera != null)
        {
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
        }


        // =====================================================
        // ⑨ Editor操作復帰
        // =====================================================

        if (editorMoveScript != null)
        {
            editorMoveScript.enabled = true;
        }


        // =====================================================
        // ⑩ EditorLog
        // =====================================================

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CraftErrorLog(
                "Player was unable to reach the goal."
            );
        }


        // =====================================================
        // ⑪ 明転
        // =====================================================

        yield return StartCoroutine(
            FadeTo(0f)
        );


        // =====================================================
        // ⑫ FadeCube Renderer OFF
        // =====================================================

        SetFadeAlpha(0f);

        if (fadeRenderer != null)
        {
            fadeRenderer.enabled = false;
        }


        // =====================================================
        // ⑬ 再挑戦可能に戻す
        // =====================================================

        editorGoalTriggered = false;
        playerGoalTriggered = false;
        playerFailedTriggered = false;

        processing = false;


        Debug.Log(
            "GoalEnter2 : Editorを復帰しました。再挑戦可能です。"
        );
    }


    // =========================================================
    // Editor Respawn
    // =========================================================

    private IEnumerator EditorRespawn()
    {
        if (editor == null ||
            editorRespawnPoint == null)
        {
            processing = false;
            yield break;
        }


        // =====================================================
        // Editor位置を戻す
        // =====================================================

        editor.transform.position =
            editorRespawnPoint.position;

        editor.transform.rotation =
            editorRespawnPoint.rotation;

        editor.SetActive(true);


        // =====================================================
        // Editor Camera ON
        // =====================================================

        if (editorCamera != null)
        {
            editorCamera.gameObject.SetActive(true);
            editorCamera.enabled = true;
        }


        // =====================================================
        // Player Camera OFF
        // =====================================================

        if (playerCamera != null)
        {
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
        }


        // =====================================================
        // Editor操作復帰
        // =====================================================

        if (editorMoveScript != null)
        {
            editorMoveScript.enabled = true;
        }


        // =====================================================
        // 明転
        // =====================================================

        yield return StartCoroutine(
            FadeTo(0f)
        );


        // =====================================================
        // FadeCube Renderer OFF
        // =====================================================

        SetFadeAlpha(0f);

        if (fadeRenderer != null)
        {
            fadeRenderer.enabled = false;
        }


        // =====================================================
        // 再挑戦可能
        // =====================================================

        editorGoalTriggered = false;
        playerGoalTriggered = false;
        playerFailedTriggered = false;

        processing = false;
    }


    // =========================================================
    // Fade
    // =========================================================

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeMaterial == null)
            yield break;


        float startAlpha =
            GetFadeAlpha();

        float elapsed = 0f;


        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed / fadeDuration
                );


            float alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );


            SetFadeAlpha(alpha);


            yield return null;
        }


        SetFadeAlpha(targetAlpha);
    }


    // =========================================================
    // Fade Alpha設定
    // =========================================================

    private void SetFadeAlpha(float alpha)
    {
        if (fadeMaterial == null)
            return;


        Color color =
            fadeMaterial.color;


        color.a =
            alpha;


        fadeMaterial.color =
            color;
    }


    // =========================================================
    // Fade Alpha取得
    // =========================================================

    private float GetFadeAlpha()
    {
        if (fadeMaterial == null)
            return 0f;


        return fadeMaterial.color.a;
    }
}