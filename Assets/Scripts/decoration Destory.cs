using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshCollider))]
public class DecorationDestroy : MonoBehaviour
{
    [Header("生成物")]
    [Tooltip("ElementPrefabアタッチ")]
    [SerializeField]
    private GameObject elementPrefab;

    [Header("上方向力")]
    [SerializeField]
    private float upForce;


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
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

        GameObject element = Instantiate(elementPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = element.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
        }
    }
}