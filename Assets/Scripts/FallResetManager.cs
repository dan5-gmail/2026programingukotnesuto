using UnityEngine;

public class FallResetManager : MonoBehaviour
{
    [Header("落下判定")]
    [SerializeField]
    private float fallHeight = -10f;

    [Header("Editor")]
    [SerializeField]
    private GameObject editor;

    [SerializeField]
    private Transform editorInitialPoint;

    [SerializeField]
    private bool useDirectEditorPosition = false;

    [SerializeField]
    private Vector3 editorInitialPosition;

    [Header("Player")]
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Transform playerInitialPoint;

    private Rigidbody editorRb;
    private Rigidbody playerRb;

    private PPlayerAutoMove playerAutoMove;

    private bool resetting = false;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        // Editor取得
        if (editor != null)
        {
            editorRb = editor.GetComponent<Rigidbody>();
        }

        // Player取得
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
            playerAutoMove = player.GetComponent<PPlayerAutoMove>();
        }
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        // 念のためStart時にも取得
        if (editor != null && editorRb == null)
        {
            editorRb = editor.GetComponent<Rigidbody>();
        }

        if (player != null)
        {
            if (playerRb == null)
            {
                playerRb = player.GetComponent<Rigidbody>();
            }

            if (playerAutoMove == null)
            {
                playerAutoMove =
                    player.GetComponent<PPlayerAutoMove>();
            }
        }
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (resetting)
            return;


        // =====================================================
        // Editor落下判定
        // =====================================================

        if (editor != null)
        {
            if (editor.transform.position.y <= fallHeight)
            {
                Debug.Log($"FallResetManager : Editor fell at Y={editor.transform.position.y}");
                EditorFell();
                return;
            }
        }


        // =====================================================
        // Player落下判定
        // =====================================================

        if (player != null)
        {
            if (player.transform.position.y <= fallHeight)
            {
                Debug.Log($"FallResetManager : Player fell at Y={player.transform.position.y}");
                PlayerFell();
                return;
            }
        }
    }


    // =========================================================
    // Editor落下
    // =========================================================

    private void EditorFell()
    {
        resetting = true;

        Debug.Log(
            "FallResetManager : Editor has fallen."
        );

        ResetEditor();

        resetting = false;
    }


    // =========================================================
    // Player落下
    // =========================================================

    private void PlayerFell()
    {
        resetting = true;

        Debug.Log(
            "FallResetManager : Player could not reach the goal."
        );


        // =====================================================
        // EditorLog
        // =====================================================

        if (EditorLogManager.Instance != null)
        {
            EditorLogManager.Instance.AddErrorLog(
                "! Player fell into the abyss."
            );
        }
        else
        {
            Debug.LogWarning(
                "FallResetManager : EditorLogManagerが見つかりません。"
            );
        }


        // =====================================================
        // PlayerAutoMoveを一旦停止
        // =====================================================

        if (playerAutoMove != null)
        {
            playerAutoMove.StopPlayer();
        }


        // =====================================================
        // Playerを初期地点へ
        // =====================================================

        ResetPlayer();


        // =====================================================
        // Editorを初期地点へ
        // =====================================================

        ResetEditor();


        // =====================================================
        // Playerを再開
        // =====================================================

        if (playerAutoMove != null)
        {
            playerAutoMove.ResumePlayer();
        }


        resetting = false;
    }


    // =========================================================
    // Editorを初期地点へ
    // =========================================================

    private void ResetEditor()
    {
        if (editor == null)
        {
            Debug.LogWarning(
                "FallResetManager : Editorが設定されていません。"
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
                    "FallResetManager : EditorInitialPointが設定されていません。"
                );

                return;
            }

            resetPosition = editorInitialPoint.position;
            resetRotation = editorInitialPoint.rotation;
        }


        // Rigidbodyがある場合
        if (editorRb != null)
        {
            editorRb.linearVelocity = Vector3.zero;
            editorRb.angularVelocity = Vector3.zero;

            // Rigidbodyを直接移動
            editorRb.position = resetPosition;
            editorRb.rotation = resetRotation;

            // もう一度速度を0
            editorRb.linearVelocity = Vector3.zero;
            editorRb.angularVelocity = Vector3.zero;
        }
        else
        {
            // Rigidbodyがない場合の予備
            editor.transform.position = resetPosition;
            editor.transform.rotation = resetRotation;
        }


        Physics.SyncTransforms();


        Debug.Log(
            "FallResetManager : Editor reset."
        );
    }


    // =========================================================
    // Playerを初期地点へ
    // =========================================================

    private void ResetPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning(
                "FallResetManager : Playerが設定されていません。"
            );

            return;
        }

        if (playerInitialPoint == null)
        {
            Debug.LogWarning(
                "FallResetManager : PlayerInitialPointが設定されていません。"
            );

            return;
        }


        if (playerRb != null)
        {
            // まず物理速度を完全停止
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;


            // ★重要★
            // TransformではなくRigidbodyを直接移動
            playerRb.position =
                playerInitialPoint.position;

            playerRb.rotation =
                playerInitialPoint.rotation;


            // 移動後の速度も完全停止
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }
        else
        {
            player.transform.position =
                playerInitialPoint.position;

            player.transform.rotation =
                playerInitialPoint.rotation;
        }


        // 物理演算へ位置変更を反映
        Physics.SyncTransforms();


        Debug.Log(
            "FallResetManager : Player reset."
        );
    }
}