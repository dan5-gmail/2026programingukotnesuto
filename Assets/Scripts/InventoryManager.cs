using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // =========================================
    // 木の杭
    // =========================================

    [Header("木の杭")]
    [SerializeField]
    private int woodenStake = 0;

    [Header("木の杭の最大所持数")]
    public int maxWoodenStake = 10;

    [Header("木の杭の表示")]
    [SerializeField]
    private GameObject woodStakePanel;

    [SerializeField]
    private TMP_Text woodStakeCount;


    // =========================================
    // 栄養アイテム
    // =========================================

    [Header("栄養アイテム")]
    [SerializeField]
    private int nutritionItem = 0;

    [Header("栄養アイテムの最大所持数")]
    [SerializeField]
    private int maxNutritionItem = 10;

    [Header("栄養アイテムの表示")]
    [SerializeField]
    private GameObject nutritionPanel;

    [SerializeField]
    private TMP_Text nutritionCount;


    // =========================================
    // 睡蓮
    // =========================================

    [Header("睡蓮")]
    [SerializeField]
    private int lilyPad = 0;

    [Header("睡蓮の最大所持数")]
    [SerializeField]
    private int maxLilyPad = 10;

    [Header("睡蓮の表示")]
    [SerializeField]
    private GameObject lilyPadPanel;

    [SerializeField]
    private TMP_Text lilyPadCount;


    // =========================================
    // Start
    // =========================================

    private void Start()
    {
        RefreshWoodenStakeUI();

        RefreshNutritionUI();

        RefreshHeavyStoneUI();
    }


    // =========================================
    // 木の杭
    // =========================================

    public bool CanAddWoodenStake(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        return woodenStake + amount <= maxWoodenStake;
    }


    public bool AddWoodenStake(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (!CanAddWoodenStake(amount))
        {
            return false;
        }

        woodenStake += amount;

        RefreshWoodenStakeUI();

        return true;
    }


    public bool UseWoodenStake(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (woodenStake < amount)
        {
            return false;
        }

        woodenStake -= amount;

        RefreshWoodenStakeUI();

        return true;
    }


    public int GetWoodenStake()
    {
        return woodenStake;
    }


    private void RefreshWoodenStakeUI()
    {
        if (woodenStake <= 0)
        {
            woodenStake = 0;

            if (woodStakePanel != null)
            {
                woodStakePanel.SetActive(false);
            }

            return;
        }

        if (woodStakePanel != null)
        {
            woodStakePanel.SetActive(true);
        }

        if (woodStakeCount != null)
        {
            woodStakeCount.gameObject.SetActive(true);

            woodStakeCount.text =
                woodenStake.ToString();
        }
    }


    // =========================================
    // 栄養アイテム
    // =========================================

    public bool CanAddNutritionItem(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        return nutritionItem + amount
            <= maxNutritionItem;
    }


    public bool AddNutritionItem(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (!CanAddNutritionItem(amount))
        {
            return false;
        }

        nutritionItem += amount;

        RefreshNutritionUI();

        return true;
    }


    // =========================================
    // 栄養アイテムを使用できるか
    // =========================================
    public bool CanUseNutritionItem()
    {
        return nutritionItem > 0;
    }


    // =========================================
    // 栄養アイテムを1個使用
    // =========================================
    public bool UseNutritionItem()
    {
        return UseNutritionItem(1);
    }


    // =========================================
    // 栄養アイテムを指定個数使用
    // =========================================
    public bool UseNutritionItem(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (nutritionItem < amount)
        {
            return false;
        }

        nutritionItem -= amount;

        RefreshNutritionUI();

        return true;
    }


    public int GetNutritionItem()
    {
        return nutritionItem;
    }


    // =========================================
    // 睡蓮
    // =========================================

    public bool CanAddLilyPad(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        return lilyPad + amount <= maxLilyPad;
    }


    public bool AddLilyPad(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (!CanAddLilyPad(amount))
        {
            return false;
        }

        lilyPad += amount;

        RefreshLilyPadUI();

        return true;
    }


    public bool CanUseLilyPad()
    {
        return lilyPad > 0;
    }


    public bool UseLilyPad()
    {
        return UseLilyPad(1);
    }


    public bool UseLilyPad(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (lilyPad < amount)
        {
            return false;
        }

        lilyPad -= amount;

        RefreshLilyPadUI();

        return true;
    }


    public int GetLilyPad()
    {
        return lilyPad;
    }


    private void RefreshLilyPadUI()
    {
        if (lilyPad <= 0)
        {
            lilyPad = 0;

            if (lilyPadPanel != null)
            {
                lilyPadPanel.SetActive(false);
            }

            if (lilyPadCount != null)
            {
                lilyPadCount.gameObject.SetActive(false);
                lilyPadCount.text = "";
            }

            return;
        }

        if (lilyPadPanel != null)
        {
            lilyPadPanel.SetActive(true);
        }

        if (lilyPadCount != null)
        {
            lilyPadCount.gameObject.SetActive(true);
            lilyPadCount.text = $"×{lilyPad}";
        }
    }


    private void RefreshNutritionUI()
    {
        if (nutritionItem <= 0)
        {
            nutritionItem = 0;

            if (nutritionPanel != null)
            {
                nutritionPanel.SetActive(false);
            }

            return;
        }

        if (nutritionPanel != null)
        {
            nutritionPanel.SetActive(true);
        }

        if (nutritionCount != null)
        {
            nutritionCount.gameObject.SetActive(true);

            nutritionCount.text =
                nutritionItem.ToString();
        }
    }

    // =========================================
    // 重い石
    // =========================================

    [Header("重い石")]
    [SerializeField]
    private int heavyStone = 0;

    [Header("重い石の最大所持数")]
    [SerializeField]
    private int maxHeavyStone = 10;

    [Header("重い石の表示")]
    [SerializeField]
    private GameObject heavyStonePanel;

    [SerializeField]
    private TMP_Text heavyStoneCount;

    // =========================================
    // 重い石
    // =========================================

    public bool CanAddHeavyStone(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        return heavyStone + amount <= maxHeavyStone;
    }


    public bool AddHeavyStone(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (!CanAddHeavyStone(amount))
        {
            return false;
        }

        heavyStone += amount;

        RefreshHeavyStoneUI();

        return true;
    }


    public bool CanUseHeavyStone()
    {
        return heavyStone > 0;
    }


    public bool UseHeavyStone()
    {
        return UseHeavyStone(1);
    }


    public bool UseHeavyStone(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (heavyStone < amount)
        {
            return false;
        }

        heavyStone -= amount;

        RefreshHeavyStoneUI();

        return true;
    }


    public int GetHeavyStone()
    {
        return heavyStone;
    }


    private void RefreshHeavyStoneUI()
    {
        if (heavyStone <= 0)
        {
            heavyStone = 0;

            if (heavyStonePanel != null)
            {
                heavyStonePanel.SetActive(false);
            }

            if (heavyStoneCount != null)
            {
                heavyStoneCount.gameObject.SetActive(false);
                heavyStoneCount.text = "";
            }

            return;
        }

        if (heavyStonePanel != null)
        {
            heavyStonePanel.SetActive(true);
        }

        if (heavyStoneCount != null)
        {
            heavyStoneCount.gameObject.SetActive(true);
            heavyStoneCount.text = $"×{heavyStone}";
        }
    }
}