using System.Collections;
using UnityEngine;

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
    // 内部変数
    // ========================================

    private bool triggered = false;

    private GameObject editorObject;

    private GameObject spawnedPlayer;

    private Camera playerCamera;

    private PlayerCameraFollow playerCameraFollow;

    private MonoBehaviour playerMoveScript;

    private Renderer fadeRenderer;

    private Material fadeMaterial;


    // ========================================
    // Start
    // ========================================

    private void Start()
    {
        // ========================================
        // Editor Camera = MainCamera
        // ========================================

        if (editorCamera != null)
        {
            editorCamera.gameObject.SetActive(true);
            editorCamera.enabled = true;
        }
        else
        {
            Debug.LogError(
                "GoalEnter : Editor Camera（Main Camera）が設定されていません。"
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

        editorObject = other.gameObject;


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

        Debug.Log("GoalEnter : Editorがゴールから離れました。判定をリセットします。");
    }


    // ========================================
    // Goal Sequence
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
        // ③ Editor非表示
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
        // ④ Player Prefab確認
        // ========================================

        if (playerPrefab == null)
        {
            Debug.LogError(
                "GoalEnter : Player Prefabが設定されていません。"
            );

            // Editorを復帰
            RestoreEditor();

            yield break;
        }


        // ========================================
        // ⑤ Player生成位置確認
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

                // Editorを復帰
                RestoreEditor();

                yield break;
            }


            spawnPosition =
                playerSpawnPoint.position;
        }


        // ========================================
        // ⑥ Player生成
        //
        // ここでは既存Playerを勝手に探して
        // Destroyしない
        // ========================================

        spawnedPlayer = Instantiate(
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
        // ⑦ Player自動移動取得
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
        // ⑧ Animator初期化
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
        // ⑨ Player Camera取得
        // ========================================

        playerCamera =
            spawnedPlayer.GetComponentInChildren<Camera>(true);


        if (playerCamera == null)
        {
            Debug.LogError(
                "GoalEnter : Player Prefab内にCameraがありません。"
            );

            Destroy(spawnedPlayer);

            RestoreEditor();

            yield break;
        }


        // ========================================
        // ⑩ Player Camera Follow取得
        // ========================================

        playerCameraFollow =
            playerCamera.GetComponent<PlayerCameraFollow>();


        if (playerCameraFollow == null)
        {
            Debug.LogError(
                "GoalEnter : Player CameraにPlayerCameraFollowがありません。"
            );

            Destroy(spawnedPlayer);

            RestoreEditor();

            yield break;
        }


        // ========================================
        // ⑪ Player Camera Follow Target設定
        // ========================================

        playerCameraFollow.target =
            spawnedPlayer.transform;


        playerCameraFollow.enabled = true;


        Debug.Log(
            "GoalEnter : PlayerCameraFollowのTargetをPlayerに設定しました。"
        );


        // ========================================
        // ⑫ Player Camera ON
        // ========================================

        playerCamera.gameObject.SetActive(true);

        playerCamera.enabled = true;


        Debug.Log(
            "GoalEnter : Player Camera ON"
        );


        // ========================================
        // ⑬ Main Camera OFF
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
        // ⑭ カメラ切り替えを確定
        // ========================================

        yield return null;


        // ========================================
        // ⑮ 少し待機
        // ========================================

        yield return new WaitForSeconds(
            0.3f
        );


        // ========================================
        // ⑯ 明転
        // ========================================

        yield return StartCoroutine(
            FadeTo(0f)
        );


        // ========================================
        // ⑰ Fade Cube非表示
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
        // ⑱ Player開始前待機
        // ========================================

        yield return new WaitForSeconds(
            1f
        );


        // ========================================
        // ⑲ Player開始
        // ========================================

        if (playerMoveScript != null)
        {
            playerMoveScript.enabled = true;


            Debug.Log(
                "GoalEnter : Playerの自動移動を開始しました。"
            );
        }


        // ========================================
        // ⑳ Goal処理完了
        // ========================================

        Debug.Log(
            "GoalEnter : Goal処理完了。Playerステージへ移行しました。"
        );
    }


    // ========================================
    // Editor復帰
    // ========================================

    private void RestoreEditor()
    {
        // MainCamera ON
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


        color.a = alpha;


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