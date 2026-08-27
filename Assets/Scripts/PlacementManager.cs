using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [Header("木杭")]
    [SerializeField] private GameObject woodenStakePrefab;

    [Header("木杭プレビュー")]
    [SerializeField] private GameObject woodenStakePreviewPrefab;

    [Header("カメラ")]
    [SerializeField] private Camera mainCamera;

    [Header("設置可能なサーフェス")]
    [SerializeField] private LayerMask placeableLayers;

    [Header("プレビュー設定")]
    [SerializeField] private float previewAlpha = 0.45f;

    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 90f;

    [Header("杭の刺し込み設定")]
    [SerializeField] private float stakeDepth = 0.5f;
    [SerializeField] private float stakeLength = 2f; // 杭の長さ

    private bool placingWoodenStake = false;
    private bool isRotating = false;
    private float currentRotation = 0f;
    private Vector3 lastMousePosition;

    private GameObject previewObject;

    private void Update()
    {
        if (!placingWoodenStake)
        {
            return;
        }

        // =========================================
        // Escで配置モード終了
        // =========================================
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
            return;
        }

        // =========================================
        // 右クリックでも配置モード終了
        // =========================================
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
            return;
        }

        // =========================================
        // Rキーで回転モード開始/終了
        // =========================================
        if (Input.GetKeyDown(KeyCode.R))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            isRotating = false;
        }

        // =========================================
        // 回転中にマウス移動で角度調整
        // =========================================
        if (isRotating)
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera != null)
            {
                // 地面の場合のみ回転を有効にする
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000f, placeableLayers))
                {
                    Vector3 normal = hit.normal;
                    bool isWall = Mathf.Abs(normal.y) < 0.5f;

                    // 地面の場合のみ回転を許可
                    if (!isWall)
                    {
                        Vector3 currentMousePosition = Input.mousePosition;
                        float deltaX = currentMousePosition.x - lastMousePosition.x;

                        currentRotation += deltaX * rotationSpeed * Time.deltaTime;
                        lastMousePosition = currentMousePosition;
                    }
                }
            }
        }

        // =========================================
        // プレビューをマウス位置へ移動
        // =========================================
        UpdatePreview();

        // =========================================
        // 左クリックで設置
        // =========================================
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceWoodenStake();
        }
    }

    // =========================================
    // 木杭配置モード開始
    // =========================================
    public void StartWoodenStakePlacement()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        InventoryManager inventory =
            GameManager.Instance.GetInventoryManager();

        if (inventory == null)
        {
            return;
        }

        // 木杭を持っていなければ開始しない
        if (inventory.GetWoodenStake() <= 0)
        {
            return;
        }

        placingWoodenStake = true;

        // InventoryPanelを閉じる
        InventoryPanel inventoryPanel =
            FindAnyObjectByType<InventoryPanel>();

        if (inventoryPanel != null)
        {
            inventoryPanel.ClosePanel();
        }

        // プレビュー生成
        CreatePreview();
    }

    // =========================================
    // プレビュー生成
    // =========================================
    private void CreatePreview()
    {
        if (woodenStakePreviewPrefab == null)
        {
            return;
        }

        // 既にある場合は作らない
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = Instantiate(
            woodenStakePreviewPrefab
        );

        // プレビューは物理演算させない
        Rigidbody rb =
            previewObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Colliderを無効化
        Collider[] colliders =
            previewObject.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        // 半透明化
        Renderer[] renderers =
            previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;

            if (material.HasProperty("_BaseColor"))
            {
                Color color = material.GetColor("_BaseColor");

                color.a = previewAlpha;

                material.SetColor(
                    "_BaseColor",
                    color
                );
            }
        }
    }

    // =========================================
    // プレビューをマウスに追従
    // =========================================
    private void UpdatePreview()
    {
        if (previewObject == null)
        {
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }

        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            1000f,
            placeableLayers
        ))
        {
            // 壁か地面かを判定
            Vector3 normal = hit.normal;
            bool isWall = Mathf.Abs(normal.y) < 0.7f; // 判定を緩和して崖の横面も壁として扱う

            Quaternion baseRotation;

            if (isWall)
            {
                // 壁の場合：法線方向に杭を向ける
                baseRotation = Quaternion.LookRotation(normal);

                // 杭を壁にめり込ませる（刺した感じを出す）
                previewObject.transform.position = hit.point - normal * stakeDepth;
                previewObject.transform.rotation = baseRotation;
            }
            else
            {
                // 地面の場合：上向きに配置してY軸周りに回転
                baseRotation = Quaternion.FromToRotation(Vector3.up, normal);
                Quaternion rotationOffset = Quaternion.Euler(0, currentRotation, 0);
                previewObject.transform.position = hit.point;
                previewObject.transform.rotation = baseRotation * rotationOffset;
            }
        }
    }

    // =========================================
    // 木杭を実際に設置
    // =========================================
    private void TryPlaceWoodenStake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }

        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );

        RaycastHit hit;

        // サーフェスに当たっていなければ設置しない
        if (!Physics.Raycast(
            ray,
            out hit,
            1000f,
            placeableLayers
        ))
        {
            return;
        }

        if (woodenStakePrefab == null)
        {
            return;
        }

        // =========================================
        // 本物の木杭を生成
        // =========================================
        GameObject newStake = Instantiate(
            woodenStakePrefab,
            hit.point,
            Quaternion.identity
        );

        // プレビューと同じ回転と位置を適用
        Vector3 normal = hit.normal;
        bool isWall = Mathf.Abs(normal.y) < 0.7f; // 判定を緩和して崖の横面も壁として扱う

        Quaternion baseRotation;

        if (isWall)
        {
            // 壁の場合：法線方向に杭を向ける
            baseRotation = Quaternion.LookRotation(normal);

            // 杭を壁にめり込ませる（刺した感じを出す）
            newStake.transform.position = hit.point - normal * stakeDepth;
            newStake.transform.rotation = baseRotation;
        }
        else
        {
            // 地面の場合：上向きに配置してY軸周りに回転
            baseRotation = Quaternion.FromToRotation(Vector3.up, normal);
            Quaternion rotationOffset = Quaternion.Euler(0, currentRotation, 0);
            newStake.transform.position = hit.point;
            newStake.transform.rotation = baseRotation * rotationOffset;
        }

        // 杭の先端が地面にめり込んでいるかチェック
        bool isEmbedded = CheckIfStakeEmbedded(newStake, normal, isWall);

        if (isEmbedded)
        {
            // めり込んでいる場合は物理的に固定
            Rigidbody rb = newStake.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            // めり込んでいない場合は物理演算を有効にして落下させる
            Rigidbody rb = newStake.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        // Colliderを有効化
        Collider[] colliders = newStake.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }

        // =========================================
        // インベントリから1本消費
        // =========================================
        InventoryManager inventory =
            GameManager.Instance.GetInventoryManager();

        if (inventory == null)
        {
            return;
        }

        inventory.UseWoodenStake(1);

        // =========================================
        // もう残っていないなら終了
        // =========================================
        if (inventory.GetWoodenStake() <= 0)
        {
            CancelPlacement();
        }
    }

    // =========================================
    // 配置モード終了
    // =========================================
    public void CancelPlacement()
    {
        placingWoodenStake = false;

        // プレビュー削除
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    // =========================================
    // 杭が地面に触れているかチェック
    // =========================================
    private bool CheckIfStakeEmbedded(GameObject stake, Vector3 surfaceNormal, bool isWall)
    {
        if (isWall)
        {
            // 壁の場合：杭の方向にレイキャストして壁に触れているかチェック
            Vector3 stakeDirection = stake.transform.forward;
            Ray ray = new Ray(stake.transform.position, stakeDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, stakeDepth * 2, placeableLayers))
            {
                return true;
            }
        }
        else
        {
            // 地面の場合：杭の位置から下方にレイキャストして地面をチェック
            Vector3 downward = Vector3.down;
            Ray ray = new Ray(stake.transform.position, downward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 0.1f, placeableLayers))
            {
                return true;
            }
        }

        return false;
    }
}