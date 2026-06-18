// using Unity.Mathematics;
// using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using Unity.Mathematics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [Header("カメラ設定")]
    [Tooltip("カメラ追従物設定")]
    public Transform target;
    [Tooltip("なめらかに追従設定")]
    public float smoothSpeed = 5f;

    [SerializeField]
    private float minX; //カメラ限度左端設定


    // <summary>
    // カメラ位置
    // </summary>

    void LateUpdate()
    {
        float cameraX = Mathf.Max(minX, target.position.x);

        Vector3 targetPosition = new Vector3(
            cameraX,
            target.position.y + 4,
         transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

    }

}
