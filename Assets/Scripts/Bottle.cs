using UnityEngine;

public class Bottle : MonoBehaviour
{
    public int wood;
    public int leaf;
    public int stone;

    public void AddElement(Element.ElementType type)
    {
        switch (type)
        {
            case Element.ElementType.Wood:
                wood++;
                break;

            case Element.ElementType.Leaf:
                leaf++;
                break;

            case Element.ElementType.Stone:
                stone++;
                break;
        }
    }
}
