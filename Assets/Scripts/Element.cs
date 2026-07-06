using UnityEngine;

public class Element : MonoBehaviour
{
    public enum ElementType
    {
        Leaf,
        Wood,
        Stone
    }

    [Header("エレメント種類")]
    public ElementType elementType;


    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetElement(ElementType type)
    {
        elementType = type;

        if (rend == null)
        {
            rend = GetComponent<Renderer>();


            setColor();
        }
    }

    private void setColor()
    {
        rend.material.EnableKeyword("_EMISSION");

        switch (elementType)
        {
            case ElementType.Leaf:
                {
                    Color leafColor = Color.green;

                    rend.material.color = leafColor;
                    // 発光
                    rend.material.SetColor("_EmissionColor", leafColor * 3f);
                    break;
                }
            case ElementType.Wood:
                {
                    Color woodColor = new Color(0.6f, 0.3f, 0.1f);

                    rend.material.color = woodColor;

                    // 発光
                    rend.material.SetColor("_EmissionColor", woodColor * 1.5f);
                    break;
                }
        }
    }

}