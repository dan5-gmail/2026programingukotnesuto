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

    [Header("ボトル回収")]
    [SerializeField]
    private float collectRange = 5f;

    [SerializeField]
    private float collectSpeed = 5f;

    private Bottle bottle;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        bottle = FindFirstObjectByType<Bottle>();
    }



    void Update()
    {
        CollectBottle();
    }


    private void CollectBottle()
    {
        if (bottle == null) return;

        // Z軸含めない
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 bottlePos = new Vector2(bottle.transform.position.x, bottle.transform.position.y);

        float distance = Vector2.Distance(myPos, bottlePos);

        if (distance <= collectRange)
        {
            // 距離が近ければ近いほど速くなる
            float speed = collectSpeed * (collectRange - distance + 0.1f);

            transform.position = Vector3.MoveTowards(
                transform.position,
                bottle.transform.position,
                speed * Time.fixedDeltaTime
            );

            // 近づいたら回収
            if (distance < 0.04f)
            {
                bottle.AddElement(elementType);
                Destroy(gameObject);
            }
        }
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