using Unity.VisualScripting;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    [Header("アイテム等")]
    public int wood;
    public int leaf;
    public int stone;

    [Header("+1Prefab")]
    [SerializeField]
    private GameObject PlusonePrefab;

    public void AddElement(Element.ElementType type)
    {
        switch (type)
        {
            case Element.ElementType.Wood:
                wood++;
                Debug.Log("木材+1");
                break;

            case Element.ElementType.Leaf:
                leaf++;
                Debug.Log("葉+1");
                break;

            case Element.ElementType.Stone:
                stone++;
                Debug.Log("石+1");
                break;
        }
        GameManager.Instance.ItemCollected(type, 1);

        // +1表示
        if (PlusonePrefab != null)
        {
            Instantiate(
                PlusonePrefab,
                transform.position + Vector3.up * 1.2f,
                Quaternion.identity
            );

        }
    }
}
