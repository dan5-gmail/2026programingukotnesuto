using UnityEngine;

public class NutritionReceiver : MonoBehaviour
{
    [Header("木の成長")]
    [SerializeField]
    private TreeGrowth treeGrowth;

    private void Awake()
    {
        if (treeGrowth == null)
        {
            treeGrowth =
                GetComponentInParent<TreeGrowth>();
        }
    }

    // =========================================
    // TreeGrowth取得
    // =========================================
    public TreeGrowth GetTreeGrowth()
    {
        return treeGrowth;
    }
}