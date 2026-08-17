using UnityEngine;
using TMPro;

public class LeafCountDisplay : MonoBehaviour
{
    [Header("BottleのGameObject")]
    [SerializeField] private GameObject bottleObject;

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

        text.text = "×" + bottle.leaf;
    }
}