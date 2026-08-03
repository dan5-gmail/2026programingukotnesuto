using UnityEngine;
// ChatGTP一部使用
[RequireComponent(typeof(Collider))]
public class EditorLogScroll : MonoBehaviour
{
    [SerializeField]
    private float wheelThreshold = 0.05f;

    private bool hover;

    void Update()
    {
        hover = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == GetComponent<Collider>())
            {
                hover = true;
            }
        }

        if (!hover) return;

        float wheel = Input.GetAxis("Mouse ScrollWheel");

        if (wheel > wheelThreshold)
        {
            EditorLogManager.Instance.Scroll(1);
        }
        else if (wheel < -wheelThreshold)
        {
            EditorLogManager.Instance.Scroll(-1);
        }
    }
}