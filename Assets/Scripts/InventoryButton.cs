using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    [Header("開くパネル")]
    [SerializeField] private GameObject inventoryPanelObject;

    [Header("明るさ")]
    [SerializeField] private float normalBrightness = 1.0f;
    [SerializeField] private float hoverBrightness = 1.25f;

    private Renderer[] renderers;
    private Color[] originalColors;
    private InventoryPanel inventoryPanel;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_BaseColor"))
            {
                originalColors[i] = renderers[i].material.color;
            }
        }

        if (inventoryPanelObject != null)
        {
            inventoryPanel = inventoryPanelObject.GetComponent<InventoryPanel>();
        }

        SetBrightness(normalBrightness);
    }

    private void OnMouseEnter()
    {
        SetBrightness(hoverBrightness);
    }

    private void OnMouseExit()
    {
        SetBrightness(normalBrightness);
    }

    private void OnMouseDown()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.TogglePanel();
        }
    }

    private void SetBrightness(float brightness)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (!renderers[i].material.HasProperty("_BaseColor"))
                continue;

            Color color = originalColors[i];

            color.r = Mathf.Clamp01(color.r * brightness);
            color.g = Mathf.Clamp01(color.g * brightness);
            color.b = Mathf.Clamp01(color.b * brightness);

            renderers[i].material.color = color;
        }
    }
}