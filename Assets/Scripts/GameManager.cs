using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Manager")]
    // [SerializeField]
    // private InventoryManager inventoryManager;


    [SerializeField]
    private EditorLogManager editorlogManager;

    // [SerializeField]
    // private EffectManager effectManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }


    // ================================
    // アイテム取得
    // ================================
    public void ItemCollected(Element.ElementType type, int amout)
    {
        Debug.Log("AddLogaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        // インベントリ追加
        // inventoryManager.AddElement(type, amout);

        // Log表示
        editorlogManager.AddLog(type, amout);

        // +1
        //    effectManager.ShowPlusOne(type,amout); 
    }
}
