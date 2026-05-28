using UnityEngine;

public class Waterfloat : MonoBehaviour
{
    [Header("揺れ設定")]
    [SerializeField]
    private float speed;

    [SerializeField]
    private float height;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float moveY = Mathf.Sin(Time.time * speed) * height;


        transform.position = startPos + new Vector3(0,
        moveY,
        0);
    }
}
