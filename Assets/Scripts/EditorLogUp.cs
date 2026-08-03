using UnityEngine;

public class EditorLogUp : MonoBehaviour
{

    // 一部チャットgpt使用
    [Header("移動距離")]
    [SerializeField]
    private float moveHeight = 0.4f;

    [Header("移動速度")]
    [SerializeField]
    private float moveSpeed = 8f;

    [Header("Hover判定")]
    [SerializeField]
    private float hoverDistance = 0.8f;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        startPos = transform.localPosition;
        targetPos = startPos;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool hover = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                hover = true;
            }
        }

        if (hover)
        {
            targetPos = startPos + Vector3.up * moveHeight;
        }
        else
        {
            targetPos = startPos;
        }

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }
}