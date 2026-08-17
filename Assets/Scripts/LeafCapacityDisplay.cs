using UnityEngine;
using TMPro;

public class LeafCapacityDisplay : MonoBehaviour
{
    [Header("BottleのGameObject")]
    [SerializeField] private GameObject bottleObject;

    [Header("最大表示")]
    [SerializeField] private int maxStock = 100;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (bottleObject == null)
            return;

        Bottle bottle = bottleObject.GetComponent<Bottle>();

        if (bottle == null)
            return;

        text.text = bottle.leaf.ToString();
    }
}