using System.Collections;
using UnityEngine;

public class GoalEnter : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;


    [Header("Editor")]
    public GameObject editor;
    public MonoBehaviour editorMoveScript;


    [Header("Camera")]
    public Camera editorCamera;


    [Header("Fade Cube")]
    public GameObject fadeCube;

    [Tooltip("暗転・明転にかかる時間")]
    public float fadeDuration = 1.0f;


    private bool triggered = false;

    // Goalに入ったEditor
    private GameObject editorObject;

    // Fade CubeのRenderer
    private Renderer fadeRenderer;

    // マテリアル
    private Material fadeMaterial;


    private void Start()
    {
        // Fade CubeのRendererを取得
        if (fadeCube != null)
        {
            fadeRenderer = fadeCube.GetComponent<Renderer>();

            if (fadeRenderer != null)
            {
                // 他のオブジェクトのマテリアルまで
                // 変えてしまわないようにInstance化
                fadeMaterial = fadeRenderer.material;

                // 最初は透明
                SetFadeAlpha(0f);
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


    private void OnTriggerEnter(Collider other)
    {
        // すでに発動していたら何もしない
        if (triggered)
            return;

        // Editor以外は反応しない
        if (editor != null && other.gameObject != editor)
            return;

        triggered = true;

        // 入ってきたEditorを保存
        editorObject = other.gameObject;

        // Goal演出開始
        StartCoroutine(GoalSequence());
    }


    private IEnumerator GoalSequence()
    {
        // ========================================
        // ① Editorの操作を停止
        // ========================================

        if (editorMoveScript != null)
        {
            editorMoveScript.enabled = false;
        }


        // ========================================
        // ② 画面を暗転
        // ========================================

        yield return StartCoroutine(FadeTo(1f));


        // ========================================
        // ③ Editorを非表示
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
        // ④ Playerの設定確認
        // ========================================

        if (playerPrefab == null)
        {
            Debug.LogError(
                "GoalEnter : Player Prefabが設定されていません。"
            );

            yield break;
        }

        if (playerSpawnPoint == null)
        {
            Debug.LogError(
                "GoalEnter : Player Spawn Pointが設定されていません。"
            );

            yield break;
        }


        // ========================================
        // ⑤ Player召喚
        // ========================================

        GameObject player = Instantiate(
            playerPrefab,
            playerSpawnPoint.position,
            playerSpawnPoint.rotation
        );

        Debug.Log(
            "GoalEnter : Playerを召喚しました。"
        );


        // ========================================
        // ⑥ Editor CameraをOFF
        // ========================================

        if (editorCamera != null)
        {
            editorCamera.gameObject.SetActive(false);
        }


        // ========================================
        // ⑦ Player CameraをON
        // ========================================

        Camera playerCamera =
            player.GetComponentInChildren<Camera>();

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "GoalEnter : Player Prefabの中にCameraがありません。"
            );
        }


        // ========================================
        // ⑧ 少し待つ
        // ========================================

        yield return new WaitForSeconds(0.3f);


        // ========================================
        // ⑨ 明転
        // ========================================

        yield return StartCoroutine(FadeTo(0f));


        Debug.Log(
            "GoalEnter : Playerのシーンへ切り替わりました。"
        );
    }


    // ========================================
    // Fade処理
    // ========================================

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeMaterial == null)
            yield break;

        float startAlpha = GetFadeAlpha();

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float t =
                Mathf.Clamp01(time / fadeDuration);

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


    // ========================================
    // Alphaを設定
    // ========================================

    private void SetFadeAlpha(float alpha)
    {
        if (fadeMaterial == null)
            return;

        Color color = fadeMaterial.color;

        color.a = alpha;

        fadeMaterial.color = color;
    }


    // ========================================
    // 現在のAlphaを取得
    // ========================================

    private float GetFadeAlpha()
    {
        if (fadeMaterial == null)
            return 0f;

        return fadeMaterial.color.a;
    }
}