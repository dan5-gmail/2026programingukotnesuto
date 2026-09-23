using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshCollider))]
public class DecorationDestroy : MonoBehaviour
{
    public enum DecorationType
    {
        Tree,
        Leaf,
        Stone,
        Moss
    }


    [Header("装飾物の種類選択")]
    public DecorationType decorationType;


    [Header("生成物")]
    [Tooltip("ElementPrefabをアタッチ")]
    [SerializeField]
    private GameObject elementPrefab;


    [Header("上方向力")]
    [SerializeField]
    private float upForce;


    [Header("ドロップ数")]
    [SerializeField]
    private int minDrop = 1;

    [SerializeField]
    private int maxDrop = 1;


    private BoxCollider bc;


    private void Start()
    {
        bc =
            GetComponent<BoxCollider>();
    }


    private void Update()
    {
        MouseTouch();
    }


    private void MouseTouch()
    {
        if (!Input.GetMouseButtonDown(0))
            return;


        Camera mainCamera =
            Camera.main;


        if (mainCamera == null)
            return;


        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );


        if (
            Physics.Raycast(
                ray,
                out RaycastHit hit
            )
        )
        {
            if (hit.collider == bc)
            {
                SpawnElement();

                Destroy(gameObject);
            }
        }
    }


    private void SpawnElement()
    {
        int dropCount =
            Random.Range(
                minDrop,
                maxDrop + 1
            );


        for (int i = 0; i < dropCount; i++)
        {
            Vector3 spawnPos =
                transform.position +
                Vector3.up * 0.5f;


            GameObject element =
                Instantiate(
                    elementPrefab,
                    spawnPos,
                    Quaternion.identity
                );


            Element elementScript =
                element.GetComponent<Element>();


            if (elementScript == null)
            {
                Debug.LogError(
                    "DecorationDestroy : " +
                    "ElementPrefabにElement.csがありません。"
                );

                continue;
            }


            switch (decorationType)
            {
                case DecorationType.Tree:

                    elementScript.SetElement(
                        Element.ElementType.Wood
                    );

                    break;


                case DecorationType.Leaf:

                    elementScript.SetElement(
                        Element.ElementType.Leaf
                    );

                    break;


                case DecorationType.Stone:

                    elementScript.SetElement(
                        Element.ElementType.Stone
                    );

                    break;


                case DecorationType.Moss:

                    elementScript.SetElement(
                        Element.ElementType.Moss
                    );

                    break;
            }


            Rigidbody rb =
                element.GetComponent<Rigidbody>();


            if (rb != null)
            {
                rb.AddForce(
                    Vector3.up * upForce,
                    ForceMode.Impulse
                );
            }
        }
    }
}