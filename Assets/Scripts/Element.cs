using UnityEngine;

public class Element : MonoBehaviour
{
    public enum ElementType
    {
        Leaf,
        Wood,
        Stone,
        Moss
    }


    [Header("エレメント種類")]
    public ElementType elementType;


    [Header("ボトル回収")]
    [SerializeField]
    private float collectRange = 0.04f;

    [SerializeField]
    private float collectSpeed = 0.02f;


    private Bottle bottle;
    private Renderer rend;


    private void Start()
    {
        rend = GetComponent<Renderer>();

        bottle = FindFirstObjectByType<Bottle>();

        SetColor();
    }


    private void Update()
    {
        CollectBottle();
    }


    private void CollectBottle()
    {
        if (bottle == null)
            return;


        // Z軸は無視
        Vector2 myPos =
            new Vector2(
                transform.position.x,
                transform.position.y
            );

        Vector2 bottlePos =
            new Vector2(
                bottle.transform.position.x,
                bottle.transform.position.y
            );


        float distance =
            Vector2.Distance(
                myPos,
                bottlePos
            );


        if (distance <= collectRange)
        {
            // 距離が近いほど速くなる
            float speed =
                collectSpeed *
                (collectRange - distance + 0.1f);


            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    bottle.transform.position,
                    speed * Time.deltaTime
                );


            // 十分近づいたら回収
            if (distance < 0.03f)
            {
                bottle.AddElement(
                    elementType
                );

                Destroy(gameObject);
            }
        }
    }


    public void SetElement(ElementType type)
    {
        elementType = type;

        if (rend == null)
        {
            rend =
                GetComponent<Renderer>();
        }

        SetColor();
    }


    private void SetColor()
    {
        if (rend == null)
            return;


        rend.material.EnableKeyword(
            "_EMISSION"
        );


        switch (elementType)
        {
            case ElementType.Leaf:
                {
                    Color leafColor =
                        Color.green;

                    rend.material.color =
                        leafColor;

                    rend.material.SetColor(
                        "_EmissionColor",
                        leafColor * 3f
                    );

                    break;
                }


            case ElementType.Wood:
                {
                    Color woodColor =
                        new Color(
                            0.6f,
                            0.3f,
                            0.1f
                        );

                    rend.material.color =
                        woodColor;

                    rend.material.SetColor(
                        "_EmissionColor",
                        woodColor * 1.5f
                    );

                    break;
                }


            case ElementType.Stone:
                {
                    Color stoneColor =
                        new Color(
                            0.5f,
                            0.5f,
                            0.5f
                        );

                    rend.material.color =
                        stoneColor;

                    rend.material.SetColor(
                        "_EmissionColor",
                        stoneColor * 1.5f
                    );

                    break;
                }


            case ElementType.Moss:
                {
                    Color mossColor =
                        new Color(
                            0.15f,
                            0.55f,
                            0.12f
                        );

                    rend.material.color =
                        mossColor;

                    rend.material.SetColor(
                        "_EmissionColor",
                        mossColor * 2f
                    );

                    break;
                }
        }
    }
}