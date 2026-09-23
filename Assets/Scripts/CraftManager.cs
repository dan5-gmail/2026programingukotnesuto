using UnityEngine;

public class CraftManager : MonoBehaviour
{
    // =========================================
    // 木の杭
    // =========================================
    [Header("木の杭クラフト")]
    [SerializeField]
    private int woodCost = 1;

    [SerializeField]
    private int woodenStakeAmount = 4;


    // =========================================
    // 木の橋
    // =========================================
    [Header("木の橋クラフト")]
    [SerializeField]
    private int woodBridgeAmount = 1;

    [SerializeField]
    private int woodBridgeWoodCost = 3;

    [SerializeField]
    private int woodBridgeLeafCost = 4;


    // =========================================
    // 栄養アイテム
    // =========================================
    [Header("栄養アイテムクラフト")]
    [SerializeField]
    private int nutritionMossCost = 2;

    [SerializeField]
    private int nutritionWoodCost = 2;

    [SerializeField]
    private int nutritionLeafCost = 2;

    [SerializeField]
    private int nutritionItemAmount = 1;


    // =========================================
    // 睡蓮
    // =========================================
    [Header("睡蓮クラフト")]
    [SerializeField]
    private int lilyPadMossCost = 3;

    [SerializeField]
    private int lilyPadLeafCost = 2;

    [SerializeField]
    private int lilyPadAmount = 1;


    // =========================================
    // 木の杭クラフト
    // =========================================
    public void CraftWoodenStake()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        Bottle bottle =
            GameManager.Instance.GetBottle();

        if (bottle == null)
        {
            return;
        }

        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();

        if (inventoryManager == null)
        {
            return;
        }

        // -----------------------------
        // 木材チェック
        // -----------------------------
        if (bottle.wood < woodCost)
        {
            GameManager.Instance.CraftErrorLog(
                "Not enough wood."
            );

            return;
        }

        // -----------------------------
        // 所持上限チェック
        // -----------------------------
        if (!inventoryManager.CanAddWoodenStake(
                woodenStakeAmount))
        {
            GameManager.Instance.CraftErrorLog(
                "Wooden Stake inventory is full."
            );

            return;
        }

        // -----------------------------
        // 木材消費
        // -----------------------------
        bottle.wood -= woodCost;

        // -----------------------------
        // 木の杭追加
        // -----------------------------
        GameManager.Instance.AddWoodenStake(
            woodenStakeAmount
        );

        // -----------------------------
        // 成功ログ
        // -----------------------------
        GameManager.Instance.CraftedItem(
            "Wooden Stakes",
            woodenStakeAmount
        );
    }


    // =========================================
    // 木の橋クラフト
    // =========================================
    public void CraftWoodBridge()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        Bottle bottle =
            GameManager.Instance.GetBottle();

        if (bottle == null)
        {
            return;
        }

        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();

        if (inventoryManager == null)
        {
            return;
        }

        // -----------------------------
        // 木材チェック
        // -----------------------------
        if (bottle.wood < woodBridgeWoodCost)
        {
            GameManager.Instance.CraftErrorLog(
                "Not enough wood for bridge."
            );

            return;
        }

        // -----------------------------
        // 葉チェック
        // -----------------------------
        if (bottle.leaf < woodBridgeLeafCost)
        {
            GameManager.Instance.CraftErrorLog(
                "Not enough leaves for bridge."
            );

            return;
        }

        // -----------------------------
        // 素材消費
        // -----------------------------
        bottle.wood -= woodBridgeWoodCost;

        bottle.leaf -= woodBridgeLeafCost;

        // -----------------------------
        // 木の橋設置モード開始
        // -----------------------------
        GameManager.Instance.StartWoodBridgePlacement();

        // -----------------------------
        // 成功ログ
        // -----------------------------
        GameManager.Instance.CraftedItem(
            "Wood Bridge",
            woodBridgeAmount
        );
    }


    // =========================================
    // 栄養アイテムクラフト
    // =========================================
    public void CraftNutritionItem()
    {
        Debug.Log("CraftManager : 栄養アイテムクラフト開始");

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "CraftManager : GameManager.Instanceがnull"
            );

            return;
        }

        Bottle bottle =
            GameManager.Instance.GetBottle();

        if (bottle == null)
        {
            Debug.LogError(
                "CraftManager : Bottleがnull"
            );

            return;
        }

        Debug.Log(
            $"CraftManager : 素材確認 - " +
            $"Wood: {bottle.wood}, " +
            $"Leaf: {bottle.leaf}, " +
            $"Moss: {bottle.moss}"
        );

        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();

        if (inventoryManager == null)
        {
            Debug.LogError(
                "CraftManager : InventoryManagerがnull"
            );

            return;
        }

        // -----------------------------
        // Mossチェック
        // -----------------------------
        if (bottle.moss < nutritionMossCost)
        {
            Debug.Log(
                $"CraftManager : Moss不足 - " +
                $"所持: {bottle.moss}, " +
                $"必要: {nutritionMossCost}"
            );

            GameManager.Instance.CraftErrorLog(
                "Not enough moss."
            );

            return;
        }

        // -----------------------------
        // Woodチェック
        // -----------------------------
        if (bottle.wood < nutritionWoodCost)
        {
            Debug.Log(
                $"CraftManager : Wood不足 - " +
                $"所持: {bottle.wood}, " +
                $"必要: {nutritionWoodCost}"
            );

            GameManager.Instance.CraftErrorLog(
                "Not enough wood for nutrition item."
            );

            return;
        }

        // -----------------------------
        // Leafチェック
        // -----------------------------
        if (bottle.leaf < nutritionLeafCost)
        {
            Debug.Log(
                $"CraftManager : Leaf不足 - " +
                $"所持: {bottle.leaf}, " +
                $"必要: {nutritionLeafCost}"
            );

            GameManager.Instance.CraftErrorLog(
                "Not enough leaves for nutrition item."
            );

            return;
        }

        // -----------------------------
        // 栄養アイテム所持上限チェック
        // -----------------------------
        if (!inventoryManager.CanAddNutritionItem(
                nutritionItemAmount))
        {
            Debug.Log(
                "CraftManager : 栄養アイテム所持上限オーバー"
            );

            GameManager.Instance.CraftErrorLog(
                "Nutrition item inventory is full."
            );

            return;
        }

        // -----------------------------
        // Moss消費
        // -----------------------------
        bottle.moss -= nutritionMossCost;

        // -----------------------------
        // Wood消費
        // -----------------------------
        bottle.wood -= nutritionWoodCost;

        // -----------------------------
        // Leaf消費
        // -----------------------------
        bottle.leaf -= nutritionLeafCost;

        // -----------------------------
        // 栄養アイテム追加
        // -----------------------------
        inventoryManager.AddNutritionItem(
            nutritionItemAmount
        );

        // -----------------------------
        // 成功ログ
        // -----------------------------
        GameManager.Instance.CraftedItem(
            "Nutrition Item",
            nutritionItemAmount
        );

        Debug.Log(
            "CraftManager : 栄養アイテムクラフト成功"
        );
    }


    // =========================================
    // 睡蓮クラフト
    // =========================================
    public void CraftLilyPad()
    {
        Debug.Log("CraftManager : 睡蓮クラフト開始");

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "CraftManager : GameManager.Instanceがnull"
            );

            return;
        }

        Bottle bottle =
            GameManager.Instance.GetBottle();

        if (bottle == null)
        {
            Debug.LogError(
                "CraftManager : Bottleがnull"
            );

            return;
        }

        // -----------------------------
        // Mossチェック
        // -----------------------------
        if (bottle.moss < lilyPadMossCost)
        {
            GameManager.Instance.CraftErrorLog(
                "Not enough moss for lily pad."
            );

            return;
        }

        // -----------------------------
        // Leafチェック
        // -----------------------------
        if (bottle.leaf < lilyPadLeafCost)
        {
            GameManager.Instance.CraftErrorLog(
                "Not enough leaves for lily pad."
            );

            return;
        }

        // -----------------------------
        // Moss消費
        // -----------------------------
        bottle.moss -= lilyPadMossCost;

        // -----------------------------
        // Leaf消費
        // -----------------------------
        bottle.leaf -= lilyPadLeafCost;

        // -----------------------------
        // InventoryManager取得
        // -----------------------------
        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();

        if (inventoryManager == null)
        {
            return;
        }

        // -----------------------------
        // 睡蓮所持上限チェック
        // -----------------------------
        if (!inventoryManager.CanAddLilyPad(
                lilyPadAmount))
        {
            GameManager.Instance.CraftErrorLog(
                "Lily pad inventory is full."
            );

            return;
        }

        // -----------------------------
        // 睡蓮追加
        // -----------------------------
        inventoryManager.AddLilyPad(
            lilyPadAmount
        );

        // -----------------------------
        // 成功ログ
        // -----------------------------
        GameManager.Instance.CraftedItem(
            "Lily Pad",
            lilyPadAmount
        );

        Debug.Log("CraftManager : 睡蓮クラフト成功");
    }

    // =========================================
    // 重い石
    // =========================================

    [Header("重い石クラフト")]
    [SerializeField]
    private int heavyStoneStoneCost = 2;

    [SerializeField]
    private int heavyStoneAmount = 1;


    // =========================================
    // 重い石クラフト
    // =========================================
    public void CraftHeavyStone()
    {
        Debug.Log("CraftManager : 重い石クラフト開始");

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "CraftManager : GameManager.Instanceがnull"
            );

            return;
        }

        Bottle bottle =
            GameManager.Instance.GetBottle();

        if (bottle == null)
        {
            Debug.LogError(
                "CraftManager : Bottleがnull"
            );

            return;
        }

        InventoryManager inventoryManager =
            GameManager.Instance.GetInventoryManager();

        if (inventoryManager == null)
        {
            Debug.LogError(
                "CraftManager : InventoryManagerがnull"
            );

            return;
        }

        // -----------------------------
        // Stoneチェック
        // -----------------------------
        if (bottle.stone < heavyStoneStoneCost)
        {
            GameManager.Instance.CraftErrorLog(
                "Not enough stone for heavy stone."
            );

            return;
        }

        // -----------------------------
        // 所持上限チェック
        // -----------------------------
        if (!inventoryManager.CanAddHeavyStone(
                heavyStoneAmount))
        {
            GameManager.Instance.CraftErrorLog(
                "Heavy stone inventory is full."
            );

            return;
        }

        // -----------------------------
        // Stone消費
        // -----------------------------
        bottle.stone -= heavyStoneStoneCost;

        // -----------------------------
        // 重い石追加
        // -----------------------------
        inventoryManager.AddHeavyStone(
            heavyStoneAmount
        );

        // -----------------------------
        // 成功ログ
        // -----------------------------
        GameManager.Instance.CraftedItem(
            "Heavy Stone",
            heavyStoneAmount
        );

        Debug.Log(
            "CraftManager : 重い石クラフト成功"
        );
    }
}