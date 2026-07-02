using UnityEngine;

public class Element : MonoBehaviour
{
    public enum ElementType
    {
        Leaf,
        Wood
    }

    [Header("エレメント種類")]
    public ElementType elementType;


    private Renderer render;

    void Start()
    {
        render = GetComponent<Renderer>();

        switch (elementType)
        {
            case ElementType.Leaf:
                render.material.color = Color.green;
                break;

            case ElementType.Wood:
                render.material.color = new Color(0.6f, 0.3f, 0.1f);
                break;
        }
    }
}
