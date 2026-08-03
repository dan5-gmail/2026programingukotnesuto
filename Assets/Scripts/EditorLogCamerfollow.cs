using UnityEngine;

public class EditorLogCamerfollow : MonoBehaviour
{
    [Header("カメラに追従")]
    [SerializeField]
    private Camera targetCamera;

    [Header("位置調整")]
    [SerializeField]
    private float forwardDistance = 3f;

    [SerializeField]
    private float leftOffset = -2.2f;

    [SerializeField]
    private float downOffset = -1.3f;

    [Header("角度・回転補正")]
    [SerializeField]
    private Vector3 rotationOffset = Vector3.zero;


    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;

        transform.position =
        targetCamera.transform.position +
        targetCamera.transform.forward * forwardDistance +
        targetCamera.transform.right * leftOffset +
        targetCamera.transform.up * downOffset;


        transform.rotation = targetCamera.transform.rotation * Quaternion.Euler(rotationOffset);

    }
}
