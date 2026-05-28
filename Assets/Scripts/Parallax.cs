using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("追跡するカメラ")]
    [SerializeField]
    private Transform cameraTransform;

    [Header("移動倍率")]
    [SerializeField]
    private float parallaxEffect = 0.5f;

    private float startPosX;
    private float cameraStartX;

    void Start()
    {
        startPosX = transform.position.x;
        cameraStartX = cameraTransform.position.x;
    }

    void Update()
    {
        float distance =
            (cameraTransform.position.x - cameraStartX)
            * parallaxEffect;

        transform.position = new Vector3(
            startPosX + distance,
            transform.position.y,
            transform.position.z
        );
    }
}