using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [Header("カメラ設定")]
    [Tooltip("カメラが追従するPlayer")]
    public Transform target;

    [Tooltip("なめらかに追従する速度")]
    public float smoothSpeed = 5f;

    [SerializeField]
    private float minX;

    void LateUpdate()
    {
        if (target == null)
            return;

        // PlayerのX位置に追従
        float cameraX = Mathf.Max(minX, target.position.x);

        // PlayerのY位置に追従
        // Zは2.5DなのでPlayerのZを使用しない
        Vector3 targetPosition = new Vector3(
            cameraX,
            target.position.y + 2.5f,
            transform.position.z
        );

        // なめらかに追従
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}