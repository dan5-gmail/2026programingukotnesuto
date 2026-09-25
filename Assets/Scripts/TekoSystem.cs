using System.Collections;
using UnityEngine;

public class TekoSystem : MonoBehaviour
{
    // =========================================================
    // てこの板
    // =========================================================

    [Header("てこの板")]

    [Tooltip("実際に回転するてこの板")]
    [SerializeField] private Transform てこの板;

    [Tooltip("Editorが乗っているか判定するための板のCollider")]
    [SerializeField] private Collider てこの板のCollider;


    // =========================================================
    // Editor
    // =========================================================

    [Header("Editor")]

    [Tooltip("このてこに乗るEditor")]
    [SerializeField] private GameObject Editor;

    [Tooltip("Editorとてこの板の距離がこの値以下ならEditorが乗っていると判定")]
    [SerializeField] private float Editor判定距離 = 10f;


    // =========================================================
    // 初期の重り
    // =========================================================

    [Header("初期の重り")]

    [Tooltip("最初から板の上に置いてある重り")]
    [SerializeField] private GameObject 初期の重り;


    // =========================================================
    // 角度
    // =========================================================

    [Header("てこの角度")]

    [Tooltip("何も乗っていない初期状態の角度")]
    [SerializeField] private float 初期の角度 = 15f;

    [Tooltip("Editorが板の上に乗ったときの角度")]
    [SerializeField] private float Editorが乗った時の角度 = 5f;

    [Tooltip("重い石を置いた時の完全に倒れる角度")]
    [SerializeField] private float 重い石を置いた時の角度 = -25f;

    [Tooltip("板が指定角度まで傾く時間")]
    [SerializeField] private float 傾く時間 = 0.5f;

    [Tooltip("板が回転する軸")]
    [SerializeField] private Vector3 回転軸 = Vector3.forward;


    // =========================================================
    // 遺跡
    // =========================================================

    [Header("遺跡")]

    [Tooltip("地下から出てくる遺跡")]
    [SerializeField] private Transform 遺跡;

    [Tooltip("遺跡が上昇する距離")]
    [SerializeField] private float 遺跡の上昇距離 = 3f;

    [Tooltip("遺跡が出てくる時間")]
    [SerializeField] private float 遺跡の上昇時間 = 2f;


    // =========================================================
    // 画面揺れ
    // =========================================================

    [Header("画面揺れ")]

    [Tooltip("揺らすカメラ")]
    [SerializeField] private Transform 揺らすカメラ;

    [Tooltip("画面揺れの強さ")]
    [SerializeField] private float 画面揺れの強さ = 0.12f;

    [Tooltip("画面揺れの時間")]
    [SerializeField] private float 画面揺れの時間 = 0.8f;


    // =========================================================
    // 内部
    // =========================================================

    private Quaternion 基準回転;

    private Vector3 遺跡の初期位置;

    private bool Editorが乗った = false;

    private bool 発動済み = false;

    private bool 角度変更中 = false;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        // -----------------------------------------------------
        // てこの板
        // -----------------------------------------------------

        if (てこの板 == null)
        {
            Debug.LogError(
                "TekoSystem : 「てこの板」が設定されていません。"
            );

            enabled = false;
            return;
        }


        // -----------------------------------------------------
        // 板のCollider
        // -----------------------------------------------------

        if (てこの板のCollider == null)
        {
            てこの板のCollider =
                てこの板.GetComponent<Collider>();
        }

        if (てこの板のCollider == null)
        {
            Debug.LogError(
                "TekoSystem : 「てこの板」にColliderがありません。"
            );

            enabled = false;
            return;
        }


        // -----------------------------------------------------
        // 基準回転
        // -----------------------------------------------------

        基準回転 = てこの板.rotation;


        // -----------------------------------------------------
        // 遺跡の初期位置
        // -----------------------------------------------------

        if (遺跡 != null)
        {
            遺跡の初期位置 =
                遺跡.position;
        }


        // -----------------------------------------------------
        // カメラ
        // -----------------------------------------------------

        if (揺らすカメラ == null)
        {
            if (Camera.main != null)
            {
                揺らすカメラ =
                    Camera.main.transform;
            }
        }


        // -----------------------------------------------------
        // 初期角度
        // -----------------------------------------------------

        てこの板.rotation =
            基準回転 *
            Quaternion.AngleAxis(
                初期の角度,
                回転軸.normalized
            );
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        // 発動済みなら何もしない
        if (発動済み)
        {
            return;
        }

        // 角度変更中は判定しない
        if (角度変更中)
        {
            return;
        }


        // =====================================================
        // Editor判定
        // =====================================================

        bool 現在Editorが乗っている =
            Editorが板の上にいる();


        // =====================================================
        // Editorが乗った
        // =====================================================

        if (現在Editorが乗っている)
        {
            if (!Editorが乗った)
            {
                Editorが乗った = true;

                StartCoroutine(
                    Editorが乗った時の処理()
                );
            }

            return;
        }


        // =====================================================
        // Editorが降りた
        // =====================================================

        if (Editorが乗った)
        {
            Editorが乗った = false;

            StartCoroutine(
                角度を変更する(
                    初期の角度
                )
            );
        }
    }


    // =========================================================
    // Editorが板の上にいるか
    // =========================================================

    private bool Editorが板の上にいる()
    {
        if (Editor == null)
        {
            return false;
        }

        if (てこの板 == null)
        {
            return false;
        }


        float 距離 =
            Vector3.Distance(
                Editor.transform.position,
                てこの板.position
            );


        return 距離 <= Editor判定距離;
    }


    // =========================================================
    // Editorが乗った
    // =========================================================

    private IEnumerator Editorが乗った時の処理()
    {
        yield return StartCoroutine(
            角度を変更する(
                Editorが乗った時の角度
            )
        );
    }


    // =========================================================
    // 重い石が配置された時
    // =========================================================
    // PlacementManagerから直接呼ばれる
    // =========================================================

    public void 重い石を受け取った(GameObject 置かれた重い石)
    {
        // すでに発動済みなら無視
        if (発動済み)
        {
            return;
        }


        if (置かれた重い石 == null)
        {
            Debug.LogWarning(
                "TekoSystem : 置かれた重い石がnullです。"
            );

            return;
        }


        Debug.Log(
            "TekoSystem : 重い石の配置を確認しました。"
        );


        Debug.Log(
            "TekoSystem : 発動条件を満たしました。てこを傾けます。"
        );


        StartCoroutine(
            重い石を置いた時の処理()
        );
    }


    // =========================================================
    // 重い石を置いた時の処理
    // =========================================================

    private IEnumerator 重い石を置いた時の処理()
    {
        // 二重発動防止
        発動済み = true;

        //少し待つ
        yield return new WaitForSeconds(1.2f);


        // =====================================================
        // ① てこを完全に倒す
        // =====================================================

        yield return StartCoroutine(
            角度を変更する(
                重い石を置いた時の角度
            )
        );


        // =====================================================
        // ② 画面揺れ
        // =====================================================

        yield return StartCoroutine(
            画面を揺らす()
        );


        // =====================================================
        // ③ 少し待つ
        // =====================================================

        yield return new WaitForSeconds(
            0.25f
        );


        // =====================================================
        // ④ 遺跡出現
        // =====================================================

        yield return StartCoroutine(
            遺跡を出す()
        );
    }


    // =========================================================
    // 指定角度へ回転
    // =========================================================

    private IEnumerator 角度を変更する(
        float 目標角度
    )
    {
        if (てこの板 == null)
        {
            yield break;
        }


        角度変更中 = true;


        Quaternion 開始回転 =
            てこの板.rotation;


        Quaternion 終了回転 =
            基準回転 *
            Quaternion.AngleAxis(
                目標角度,
                回転軸.normalized
            );


        float 経過時間 = 0f;


        while (経過時間 < 傾く時間)
        {
            経過時間 +=
                Time.deltaTime;


            float 割合 =
                Mathf.Clamp01(
                    経過時間 /
                    傾く時間
                );


            割合 =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    割合
                );


            てこの板.rotation =
                Quaternion.Slerp(
                    開始回転,
                    終了回転,
                    割合
                );


            yield return null;
        }


        てこの板.rotation =
            終了回転;


        角度変更中 = false;
    }


    // =========================================================
    // 遺跡出現
    // =========================================================

    private IEnumerator 遺跡を出す()
    {
        if (遺跡 == null)
        {
            Debug.LogWarning(
                "TekoSystem : 「遺跡」が設定されていません。"
            );

            yield break;
        }


        Vector3 開始位置 =
            遺跡の初期位置;


        Vector3 終了位置 =
            開始位置 +
            Vector3.up *
            遺跡の上昇距離;


        float 経過時間 = 0f;


        while (経過時間 < 遺跡の上昇時間)
        {
            経過時間 +=
                Time.deltaTime;


            float 割合 =
                Mathf.Clamp01(
                    経過時間 /
                    遺跡の上昇時間
                );


            割合 =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    割合
                );


            遺跡.position =
                Vector3.Lerp(
                    開始位置,
                    終了位置,
                    割合
                );


            yield return null;
        }


        遺跡.position =
            終了位置;
    }


    // =========================================================
    // 画面揺れ
    // =========================================================

    private IEnumerator 画面を揺らす()
    {
        if (揺らすカメラ == null)
        {
            yield break;
        }


        Vector3 元の位置 =
            揺らすカメラ.localPosition;


        float 経過時間 = 0f;


        while (経過時間 < 画面揺れの時間)
        {
            経過時間 +=
                Time.deltaTime;


            float 割合 =
                Mathf.Clamp01(
                    経過時間 /
                    画面揺れの時間
                );


            float 現在の強さ =
                Mathf.Lerp(
                    画面揺れの強さ,
                    0f,
                    割合
                );


            Vector3 揺れ =
                Random.insideUnitSphere *
                現在の強さ;


            揺らすカメラ.localPosition =
                元の位置 +
                揺れ;


            yield return null;
        }


        揺らすカメラ.localPosition =
            元の位置;
    }


    // =========================================================
    // 強制発動
    // =========================================================

    public void 強制発動()
    {
        if (発動済み)
        {
            return;
        }


        StartCoroutine(
            重い石を置いた時の処理()
        );
    }


    // =========================================================
    // 発動済み確認
    // =========================================================

    public bool 発動済みか()
    {
        return 発動済み;
    }


    // =========================================================
    // Gizmo
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (てこの板 == null)
        {
            return;
        }


        // Editor判定距離を表示
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(
            てこの板.position,
            Editor判定距離
        );
    }
}