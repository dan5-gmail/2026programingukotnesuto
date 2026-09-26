using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GoalEnter : MonoBehaviour
{
    // ========================================
    // Player
    // ========================================

    [Header("Player")]

    [Tooltip("Goal到達後に生成するPlayer Prefab")]
    [SerializeField]
    private GameObject playerPrefab;

    [Tooltip("Playerの生成位置")]
    [SerializeField]
    private Transform playerSpawnPoint;


    // ========================================
    // Player Spawn Position
    // ========================================

    [Header("Player Spawn Position")]

    [Tooltip("PlayerSpawnPointを使わず、直接座標を指定する")]
    [SerializeField]
    private bool useDirectPosition = false;

    [Tooltip("直接指定するPlayerの生成座標")]
    [SerializeField]
    private Vector3 playerSpawnPosition;


    // ========================================
    // Player Scale
    // ========================================

    [Header("Player Scale")]

    [Tooltip("生成したPlayerのScale")]
    [SerializeField]
    private Vector3 playerScale = Vector3.one;


    // ========================================
    // Editor
    // ========================================

    [Header("Editor")]

    [Tooltip("現在ステージを編集しているEditor")]
    [SerializeField]
    private GameObject editor;

    [Tooltip("Editorの移動スクリプト")]
    [SerializeField]
    private MonoBehaviour editorMoveScript;


    // ========================================
    // Camera
    // ========================================

    [Header("Camera")]

    [Tooltip("Editor操作時に使用するMain Camera")]
    [SerializeField]
    private Camera editorCamera;


    // ========================================
    // Fade Cube
    // ========================================

    [Header("Fade Cube")]

    [Tooltip("暗転・明転に使用する3D Fade Cube")]
    [SerializeField]
    private GameObject fadeCube;

    [Tooltip("暗転・明転にかかる時間")]
    [SerializeField]
    private float fadeDuration = 1f;


    // ========================================
    // Editor → Player Text
    // ========================================

    [Header("Editor → Player Text")]

    [Tooltip("EditorからPlayerへ移行するときに表示する3D TextMeshPro")]
    [SerializeField]
    private GameObject editorGoalTextObject;

    [Tooltip("Editor→Player移行時に表示する文章")]
    [SerializeField]
    private string transitionMessage =
        "EditorからPlayerへ移り変わる";

    [Tooltip("移行中テキストの点滅速度")]
    [SerializeField]
    private float transitionBlinkSpeed = 2f;

    [Tooltip("移行中テキストの最低Alpha")]
    [Range(0f, 1f)]
    [SerializeField]
    private float transitionMinAlpha = 0.65f;


    // ========================================
    // Game Clear Scene
    // ========================================

    [Header("Game Clear Scene")]

    [Tooltip("PlayerがGoalに到達したときにロードするScene名")]
    [SerializeField]
    private string gameClearSceneName = "GameClearScene";


    // ========================================
    // 内部変数
    // ========================================

    private bool triggered = false;

    // Player Goalの二重実行防止
    private bool playerGoalTriggered = false;

    private GameObject editorObject;
    private GameObject spawnedPlayer;

    private Camera playerCamera;
    private PlayerCameraFollow playerCameraFollow;

    private MonoBehaviour playerMoveScript;

    private Renderer fadeRenderer;
    private Material fadeMaterial;

    // Editor → Player用TMP
    private TextMeshPro EditorGoaltext;

    private Coroutine transitionBlinkCoroutine;


    // ========================================
    // Start
    // ========================================

    private void Start()
    {
        // ========================================
        // Editor
        // ========================================

        if (editor != null)
        {
            editorObject = editor.gameObject;
        }


        // ========================================
        // Editor → Player Text
        // ========================================

        if (editorGoalTextObject != null)
        {
            EditorGoaltext =
                editorGoalTextObject.GetComponent<TextMeshPro>();

            if (EditorGoaltext != null)
            {
                // ゲーム開始時は非表示
                editorGoalTextObject.SetActive(false);

                // Alphaは100%
                Color color =
                    EditorGoaltext.color;

                color.r = 1f;
                color.g = 1f;
                color.b = 1f;
                color.a = 1f;

                EditorGoaltext.color =
                    color;
            }
            else
            {
                Debug.LogError(
                    "GoalEnter : Editor Goal TextにTextMeshProがありません。"
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "GoalEnter : Editor Goal Textが設定されていません。"
            );
        }


        // ========================================
        // Fade Cube
        // ========================================

        if (fadeCube != null)
        {
            fadeRenderer =
                fadeCube.GetComponent<Renderer>();

            if (fadeRenderer != null)
            {
                fadeMaterial =
                    fadeRenderer.material;

                // ゲーム開始時は透明
                SetFadeAlpha(0f);

                fadeRenderer.enabled = true;
            }
            else
            {
                Debug.LogError(
                    "GoalEnter : Fade CubeにRendererがありません。"
                );
            }
        }
        else
        {
            Debug.LogError(
                "GoalEnter : Fade Cubeが設定されていません。"
            );
        }
    }


    // ========================================
    // Goal Trigger
    // ========================================

    private void OnTriggerEnter(Collider other)
    {
        // ========================================
        // すでにGoal処理中なら無視
        // ========================================

        if (triggered)
            return;


        // ========================================
        // Editor判定
        // ========================================

        if (editor != null)
        {
            if (other.gameObject != editor)
                return;
        }


        // ========================================
        // Goal処理開始
        // ========================================

        triggered = true;

        editorObject =
            other.gameObject;

        Debug.Log(
            "GoalEnter : EditorがGoalに到達しました。"
        );

        StartCoroutine(
            GoalSequence()
        );
    }


    private void OnTriggerExit(Collider other)
    {
        if (editor != null)
        {
            if (other.gameObject != editor)
                return;
        }

        // Editorがゴールから離れたらリセット
        triggered = false;

        Debug.Log(
            "GoalEnter : Editorがゴールから離れました。判定をリセットします。"
        );
    }


    // ========================================
    // Goal Sequence
    // Editor → Player
    // ========================================

    private IEnumerator GoalSequence()
    {
        // ========================================
        // ① Editor操作停止
        // ========================================

        if (editorMoveScript != null)
        {
            editorMoveScript.enabled = false;

            Debug.Log(
                "GoalEnter : Editor操作を停止しました。"
            );
        }


        // ========================================
        // ② 暗転
        // ========================================

        if (fadeCube != null)
        {
            fadeCube.SetActive(true);

            if (fadeRenderer != null)
            {
                fadeRenderer.enabled = true;
            }
        }

        yield return StartCoroutine(
            FadeTo(1f)
        );


        // ========================================
        // ③ Editor → Player テキスト表示
        // ========================================

        ShowTransitionText();


        // ========================================
        // ④ Editor非表示
        // ========================================

        if (editorObject != null)
        {
            editorObject.SetActive(false);
        }
        else if (editor != null)
        {
            editor.SetActive(false);
        }


        // ========================================
        // ⑤ Player Prefab確認
        // ========================================

        if (playerPrefab == null)
        {
            Debug.LogError(
                "GoalEnter : Player Prefabが設定されていません。"
            );

            HideEditorGoalText();
            RestoreEditor();

            yield break;
        }


        // ========================================
        // ⑥ Player生成位置確認
        // ========================================

        Vector3 spawnPosition;

        if (useDirectPosition)
        {
            spawnPosition =
                playerSpawnPosition;
        }
        else
        {
            if (playerSpawnPoint == null)
            {
                Debug.LogError(
                    "GoalEnter : Player Spawn Pointが設定されていません。"
                );

                HideEditorGoalText();
                RestoreEditor();

                yield break;
            }

            spawnPosition =
                playerSpawnPoint.position;
        }


        // ========================================
        // ⑦ Player生成
        // ========================================

        spawnedPlayer =
            Instantiate(
                playerPrefab,
                spawnPosition,
                Quaternion.identity
            );

        spawnedPlayer.transform.localScale =
            playerScale;

        spawnedPlayer.transform.position =
            spawnPosition;

        Debug.Log(
            $"GoalEnter : Playerを生成しました。位置 = {spawnPosition}"
        );


        // ========================================
        // ⑧ Player自動移動取得
        // ========================================

        PPlayerAutoMove autoMove =
            spawnedPlayer.GetComponent<PPlayerAutoMove>();

        if (autoMove != null)
        {
            playerMoveScript =
                autoMove;

            // 生成直後は停止
            autoMove.enabled = false;

            Debug.Log(
                "GoalEnter : Playerの自動移動を一時停止しました。"
            );
        }
        else
        {
            playerMoveScript = null;

            Debug.LogWarning(
                "GoalEnter : PlayerにPPlayerAutoMoveがありません。"
            );
        }


        // ========================================
        // ⑨ Animator初期化
        // ========================================

        Animator playerAnimator =
            spawnedPlayer.GetComponentInChildren<Animator>();

        if (playerAnimator != null)
        {
            playerAnimator.SetBool(
                "IsWalking",
                false
            );

            playerAnimator.SetBool(
                "IsJumping",
                false
            );
        }


        // ========================================
        // ⑩ Player Camera取得
        // ========================================

        playerCamera =
            spawnedPlayer.GetComponentInChildren<Camera>(true);

        if (playerCamera == null)
        {
            Debug.LogError(
                "GoalEnter : Player Prefab内にCameraがありません。"
            );

            Destroy(spawnedPlayer);

            HideEditorGoalText();
            RestoreEditor();

            yield break;
        }


        // ========================================
        // ⑪ Player Camera Follow取得
        // ========================================

        playerCameraFollow =
            playerCamera.GetComponent<PlayerCameraFollow>();

        if (playerCameraFollow == null)
        {
            Debug.LogError(
                "GoalEnter : Player CameraにPlayerCameraFollowがありません。"
            );

            Destroy(spawnedPlayer);

            HideEditorGoalText();
            RestoreEditor();

            yield break;
        }


        // ========================================
        // ⑫ Player Camera Follow Target設定
        // ========================================

        playerCameraFollow.target =
            spawnedPlayer.transform;

        playerCameraFollow.enabled =
            true;

        Debug.Log(
            "GoalEnter : PlayerCameraFollowのTargetをPlayerに設定しました。"
        );


        // ========================================
        // ⑬ Player Camera ON
        // ========================================

        playerCamera.gameObject.SetActive(true);
        playerCamera.enabled = true;

        Debug.Log(
            "GoalEnter : Player Camera ON"
        );


        // ========================================
        // ⑭ Main Camera OFF
        // ========================================

        if (editorCamera != null)
        {
            editorCamera.enabled = false;
            editorCamera.gameObject.SetActive(false);

            Debug.Log(
                "GoalEnter : Main Camera OFF"
            );
        }


        // ========================================
        // ⑮ カメラ切り替えを確定
        // ========================================

        yield return null;


        // ========================================
        // ⑯ Editor → Player表示時間
        // ========================================

        yield return new WaitForSeconds(
            3f
        );


        // ========================================
        // ⑰ Editor → Playerテキストを消す
        // ========================================

        HideEditorGoalText();


        // ========================================
        // ⑱ 明転
        // ========================================

        yield return StartCoroutine(
            FadeTo(0f)
        );


        // ========================================
        // ⑲ Fade Cube非表示
        // ========================================

        if (fadeRenderer != null)
        {
            fadeRenderer.enabled = false;
        }

        if (fadeCube != null)
        {
            fadeCube.SetActive(false);
        }


        // ========================================
        // ⑳ Player開始前待機
        // ========================================

        yield return new WaitForSeconds(
            1f
        );


        // ========================================
        // ㉑ Player開始
        // ========================================

        if (playerMoveScript != null)
        {
            playerMoveScript.enabled = true;

            Debug.Log(
                "GoalEnter : Playerの自動移動を開始しました。"
            );
        }


        // ========================================
        // ㉒ Goal処理完了
        // ========================================

        Debug.Log(
            "GoalEnter : Goal処理完了。Playerステージへ移行しました。"
        );
    }


    // ========================================
    // Editor → Player テキスト表示
    // ========================================

    private void ShowTransitionText()
    {
        if (EditorGoaltext == null)
            return;

        EditorGoaltext.text =
            transitionMessage;

        Color color =
            EditorGoaltext.color;

        color.r = 1f;
        color.g = 1f;
        color.b = 1f;
        color.a = 1f;

        EditorGoaltext.color =
            color;

        EditorGoaltext.gameObject.SetActive(true);

        if (transitionBlinkCoroutine != null)
        {
            StopCoroutine(
                transitionBlinkCoroutine
            );
        }

        transitionBlinkCoroutine =
            StartCoroutine(
                BlinkTransitionText()
            );
    }


    // ========================================
    // Editor → Player テキスト点滅
    // ========================================

    private IEnumerator BlinkTransitionText()
    {
        while (
            EditorGoaltext != null &&
            EditorGoaltext.gameObject.activeSelf
        )
        {
            float alpha =
                transitionMinAlpha +
                (1f - transitionMinAlpha) *
                (Mathf.Sin(
                    Time.time *
                    transitionBlinkSpeed
                ) + 1f) *
                0.5f;

            Color color =
                EditorGoaltext.color;

            color.a =
                alpha;

            EditorGoaltext.color =
                color;

            yield return null;
        }
    }


    // ========================================
    // Editor → Player テキスト非表示
    // ========================================

    private void HideEditorGoalText()
    {
        if (transitionBlinkCoroutine != null)
        {
            StopCoroutine(
                transitionBlinkCoroutine
            );

            transitionBlinkCoroutine = null;
        }

        if (EditorGoaltext != null)
        {
            EditorGoaltext.gameObject.SetActive(false);

            Color color =
                EditorGoaltext.color;

            color.a = 1f;

            EditorGoaltext.color =
                color;
        }
    }


    // ========================================
    // Player Goal Clear
    // ========================================

    public void PlayerGoalClear()
    {
        // 二重実行防止
        if (playerGoalTriggered)
            return;

        playerGoalTriggered = true;

        StartCoroutine(
            PlayerGoalClearSequence()
        );
    }


    // ========================================
    // Player Goal Clear Sequence
    // ========================================

    private IEnumerator PlayerGoalClearSequence()
    {
        Debug.Log(
            "GoalEnter : PlayerがGoalに到達しました。"
        );


        // ========================================
        // ① Player停止
        // ========================================

        if (spawnedPlayer != null)
        {
            PPlayerAutoMove autoMove =
                spawnedPlayer.GetComponent<PPlayerAutoMove>();

            if (autoMove != null)
            {
                autoMove.StopPlayer();
            }
        }


        // ========================================
        // ② FadeCube表示
        // ========================================

        if (fadeCube != null)
        {
            fadeCube.SetActive(true);
        }

        if (fadeRenderer != null)
        {
            fadeRenderer.enabled = true;
        }


        // ========================================
        // ③ Editor → Playerテキストを消す
        // ========================================

        HideEditorGoalText();


        // ========================================
        // ④ 暗転
        // ========================================

        yield return StartCoroutine(
            FadeTo(1f)
        );


        // ========================================
        // ⑤ 暗転完了を1フレーム確定
        // ========================================

        SetFadeAlpha(1f);

        yield return null;


        // ========================================
        // ⑥ GameClearSceneへ移動
        // ========================================

        LoadGameClearScene();
    }


    // ========================================
    // Game Clear Sceneロード
    // ========================================

    private void LoadGameClearScene()
    {
        if (string.IsNullOrEmpty(gameClearSceneName))
        {
            Debug.LogError(
                "GoalEnter : Game Clear Scene名が設定されていません。"
            );

            return;
        }

        Debug.Log(
            $"GoalEnter : GameClearSceneへ移動します。Scene = {gameClearSceneName}"
        );

        SceneManager.LoadScene(
            gameClearSceneName,
            LoadSceneMode.Single
        );
    }


    // ========================================
    // Editor復帰
    // ========================================

    private void RestoreEditor()
    {
        // Main Camera ON
        if (editorCamera != null)
        {
            editorCamera.gameObject.SetActive(true);
            editorCamera.enabled = true;

            Debug.Log(
                "GoalEnter : Main Cameraを復帰しました。"
            );
        }


        // Editor ON
        if (editorObject != null)
        {
            editorObject.SetActive(true);
        }
        else if (editor != null)
        {
            editor.SetActive(true);
        }


        // Editor操作ON
        if (editorMoveScript != null)
        {
            editorMoveScript.enabled = true;
        }


        // Goalを再使用可能にする
        triggered = false;
    }


    // ========================================
    // Fade
    // ========================================

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeMaterial == null)
            yield break;

        float startAlpha =
            GetFadeAlpha();

        float elapsed = 0f;

        // fadeDurationが0以下でもエラーにしない
        if (fadeDuration <= 0f)
        {
            SetFadeAlpha(targetAlpha);
            yield break;
        }

        while (elapsed < fadeDuration)
        {
            elapsed +=
                Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    fadeDuration
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

        SetFadeAlpha(
            targetAlpha
        );
    }


    // ========================================
    // Fade Alpha設定
    // ========================================

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


    // ========================================
    // Fade Alpha取得
    // ========================================

    private float GetFadeAlpha()
    {
        if (fadeMaterial == null)
            return 0f;

        return fadeMaterial.color.a;
    }
}