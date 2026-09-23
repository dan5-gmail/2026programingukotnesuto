using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    // =========================================
    // 木杭
    // =========================================

    [Header("木杭")]
    [SerializeField] private GameObject woodenStakePrefab;


    // =========================================
    // 木杭プレビュー
    // =========================================

    [Header("木杭プレビュー")]
    [SerializeField] private GameObject woodenStakePreviewPrefab;


    // =========================================
    // 木の橋
    // =========================================

    [Header("木の橋")]
    [SerializeField] private GameObject woodBridgePrefab;


    // =========================================
    // 木の橋プレビュー
    // =========================================

    [Header("木の橋プレビュー")]
    [SerializeField] private GameObject woodBridgePreviewPrefab;


    // =========================================
    // 栄養アイテム
    // =========================================

    [Header("栄養アイテム")]
    [SerializeField] private GameObject nutritionItemPrefab;


    // =========================================
    // 栄養アイテムプレビュー
    // =========================================

    [Header("栄養アイテムプレビュー")]
    [SerializeField] private GameObject nutritionItemPreviewPrefab;


    // =========================================
    // 睡蓮
    // =========================================

    [Header("睡蓮")]
    [SerializeField] private GameObject lilyPadPrefab;


    // =========================================
    // 睡蓮プレビュー
    // =========================================

    [Header("睡蓮プレビュー")]
    [SerializeField] private GameObject lilyPadPreviewPrefab;


    // =========================================
    // 重い石
    // =========================================

    [Header("重い石")]
    [SerializeField] private GameObject heavyStonePrefab;


    // =========================================
    // 重い石プレビュー
    // =========================================

    [Header("重い石プレビュー")]
    [SerializeField] private GameObject heavyStonePreviewPrefab;


    // =========================================
    // カメラ
    // =========================================

    [Header("カメラ")]
    [SerializeField] private Camera mainCamera;


    // =========================================
    // 設置可能なサーフェス
    // =========================================

    [Header("設置可能なサーフェス")]
    [SerializeField] private LayerMask placeableLayers;


    // =========================================
    // 重い石専用の設置可能Layer
    // =========================================

    [Header("重い石の設置可能Layer")]
    [SerializeField] private LayerMask heavyStonePlaceableLayers;


    // =========================================
    // プレビュー設定
    // =========================================

    [Header("プレビュー設定")]
    [SerializeField] private float previewAlpha = 0.45f;


    // =========================================
    // 回転設定
    // =========================================

    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 90f;


    // =========================================
    // 杭の刺し込み設定
    // =========================================

    [Header("杭の刺し込み設定")]
    [SerializeField] private float stakeDepth = 0.5f;

    [SerializeField] private float stakeLength = 2f;

    [SerializeField] private float wallAngle = 45f;

    [SerializeField] private float maxGroundSlope = 0.3f;


    // =========================================
    // 配置遅延設定
    // =========================================

    [Header("配置遅延設定")]
    [SerializeField] private float placementDelay = 0.5f;


    // =========================================
    // エリア設定
    // =========================================

    [Header("エリア設定")]
    [SerializeField] private LayerMask bridgeZoneLayer;


    // =========================================
    // 配置状態
    // =========================================

    private bool placingWoodenStake = false;
    private bool placingWoodBridge = false;
    private bool placingNutritionItem = false;
    private bool placingLilyPad = false;
    private bool placingHeavyStone = false;

    private float placementStartTime;

    private bool isRotating = false;

    private float currentRotation = 0f;

    private Vector3 lastMousePosition;


    // =========================================
    // NutritionItemPlacement
    // =========================================

    [Header("NutritionItemPlacement")]
    [SerializeField] private NutritionItemPlacement nutritionItemPlacement;


    // =========================================
    // プレビュー
    // =========================================

    private GameObject previewObject;


    // =========================================
    // Update
    // =========================================

    private void Update()
    {
        if (!placingWoodenStake &&
            !placingWoodBridge &&
            !placingNutritionItem &&
            !placingLilyPad &&
            !placingHeavyStone)
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
        // Rキーで回転モード開始
        // =========================================

        if (Input.GetKeyDown(KeyCode.R))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }


        // =========================================
        // Rキーを離したら回転終了
        // =========================================

        if (Input.GetKeyUp(KeyCode.R))
        {
            isRotating = false;
        }


        // =========================================
        // 回転中にマウス移動で角度調整
        // =========================================

        if (isRotating)
        {
            Vector3 currentMousePosition =
                Input.mousePosition;

            float deltaX =
                currentMousePosition.x -
                lastMousePosition.x;

            currentRotation +=
                deltaX *
                rotationSpeed *
                Time.deltaTime;

            lastMousePosition =
                currentMousePosition;
        }


        // =========================================
        // プレビュー更新
        // =========================================

        UpdatePreview();


        // =========================================
        // 左クリックで設置
        // =========================================

        if (Input.GetMouseButtonDown(0))
        {
            if (CanPlace())
            {
                if (placingWoodenStake)
                {
                    TryPlaceWoodenStake();
                }
                else if (placingWoodBridge)
                {
                    TryPlaceWoodBridge();
                }
                else if (placingNutritionItem)
                {
                    TryPlaceNutritionItem();
                }
                else if (placingLilyPad)
                {
                    TryPlaceLilyPad();
                }
                else if (placingHeavyStone)
                {
                    TryPlaceHeavyStone();
                }
            }
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


        if (inventory.GetWoodenStake() <= 0)
        {
            return;
        }


        placingWoodenStake = true;

        placementStartTime = Time.time;


        InventoryPanel inventoryPanel =
            FindAnyObjectByType<InventoryPanel>();

        if (inventoryPanel != null)
        {
            inventoryPanel.ClosePanel();
        }


        CreatePreview();
    }


    // =========================================
    // 木の橋配置モード開始
    // =========================================

    public void StartWoodBridgePlacement()
    {
        if (GameManager.Instance == null)
        {
            return;
        }


        placingWoodBridge = true;

        placementStartTime = Time.time;


        InventoryPanel inventoryPanel =
            FindAnyObjectByType<InventoryPanel>();

        if (inventoryPanel != null)
        {
            inventoryPanel.ClosePanel();
        }


        CreatePreview();
    }


    // =========================================
    // 栄養アイテム配置モード開始
    // =========================================

    public void StartNutritionItemPlacement()
    {
        if (GameManager.Instance == null)
        {
            return;
        }


        placingNutritionItem = true;

        placementStartTime = Time.time;


        InventoryPanel inventoryPanel =
            FindAnyObjectByType<InventoryPanel>();

        if (inventoryPanel != null)
        {
            inventoryPanel.ClosePanel();
        }


        CreatePreview();
    }


    // =========================================
    // 睡蓮配置モード開始
    // =========================================

    public void StartLilyPadPlacement()
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


        if (inventory.GetLilyPad() <= 0)
        {
            return;
        }


        placingLilyPad = true;

        placementStartTime = Time.time;


        InventoryPanel inventoryPanel =
            FindAnyObjectByType<InventoryPanel>();

        if (inventoryPanel != null)
        {
            inventoryPanel.ClosePanel();
        }


        CreatePreview();
    }


    // =========================================
    // 重い石配置モード開始
    // =========================================

    public void StartHeavyStonePlacement()
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


        if (inventory.GetHeavyStone() <= 0)
        {
            return;
        }


        placingHeavyStone = true;

        placementStartTime = Time.time;


        InventoryPanel inventoryPanel =
            FindAnyObjectByType<InventoryPanel>();

        if (inventoryPanel != null)
        {
            inventoryPanel.ClosePanel();
        }


        CreatePreview();
    }


    // =========================================
    // プレビュー生成
    // =========================================

    private void CreatePreview()
    {
        GameObject prefabToUse = null;


        if (placingWoodenStake)
        {
            prefabToUse =
                woodenStakePreviewPrefab;
        }
        else if (placingWoodBridge)
        {
            prefabToUse =
                woodBridgePreviewPrefab;
        }
        else if (placingNutritionItem)
        {
            prefabToUse =
                nutritionItemPreviewPrefab;
        }
        else if (placingLilyPad)
        {
            prefabToUse =
                lilyPadPreviewPrefab;
        }
        else if (placingHeavyStone)
        {
            prefabToUse =
                heavyStonePreviewPrefab;
        }


        if (prefabToUse == null)
        {
            return;
        }


        if (previewObject != null)
        {
            Destroy(previewObject);
        }


        previewObject =
            Instantiate(prefabToUse);


        // =========================================
        // 物理演算を無効化
        // =========================================

        Rigidbody rb =
            previewObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }


        // =========================================
        // Colliderを無効化
        // =========================================

        Collider[] colliders =
            previewObject.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }


        // =========================================
        // 半透明化
        // =========================================

        Renderer[] renderers =
            previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material material =
                renderer.material;

            if (material.HasProperty("_BaseColor"))
            {
                Color color =
                    material.GetColor("_BaseColor");

                color.a =
                    previewAlpha;

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


        if (float.IsInfinity(Input.mousePosition.x) ||
            float.IsInfinity(Input.mousePosition.y))
        {
            return;
        }


        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );


        RaycastHit hit;


        // =========================================
        // 使用するLayer
        // =========================================

        LayerMask currentLayers =
            placeableLayers;


        // 重い石だけ専用Layer
        if (placingHeavyStone)
        {
            currentLayers =
                heavyStonePlaceableLayers;
        }
        else if (placingWoodBridge)
        {
            currentLayers |=
                bridgeZoneLayer;
        }


        // =========================================
        // Raycast
        // =========================================

        if (Physics.Raycast(
            ray,
            out hit,
            1000f,
            currentLayers))
        {
            // =====================================
            // 木の橋
            // =====================================

            if (placingWoodBridge)
            {
                previewObject.transform.position =
                    hit.point;

                previewObject.transform.rotation =
                    Quaternion.identity;
            }


            // =====================================
            // 栄養アイテム
            // =====================================

            else if (placingNutritionItem)
            {
                previewObject.transform.position =
                    hit.point +
                    Vector3.up * 0.1f;

                previewObject.transform.rotation =
                    Quaternion.identity;
            }


            // =====================================
            // 睡蓮
            // =====================================

            else if (placingLilyPad)
            {
                previewObject.transform.position =
                    hit.point +
                    Vector3.up * 0.05f;

                previewObject.transform.rotation =
                    Quaternion.identity;
            }


            // =====================================
            // 重い石
            // =====================================

            else if (placingHeavyStone)
            {
                previewObject.transform.position =
                    hit.point +
                    Vector3.up * 0.05f;

                previewObject.transform.rotation =
                    Quaternion.identity;
            }


            // =====================================
            // 木杭
            // =====================================

            else if (placingWoodenStake)
            {
                Vector3 normal =
                    hit.normal;


                bool isGround =
                    normal.y >
                    maxGroundSlope;


                bool isWall =
                    Mathf.Abs(normal.y) <
                    (1f - maxGroundSlope);


                if (!isGround && !isWall)
                {
                    return;
                }


                bool isBridgeZone =
                    IsInBridgeZone(
                        hit.point
                    );


                if (isBridgeZone &&
                    !placingWoodBridge)
                {
                    return;
                }


                Quaternion baseRotation;


                // =================================
                // 壁
                // =================================

                if (isWall)
                {
                    Vector3 wallDirection =
                        -normal;

                    Vector3 up =
                        Vector3.up;

                    Vector3 right =
                        Vector3.Cross(
                            normal,
                            up
                        ).normalized;


                    if (right == Vector3.zero)
                    {
                        right =
                            Vector3.right;
                    }


                    Vector3 diagonalDirection =
                        Quaternion.AngleAxis(
                            -wallAngle,
                            right
                        ) *
                        wallDirection;


                    baseRotation =
                        Quaternion.LookRotation(
                            diagonalDirection,
                            up
                        );


                    previewObject.transform.position =
                        hit.point;

                    previewObject.transform.rotation =
                        baseRotation;
                }


                // =================================
                // 地面
                // =================================

                else
                {
                    baseRotation =
                        Quaternion.Euler(
                            0,
                            0,
                            0
                        );


                    Quaternion rotationOffset =
                        Quaternion.Euler(
                            0,
                            currentRotation,
                            0
                        );


                    previewObject.transform.position =
                        hit.point;

                    previewObject.transform.rotation =
                        baseRotation *
                        rotationOffset;
                }
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


        if (float.IsInfinity(Input.mousePosition.x) ||
            float.IsInfinity(Input.mousePosition.y))
        {
            return;
        }


        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );


        RaycastHit hit;


        LayerMask currentLayers =
            placeableLayers;


        if (placingWoodBridge)
        {
            currentLayers |=
                bridgeZoneLayer;
        }


        if (!Physics.Raycast(
            ray,
            out hit,
            1000f,
            currentLayers))
        {
            return;
        }


        if (woodenStakePrefab == null)
        {
            return;
        }


        bool isBridgeZone =
            IsInBridgeZone(
                hit.point
            );


        if (isBridgeZone &&
            !placingWoodBridge)
        {
            return;
        }


        // =========================================
        // 木杭生成
        // =========================================

        GameObject newStake =
            Instantiate(
                woodenStakePrefab,
                hit.point,
                Quaternion.identity
            );


        // =========================================
        // 物理スクリプト
        // =========================================

        WoodStakePhysics physics =
            newStake.GetComponent<WoodStakePhysics>();


        if (physics == null)
        {
            physics =
                newStake.AddComponent<WoodStakePhysics>();
        }


        Vector3 normal =
            hit.normal;


        bool isGround =
            normal.y >
            maxGroundSlope;


        bool isWall =
            Mathf.Abs(normal.y) <
            (1f - maxGroundSlope);


        if (!isGround && !isWall)
        {
            Destroy(newStake);
            return;
        }


        Quaternion baseRotation;


        // =========================================
        // 壁
        // =========================================

        if (isWall)
        {
            Vector3 wallDirection =
                -normal;

            Vector3 up =
                Vector3.up;

            Vector3 right =
                Vector3.Cross(
                    normal,
                    up
                ).normalized;


            if (right == Vector3.zero)
            {
                right =
                    Vector3.right;
            }


            Vector3 diagonalDirection =
                Quaternion.AngleAxis(
                    -wallAngle,
                    right
                ) *
                wallDirection;


            baseRotation =
                Quaternion.LookRotation(
                    diagonalDirection,
                    up
                );


            newStake.transform.position =
                hit.point;

            newStake.transform.rotation =
                baseRotation;
        }


        // =========================================
        // 地面
        // =========================================

        else
        {
            baseRotation =
                Quaternion.Euler(
                    0,
                    0,
                    0
                );


            Quaternion rotationOffset =
                Quaternion.Euler(
                    0,
                    currentRotation,
                    0
                );


            newStake.transform.position =
                hit.point;

            newStake.transform.rotation =
                baseRotation *
                rotationOffset;
        }


        // =========================================
        // めり込みチェック
        // =========================================

        bool isEmbedded =
            CheckIfStakeEmbedded(
                newStake,
                normal,
                isWall
            );


        Rigidbody rb =
            newStake.GetComponent<Rigidbody>();


        if (isEmbedded)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }


        // =========================================
        // Collider有効化
        // =========================================

        Collider[] colliders =
            newStake.GetComponentsInChildren<Collider>();


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
        // 杭がなくなったら終了
        // =========================================

        if (inventory.GetWoodenStake() <= 0)
        {
            CancelPlacement();
        }
    }


    // =========================================
    // 木の橋を実際に設置
    // =========================================

    private void TryPlaceWoodBridge()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }


        if (mainCamera == null)
        {
            return;
        }


        if (float.IsInfinity(Input.mousePosition.x) ||
            float.IsInfinity(Input.mousePosition.y))
        {
            return;
        }


        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );


        RaycastHit hit;


        LayerMask currentLayers =
            placeableLayers;


        currentLayers |=
            bridgeZoneLayer;


        if (!Physics.Raycast(
            ray,
            out hit,
            1000f,
            currentLayers))
        {
            return;
        }


        if (woodBridgePrefab == null)
        {
            return;
        }


        // =========================================
        // 木橋生成
        // =========================================

        Instantiate(
            woodBridgePrefab,
            hit.point,
            Quaternion.identity
        );


        // =========================================
        // 配置完了
        // =========================================

        CancelPlacement();
    }


    // =========================================
    // 栄養アイテムを実際に設置
    // =========================================

    private void TryPlaceNutritionItem()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }


        if (mainCamera == null)
        {
            return;
        }


        if (float.IsInfinity(Input.mousePosition.x) ||
            float.IsInfinity(Input.mousePosition.y))
        {
            return;
        }


        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );


        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            100f,
            placeableLayers))
        {
            return;
        }


        if (nutritionItemPrefab == null)
        {
            return;
        }


        Instantiate(
            nutritionItemPrefab,
            hit.point +
            Vector3.up * 0.1f,
            Quaternion.identity
        );


        // 栄養アイテムを1個消費
        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();


        if (inventoryManager != null)
        {
            inventoryManager.UseNutritionItem();
        }


        CancelPlacement();
    }


    // =========================================
    // 睡蓮を実際に設置
    // =========================================

    private void TryPlaceLilyPad()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }


        if (mainCamera == null)
        {
            return;
        }


        if (float.IsInfinity(Input.mousePosition.x) ||
            float.IsInfinity(Input.mousePosition.y))
        {
            return;
        }


        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );


        RaycastHit hit;


        if (!Physics.Raycast(
            ray,
            out hit,
            1000f,
            placeableLayers))
        {
            return;
        }


        if (lilyPadPrefab == null)
        {
            return;
        }


        // =========================================
        // 睡蓮生成
        // =========================================

        Instantiate(
            lilyPadPrefab,
            hit.point +
            Vector3.up * 0.05f,
            Quaternion.identity
        );


        // 睡蓮を1個消費
        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();


        if (inventoryManager != null)
        {
            inventoryManager.UseLilyPad();
        }


        // =========================================
        // まだ睡蓮があるなら継続
        // =========================================

        if (inventoryManager != null &&
            inventoryManager.GetLilyPad() > 0)
        {
            Debug.Log(
                "PlacementManager : 睡蓮が残っているため配置モード継続"
            );
        }
        else
        {
            CancelPlacement();
        }
    }


    // =========================================
    // 重い石を実際に設置
    // =========================================

    private void TryPlaceHeavyStone()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }


        if (mainCamera == null)
        {
            return;
        }


        if (float.IsInfinity(Input.mousePosition.x) ||
            float.IsInfinity(Input.mousePosition.y))
        {
            return;
        }


        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );


        RaycastHit hit;


        // =========================================
        // 重い石は専用Layerのみ
        // =========================================

        if (!Physics.Raycast(
            ray,
            out hit,
            1000f,
            heavyStonePlaceableLayers))
        {
            return;
        }


        if (heavyStonePrefab == null)
        {
            Debug.LogWarning(
                "PlacementManager : heavyStonePrefabが設定されていません。"
            );

            return;
        }


        // =========================================
        // 重い石生成
        // =========================================

        GameObject newHeavyStone =
            Instantiate(
                heavyStonePrefab,
                hit.point +
                Vector3.up * 0.05f,
                Quaternion.identity
            );


        Debug.Log(
            "PlacementManager : 重い石を配置しました。"
        );


        // =========================================
        // TekoSystemへ直接通知
        // =========================================

        TekoSystem tekoSystem =
            FindFirstObjectByType<TekoSystem>();


        if (tekoSystem != null)
        {
            Debug.Log(
                "PlacementManager : TekoSystemに重い石配置を通知します。"
            );


            tekoSystem.重い石を受け取った(
                newHeavyStone
            );
        }
        else
        {
            Debug.LogWarning(
                "PlacementManager : TekoSystemが見つかりません。"
            );
        }


        // =========================================
        // インベントリから1個消費
        // =========================================

        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();


        if (inventoryManager != null)
        {
            inventoryManager.UseHeavyStone();
        }


        // =========================================
        // まだ重い石が残っているか
        // =========================================

        if (inventoryManager != null &&
            inventoryManager.GetHeavyStone() > 0)
        {
            Debug.Log(
                "PlacementManager : 重い石が残っているため配置モード継続"
            );
        }
        else
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
        placingWoodBridge = false;
        placingNutritionItem = false;
        placingLilyPad = false;
        placingHeavyStone = false;


        isRotating = false;


        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }


    // =========================================
    // 配置可能かチェック
    // =========================================

    private bool CanPlace()
    {
        return
            Time.time -
            placementStartTime >=
            placementDelay;
    }


    // =========================================
    // 橋配置エリア内かチェック
    // =========================================

    private bool IsInBridgeZone(Vector3 point)
    {
        Collider[] bridgeZones =
            Physics.OverlapBox(
                point,
                Vector3.one * 0.1f,
                Quaternion.identity,
                bridgeZoneLayer
            );


        return bridgeZones.Length > 0;
    }


    // =========================================
    // 杭が地面に触れているかチェック
    // =========================================

    private bool CheckIfStakeEmbedded(
        GameObject stake,
        Vector3 surfaceNormal,
        bool isWall)
    {
        if (isWall)
        {
            // =====================================
            // 壁
            // =====================================

            Vector3 stakeDirection =
                stake.transform.forward;


            Ray ray =
                new Ray(
                    stake.transform.position,
                    stakeDirection
                );


            RaycastHit hit;


            if (Physics.Raycast(
                ray,
                out hit,
                stakeDepth * 2,
                placeableLayers))
            {
                return true;
            }
        }
        else
        {
            // =====================================
            // 地面
            // =====================================

            Vector3 downward =
                Vector3.down;


            Ray ray =
                new Ray(
                    stake.transform.position,
                    downward
                );


            RaycastHit hit;


            if (Physics.Raycast(
                ray,
                out hit,
                0.1f,
                placeableLayers))
            {
                return true;
            }
        }


        return false;
    }
}