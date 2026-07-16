using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshCollider))]
public class DecorationDestroy : MonoBehaviour
{

    public enum DecorationType
    {
        Tree,
        Leaf,
        Stone
    }
    [Header("装飾物の種類選択")]
    public DecorationType decorationType;

    [Header("生成物")]
    [Tooltip("ElementPrefabアタッチ")]
    [SerializeField]
    private GameObject elementPrefab;

    [Header("上方向力")]
    [SerializeField]
    private float upForce;

    [SerializeField]
    private int minDrop; //アイテムドロップ数最小数値

    [SerializeField]
    private int maxDrop; //アイテムドロップ数最大数値

    private BoxCollider bc;

    private void Start()
    {
        bc = GetComponent<BoxCollider>();
    }


    private void Update()
    {
        MouseTouch();
    }

    private void MouseTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == bc)
                {
                    spawnElement();

                    Destroy(gameObject);
                }
            }
        }
    }

    private void spawnElement()
    {
        int dropCount = Random.Range(minDrop, maxDrop + 1);

        for (int i = 0; i < dropCount; i++)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

            GameObject element = Instantiate(elementPrefab, spawnPos, Quaternion.identity);

            Element elementScript = element.GetComponent<Element>();

            switch (decorationType)
            {
                case DecorationType.Tree:
                    elementScript.SetElement(Element.ElementType.Wood);
                    break;

                case DecorationType.Leaf:
                    elementScript.SetElement(Element.ElementType.Leaf);
                    break;

                case DecorationType.Stone:
                    elementScript.SetElement(Element.ElementType.Leaf);
                    break;
            }
            Rigidbody rb = element.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
            }
        }
    }
}