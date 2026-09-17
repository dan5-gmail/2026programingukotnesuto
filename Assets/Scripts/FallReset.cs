using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallReset : MonoBehaviour
{
    public enum CharacterType
    {
        Editor,
        Player
    }

    [Header("落下判定")]
    [SerializeField]
    private float fallHeight = -10f;

    [Header("初期地点")]
    [SerializeField]
    private Transform initialPoint;

    [Header("キャラクター")]
    [SerializeField]
    private CharacterType characterType = CharacterType.Editor;

    private Rigidbody rb;

    private bool isResetting = false;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        isResetting = false;

        // Initial Pointが設定されていない場合、
        // ゲーム開始時の位置を初期地点として記録
        if (initialPoint == null)
        {
            GameObject point = new GameObject(
                gameObject.name + "_InitialPoint"
            );

            point.transform.position = transform.position;
            point.transform.rotation = transform.rotation;

            initialPoint = point.transform;
        }
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (isResetting)
            return;

        if (transform.position.y <= fallHeight)
        {
            Fall();
        }
    }


    // =========================================================
    // 落下処理
    // =========================================================

    private void Fall()
    {
        if (isResetting)
            return;

        isResetting = true;

        if (characterType == CharacterType.Editor)
        {
            EditorFell();
        }
        else
        {
            PlayerFell();
        }
    }


    // =========================================================
    // Editorが落ちた
    // =========================================================

    private void EditorFell()
    {
        Debug.Log(
            "FallReset : Editor has fallen."
        );

        // Editor自身を初期地点へ
        ResetThisCharacter();

        isResetting = false;
    }


    // =========================================================
    // Playerが落ちた
    // =========================================================

    private void PlayerFell()
    {
        Debug.Log(
            "FallReset : Player could not reach the goal."
        );


        // =====================================================
        // EditorLog
        // =====================================================

        if (EditorLogManager.Instance != null)
        {
            EditorLogManager.Instance.AddErrorLog(
                "Player could not reach the goal."
            );
        }
        else
        {
            Debug.LogWarning(
                "FallReset : EditorLogManager.Instance が見つかりません。"
            );
        }


        // =====================================================
        // Playerを初期地点へ
        // =====================================================

        ResetThisCharacter();


        // =====================================================
        // PlayerAutoMoveを再開
        // =====================================================

        PPlayerAutoMove playerMove =
            GetComponent<PPlayerAutoMove>();

        if (playerMove != null)
        {
            playerMove.ResumePlayer();
        }


        // =====================================================
        // Editorを初期地点へ
        // =====================================================

        ResetEditor();


        isResetting = false;
    }


    // =========================================================
    // Editorを初期地点へ戻す
    // =========================================================

    private void ResetEditor()
    {
        GameObject editorObject =
            GameObject.FindGameObjectWithTag("Editor");

        if (editorObject == null)
        {
            Debug.LogWarning(
                "FallReset : Tag = Editor のEditorが見つかりません。"
            );

            return;
        }


        FallReset editorFallReset =
            editorObject.GetComponent<FallReset>();


        if (editorFallReset != null)
        {
            editorFallReset.ResetFromExternal();
            return;
        }


        // -----------------------------------------------------
        // FallResetが無い場合の予備処理
        // -----------------------------------------------------

        Rigidbody editorRb =
            editorObject.GetComponent<Rigidbody>();

        Transform editorInitialPoint =
            FindInitialPoint("EditorInitialPoint");


        if (editorInitialPoint != null)
        {
            editorObject.transform.position =
                editorInitialPoint.position;

            editorObject.transform.rotation =
                editorInitialPoint.rotation;
        }


        if (editorRb != null)
        {
            editorRb.linearVelocity = Vector3.zero;
            editorRb.angularVelocity = Vector3.zero;
        }
    }


    // =========================================================
    // 外部から初期地点へ戻す
    // =========================================================

    public void ResetFromExternal()
    {
        ResetThisCharacter();

        isResetting = false;
    }


    // =========================================================
    // 自分自身を初期地点へ戻す
    // =========================================================

    private void ResetThisCharacter()
    {
        if (initialPoint == null)
        {
            Debug.LogWarning(
                "FallReset : " +
                gameObject.name +
                " のInitial Pointが設定されていません。"
            );

            return;
        }


        // Rigidbodyの速度をリセット
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;


        // 初期地点へ移動
        transform.position =
            initialPoint.position;

        transform.rotation =
            initialPoint.rotation;


        // 移動後も速度を確実に0
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


    // =========================================================
    // Initial Point検索
    // =========================================================

    private Transform FindInitialPoint(string pointName)
    {
        GameObject point =
            GameObject.Find(pointName);

        if (point == null)
            return null;

        return point.transform;
    }


    // =========================================================
    // Sceneビュー用
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 center = new Vector3(
            transform.position.x,
            fallHeight,
            transform.position.z
        );

        Gizmos.DrawLine(
            center + Vector3.left * 5f,
            center + Vector3.right * 5f
        );
    }
}