using UnityEngine;

public class InventoryTabButton : MonoBehaviour
{
    public enum TabType
    {
        Bottle,
        Craft,
        Item
    }

    [Header("ボタンの種類")]
    [SerializeField]
    private TabType tabType;

    [Header("表示するパネル")]
    [SerializeField]
    private GameObject bottlePanel;
    [SerializeField]
    private GameObject craftPanel;
    [SerializeField]
    private GameObject itemPanel;

    [Header("明るさ")]
    [SerializeField]
    private float normalBrightness = 1.0f;
    [SerializeField]
    private float hoverBrightness = 1.25f;

    [Header("一番初めかどうか")]
    [SerializeField]
    private bool hasOpenedOnce = false;

    private Renderer[] renderers;
    private Color[] originalColors;


    private void Start()
    {
        if (!hasOpenedOnce)
        {

            bottlePanel.SetActive(true);
            craftPanel.SetActive(false);
            itemPanel.SetActive(false);

            hasOpenedOnce = true;
        }
    }

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
        // =========================================
        // Bottle
        // =========================================
        if (tabType == TabType.Bottle)
        {
            if (bottlePanel != null)
            {
                bottlePanel.SetActive(true);
            }

            if (craftPanel != null)
            {
                craftPanel.SetActive(false);
            }

            if (itemPanel != null)
            {
                itemPanel.SetActive(false);
            }
        }

        // =========================================
        // Craft
        // =========================================
        else if (tabType == TabType.Craft)
        {
            if (bottlePanel != null)
            {
                bottlePanel.SetActive(false);
            }

            if (craftPanel != null)
            {
                craftPanel.SetActive(true);
            }

            if (itemPanel != null)
            {
                itemPanel.SetActive(false);
            }
        }

        // =========================================
        // Item
        // =========================================
        else if (tabType == TabType.Item)
        {
            if (bottlePanel != null)
            {
                bottlePanel.SetActive(false);
            }

            if (craftPanel != null)
            {
                craftPanel.SetActive(false);
            }

            if (itemPanel != null)
            {
                itemPanel.SetActive(true);
            }
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